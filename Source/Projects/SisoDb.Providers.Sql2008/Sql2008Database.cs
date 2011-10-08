using System;
using EnsureThat;
using NCore;
using PineCone;
using PineCone.Structures;
using PineCone.Structures.Schemas;
using SisoDb.DbSchema;
using SisoDb.Querying;
using SisoDb.Querying.Lambdas.Processors.Sql;
using SisoDb.Resources;
using SisoDb.Sql2008.Dac;
using SisoDb.Sql2008.DbSchema;

namespace SisoDb.Sql2008
{
    public class Sql2008Database : ISisoDatabase
    {
        private readonly SqlConnectionInfo _connectionInfo;
        private readonly IDbSchemaManager _dbSchemaManager;
        private readonly IStructureSchemas _structureSchemas;

        //var dbClientNonTrans = new Sql2008DbClient(_connectionInfo, false);lkhgfhgfhgf
        //var identityGenerator = new SqlIdentityGenerator(dbClientNonTrans);

        public string Name
        {
            get { return _connectionInfo.Name; }
        }
        
        public ISisoConnectionInfo ConnectionInfo
        {
            get { return _connectionInfo; }
        }

        protected internal Sql2008Database(ISisoConnectionInfo connectionInfo)
        {
            Ensure.That(() => connectionInfo).IsNotNull();

            _connectionInfo = new SqlConnectionInfo(connectionInfo);
            _dbSchemaManager = SisoEnvironment.Resources.ResolveDbSchemaManager();
            _structureSchemas = SisoEnvironment.Resources.ResolveStructureSchemas();
        }

        public virtual void EnsureNewDatabase()
        {
            using (var client = new Sql2008ServerClient(_connectionInfo))
            {
                client.DropDatabaseIfExists(Name);

                client.CreateDatabase(Name);
            }
        }

        public virtual void CreateIfNotExists()
        {
            using (var client = new Sql2008ServerClient(_connectionInfo))
            {
                if (!client.DatabaseExists(Name))
                    client.CreateDatabase(Name);
            }
        }

        public virtual void InitializeExisting()
        {
            using (var client = new Sql2008ServerClient(_connectionInfo))
            {
                if (!client.DatabaseExists(Name))
                    throw new SisoDbException(ExceptionMessages.SqlDatabase_InitializeExisting_DbDoesNotExist.Inject(Name));

                client.InitializeExistingDb(Name);
            }
        }

        public virtual void DeleteIfExists()
        {
            using (var client = new Sql2008ServerClient(_connectionInfo))
            {
                client.DropDatabaseIfExists(Name);
            }
        }

        public virtual bool Exists()
        {
            using (var client = new Sql2008ServerClient(_connectionInfo))
            {
                return client.DatabaseExists(Name);
            }
        }

        public void DropStructureSet<T>() where T : class
        {
            using (var client = new Sql2008DbClient(_connectionInfo, true))
            {
                var structureSchema = _structureSchemas.GetSchema(TypeFor<T>.Type);
                
                _dbSchemaManager.DropStructureSet(structureSchema, client);

                _structureSchemas.RemoveSchema(TypeFor<T>.Type);

                client.Flush();
            }
        }

        public void UpsertStructureSet<T>() where T : class
        {
            using (var client = new Sql2008DbClient(_connectionInfo, true))
            {
                var upserter = new SqlDbSchemaUpserter(client);
                var structureSchema = _structureSchemas.GetSchema(TypeFor<T>.Type);
                _dbSchemaManager.UpsertStructureSet(structureSchema, upserter);

                client.Flush();
            }
        }

        public void UpdateStructureSet<TOld, TNew>(Func<TOld, TNew, StructureSetUpdaterStatuses> onProcess)
            where TOld : class 
            where TNew : class
        {
            var structureSchemaOld = _structureSchemas.GetSchema(TypeFor<TOld>.Type);

            _structureSchemas.RemoveSchema(TypeFor<TOld>.Type);

            var structureSchemaNew = _structureSchemas.GetSchema(TypeFor<TNew>.Type);
            
            var updater = new Sql2008StructureSetUpdater<TOld, TNew>(_connectionInfo, structureSchemaOld, structureSchemaNew, _pineConizer.Builder);
            
            updater.Process(onProcess);
        }

        public IQueryEngine CreateQueryEngine()
        {
            var dbClient = new Sql2008DbClient(_connectionInfo, false);
            var memberNameGenerator = SisoEnvironment.Resources.ResolveMemberNameGenerator();

            var queryGenerator = new SqlQueryGenerator(
                new ParsedWhereSqlProcessor(memberNameGenerator),
                new ParsedSortingSqlProcessor(memberNameGenerator),
                new ParsedIncludeSqlProcessor(memberNameGenerator));
            
            return new Sql2008QueryEngine(
                dbClient,
                _dbSchemaManager,
                new SqlDbSchemaUpserter(dbClient),
                _pineConizer,
                SisoEnvironment.Resources.ResolveJsonSerializer(),
                queryGenerator,
                new CommandBuilderFactory());
        }

        public IUnitOfWork CreateUnitOfWork()
        {
            var dbClient = new Sql2008DbClient(_connectionInfo, true);
            var dbClientNonTrans = new Sql2008DbClient(_connectionInfo, false);
            var memberNameGenerator = SisoEnvironment.Resources.ResolveMemberNameGenerator();

            var queryGenerator = new SqlQueryGenerator(
                new ParsedWhereSqlProcessor(memberNameGenerator),
                new ParsedSortingSqlProcessor(memberNameGenerator),
                new ParsedIncludeSqlProcessor(memberNameGenerator));

            var commandBuilderFactory = new CommandBuilderFactory();
            var dbSchemaUpserter = new SqlDbSchemaUpserter(dbClientNonTrans);

            return new Sql2008UnitOfWork(
                dbClient,  
                _dbSchemaManager, dbSchemaUpserter,
                _pineConizer, 
                SisoEnvironment.Resources.ResolveJsonSerializer(),
                queryGenerator,
                commandBuilderFactory);
        }
    }
}