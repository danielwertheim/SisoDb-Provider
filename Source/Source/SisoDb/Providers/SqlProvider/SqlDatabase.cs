using System;
using System.Data.SqlClient;
using SisoDb.Core;
using SisoDb.Providers.DbSchema;
using SisoDb.Providers.SqlProvider.DbSchema;
using SisoDb.Querying;
using SisoDb.Querying.Lambdas.Processors.Sql;
using SisoDb.Resources;
using SisoDb.Structures;
using SisoDb.Structures.Schemas;

namespace SisoDb.Providers.SqlProvider
{
    public class SqlDatabase : ISqlDatabase
    {
        //private readonly ISisoProviderFactory _providerFactory;

        public string Name { get; private set; }

        public ISisoConnectionInfo ServerConnectionInfo { get; private set; }

        public ISisoConnectionInfo ConnectionInfo { get; private set; }

        public IDbSchemaManager DbSchemaManager { get; set; }

        public IStructureSchemas StructureSchemas { get; set; }

        public IStructureBuilder StructureBuilder { get; set; }

        public SqlDatabase(ISisoConnectionInfo connectionInfo)
        {
            if (connectionInfo.ProviderType != StorageProviders.Sql2008)
                throw new SisoDbException(ExceptionMessages.SqlDatabase_UnsupportedProviderSpecified
                    .Inject(connectionInfo.ProviderType, StorageProviders.Sql2008));

            InitializeConnectionInfo(connectionInfo);

            //_providerFactory = SisoEnvironment.GetProviderFactory(ConnectionInfo.ProviderType);

            StructureSchemas = SisoEnvironment.Resources.ResolveStructureSchemas();
            DbSchemaManager = new DbSchemaManager();
            StructureBuilder = new StructureBuilder(
                SisoEnvironment.Resources.ResolveJsonSerializer(), 
                new SisoIdFactory(), 
                new StructureIndexesFactory(SisoEnvironment.Formatting.StringConverter));
        }

        private void InitializeConnectionInfo(ISisoConnectionInfo connectionInfo)
        {
            ConnectionInfo = connectionInfo;

            var cnStringBuilder = new SqlConnectionStringBuilder(ConnectionInfo.ConnectionString.PlainString);
            
            Name = cnStringBuilder.InitialCatalog;
            if(string.IsNullOrWhiteSpace(Name))
                throw new SisoDbException(ExceptionMessages.SqlDatabase_ConnectionInfo_MissingName);

            cnStringBuilder.InitialCatalog = string.Empty;
            var cnString = connectionInfo.ConnectionString.ReplacePlain(cnStringBuilder.ConnectionString);
            ServerConnectionInfo = new SisoConnectionInfo(cnString);
        }

        public virtual void EnsureNewDatabase()
        {
            using (var client = new SqlDbClient(ServerConnectionInfo, false))
            {
                if (client.DatabaseExists(Name))
                    client.DropDatabase(Name);

                client.CreateDatabase(Name);
            }
        }

        public virtual void CreateIfNotExists()
        {
            using (var client = new SqlDbClient(ServerConnectionInfo, false))
            {
                if (!client.DatabaseExists(Name))
                    client.CreateDatabase(Name);
            }
        }

        public virtual void InitializeExisting()
        {
            using (var client = new SqlDbClient(ServerConnectionInfo, false))
            {
                if (!client.DatabaseExists(Name))
                    throw new SisoDbException(ExceptionMessages.SqlDatabase_InitializeExisting_DbDoesNotExist.Inject(Name));

                client.CreateSysTables(Name);
            }
        }

        public virtual void DeleteIfExists()
        {
            using (var client = new SqlDbClient(ServerConnectionInfo, false))
            {
                if (client.DatabaseExists(Name))
                    client.DropDatabase(Name);
            }
        }

        public virtual bool Exists()
        {
            using (var client = new SqlDbClient(ServerConnectionInfo, false))
            {
                return client.DatabaseExists(Name);
            }
        }

        public void DropStructureSet<T>() where T : class
        {
            using (var client = new SqlDbClient(ConnectionInfo, false))
            {
                var dropper = new SqlDbSchemaDropper(client);
                var structureSchema = StructureSchemas.GetSchema(TypeFor<T>.Type);
                DbSchemaManager.DropStructureSet(structureSchema, dropper);
                StructureSchemas.RemoveSchema(TypeFor<T>.Type);
            }
        }

        public void UpsertStructureSet<T>() where T : class
        {
            using (var client = new SqlDbClient(ConnectionInfo, false))
            {
                var upserter = new SqlDbSchemaUpserter(client);
                var structureSchema = StructureSchemas.GetSchema(TypeFor<T>.Type);
                DbSchemaManager.UpsertStructureSet(structureSchema, upserter);
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
                dbClient, dbClientNonTrans, identityGenerator, 
                DbSchemaManager, dbSchemaUpserter,
                StructureSchemas, StructureBuilder, 
                SisoEnvironment.Resources.ResolveJsonSerializer(), queryGenerator,
                commandBuilderFactory);
        }
    }
}