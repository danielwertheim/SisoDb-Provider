using System;
using SisoDb.Core;
using SisoDb.DbSchema;
using SisoDb.Querying;
using SisoDb.Querying.Lambdas.Processors.Sql;
using SisoDb.Resources;
using SisoDb.Sql2008.Dac;
using SisoDb.Sql2008.DbSchema;
using SisoDb.Structures;
using SisoDb.Structures.Schemas;

namespace SisoDb.Sql2008
{
    public class SqlDatabase : ISisoDatabase
    {
        private readonly SqlConnectionInfo _innerConnectionInfo;

        public string Name
        {
            get { return _innerConnectionInfo.Name; }
        }

        public ISisoConnectionInfo ConnectionInfo
        {
            get { return _innerConnectionInfo; }
        }

        public IStructureSchemas StructureSchemas { get; set; }

        public IStructureBuilder StructureBuilder { get; set; }

        public IDbSchemaManager DbSchemaManager { get; set; }

        protected internal SqlDatabase(ISisoConnectionInfo connectionInfo)
        {
            _innerConnectionInfo = new SqlConnectionInfo(connectionInfo.AssertNotNull("connectionInfo"));

            StructureSchemas = SisoEnvironment.Resources.ResolveStructureSchemas();
            StructureBuilder = SisoEnvironment.Resources.ResolveStructureBuilder();
            DbSchemaManager = SisoEnvironment.Resources.ResolveDbSchemaManager();
        }

        public virtual void EnsureNewDatabase()
        {
            using (var client = new SqlServerClient(_innerConnectionInfo))
            {
                client.DropDatabaseIfExists(Name);

                client.CreateDatabase(Name);
            }
        }

        public virtual void CreateIfNotExists()
        {
            using (var client = new SqlServerClient(_innerConnectionInfo))
            {
                if (!client.DatabaseExists(Name))
                    client.CreateDatabase(Name);
            }
        }

        public virtual void InitializeExisting()
        {
            using (var client = new SqlServerClient(_innerConnectionInfo))
            {
                if (!client.DatabaseExists(Name))
                    throw new SisoDbException(ExceptionMessages.SqlDatabase_InitializeExisting_DbDoesNotExist.Inject(Name));

                client.InitializeExistingDb(Name);
            }
        }

        public virtual void DeleteIfExists()
        {
            using (var client = new SqlServerClient(_innerConnectionInfo))
            {
                client.DropDatabaseIfExists(Name);
            }
        }

        public virtual bool Exists()
        {
            using (var client = new SqlServerClient(_innerConnectionInfo))
            {
                return client.DatabaseExists(Name);
            }
        }

        public void DropStructureSet<T>() where T : class
        {
            using (var client = new SqlDbClient(_innerConnectionInfo, true))
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
            using (var client = new SqlDbClient(_innerConnectionInfo, true))
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
            
            var updater = new SqlStructureSetUpdater<TOld, TNew>(_innerConnectionInfo, structureSchemaOld, structureSchemaNew, StructureBuilder);
            
            updater.Process(onProcess);
        }

        public IQueryEngine CreateQueryEngine()
        {
            var dbClient = new SqlDbClient(_innerConnectionInfo, false);
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
            var dbClient = new SqlDbClient(_innerConnectionInfo, true);
            var dbClientNonTrans = new SqlDbClient(_innerConnectionInfo, false);
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