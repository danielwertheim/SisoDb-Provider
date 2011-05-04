using System;
using System.Data.SqlClient;
using SisoDb.Core;
using SisoDb.Providers.DbSchema;
using SisoDb.Providers.Sql2008.DbSchema;
using SisoDb.Querying;
using SisoDb.Querying.Lambdas.Processors.Sql;
using SisoDb.Resources;
using SisoDb.Structures;
using SisoDb.Structures.Schemas;

namespace SisoDb.Providers.Sql2008
{
    public class Sql2008Database : ISqlDatabase
    {
        
        public string Name { get; private set; }

        public ISisoConnectionInfo ServerConnectionInfo { get; private set; }

        public ISisoConnectionInfo ConnectionInfo { get; protected set; }

        public IStructureSchemas StructureSchemas { get; set; }

        public IStructureBuilder StructureBuilder { get; set; }

        public IDbSchemaManager DbSchemaManager { get; set; }

        protected Sql2008Database()
        {
            StructureSchemas = SisoEnvironment.Resources.ResolveStructureSchemas();
            StructureBuilder = SisoEnvironment.Resources.ResolveStructureBuilder();
            DbSchemaManager = SisoEnvironment.Resources.ResolveDbSchemaManager();
        }

        internal Sql2008Database(ISisoConnectionInfo connectionInfo) : this()
        {
            ConnectionInfo = connectionInfo.AssertNotNull("connectionInfo");

            if (ConnectionInfo.ProviderType != StorageProviders.Sql2008)
                throw new SisoDbException(ExceptionMessages.SqlDatabase_UnsupportedProviderSpecified
                    .Inject(ConnectionInfo.ProviderType, StorageProviders.Sql2008));

            Initialize();
        }

        protected void Initialize()
        {
            var cnStringBuilder = new SqlConnectionStringBuilder(ConnectionInfo.ConnectionString.PlainString);
            
            Name = cnStringBuilder.InitialCatalog;
            if(string.IsNullOrWhiteSpace(Name))
                throw new SisoDbException(ExceptionMessages.SqlDatabase_ConnectionInfo_MissingName);

            cnStringBuilder.InitialCatalog = string.Empty;
            var cnString = ConnectionInfo.ConnectionString.ReplacePlain(cnStringBuilder.ConnectionString);
            ServerConnectionInfo = new SisoConnectionInfo(cnString);
        }

        public virtual void EnsureNewDatabase()
        {
            using (var client = new SqlServerClient(ServerConnectionInfo))
            {
                if (client.DatabaseExists(Name))
                    client.DropDatabase(Name);

                client.CreateDatabase(Name);
            }
        }

        public virtual void CreateIfNotExists()
        {
            using (var client = new SqlServerClient(ServerConnectionInfo))
            {
                if (!client.DatabaseExists(Name))
                    client.CreateDatabase(Name);
            }
        }

        public virtual void InitializeExisting()
        {
            using (var client = new SqlServerClient(ServerConnectionInfo))
            {
                if (!client.DatabaseExists(Name))
                    throw new SisoDbException(ExceptionMessages.SqlDatabase_InitializeExisting_DbDoesNotExist.Inject(Name));

                client.InitializeExistingDb(Name);
            }
        }

        public virtual void DeleteIfExists()
        {
            using (var client = new SqlServerClient(ServerConnectionInfo))
            {
                if (client.DatabaseExists(Name))
                    client.DropDatabase(Name);
            }
        }

        public virtual bool Exists()
        {
            using (var client = new SqlServerClient(ServerConnectionInfo))
            {
                return client.DatabaseExists(Name);
            }
        }

        public void DropStructureSet<T>() where T : class
        {
            using (var client = new SqlDbClient(ConnectionInfo, true))
            {
                var dropper = new SqlDbSchemaDropper(client);
                var structureSchema = StructureSchemas.GetSchema(TypeFor<T>.Type);
                DbSchemaManager.DropStructureSet(structureSchema, dropper);
                StructureSchemas.RemoveSchema(TypeFor<T>.Type);

                client.Flush();
            }
        }

        public void UpsertStructureSet<T>() where T : class
        {
            using (var client = new SqlDbClient(ConnectionInfo, true))
            {
                var upserter = new SqlDbSchemaUpserter(client);
                var structureSchema = StructureSchemas.GetSchema(TypeFor<T>.Type);
                DbSchemaManager.UpsertStructureSet(structureSchema, upserter);

                client.Flush();
            }
        }

        public void UpdateStructureSet<TOld, TNew>(Func<TOld, TNew, StructureSetUpdaterStatuses> onProcess)
            where TOld : class 
            where TNew : class
        {
            var structureSchemaOld = StructureSchemas.GetSchema(TypeFor<TOld>.Type);

            StructureSchemas.RemoveSchema(TypeFor<TOld>.Type);
            
            var structureSchemaNew = StructureSchemas.GetSchema(TypeFor<TNew>.Type);
            
            var updater = new SqlStructureSetUpdater<TOld, TNew>(ConnectionInfo, structureSchemaOld, structureSchemaNew, StructureBuilder);
            
            updater.Process(onProcess);
        }

        public IQueryEngine CreateQueryEngine()
        {
            var dbClient = new SqlDbClient(ConnectionInfo, false);
            var memberNameGenerator = SisoEnvironment.Resources.ResolveMemberNameGenerator();

            var queryGenerator = new SqlQueryGenerator(
                new ParsedWhereSqlProcessor(memberNameGenerator),
                new ParsedSortingSqlProcessor(memberNameGenerator),
                new ParsedIncludeSqlProcessor(memberNameGenerator));
            
            return new SqlQueryEngine(
                dbClient,
                DbSchemaManager,
                new SqlDbSchemaUpserter(dbClient),
                StructureSchemas,
                SisoEnvironment.Resources.ResolveJsonSerializer(), 
                SisoEnvironment.Resources.ResolveJsonBatchDeserializer(),
                queryGenerator,
                new CommandBuilderFactory());
        }

        public IUnitOfWork CreateUnitOfWork()
        {
            var dbClient = new SqlDbClient(ConnectionInfo, true);
            var dbClientNonTrans = new SqlDbClient(ConnectionInfo, false);
            var memberNameGenerator = SisoEnvironment.Resources.ResolveMemberNameGenerator();

            var queryGenerator = new SqlQueryGenerator(
                new ParsedWhereSqlProcessor(memberNameGenerator),
                new ParsedSortingSqlProcessor(memberNameGenerator),
                new ParsedIncludeSqlProcessor(memberNameGenerator));

            var commandBuilderFactory = new CommandBuilderFactory();
            var dbSchemaUpserter = new SqlDbSchemaUpserter(dbClientNonTrans);
            var identityGenerator = new SqlIdentityGenerator(dbClientNonTrans);

            return new SqlUnitOfWork(
                dbClient, identityGenerator, 
                DbSchemaManager, dbSchemaUpserter,
                StructureSchemas, StructureBuilder, 
                SisoEnvironment.Resources.ResolveJsonSerializer(),
                SisoEnvironment.Resources.ResolveJsonBatchDeserializer(),
                queryGenerator,
                commandBuilderFactory);
        }
    }
}