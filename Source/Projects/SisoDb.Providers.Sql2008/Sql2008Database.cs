using System;
using EnsureThat;
using NCore;
using PineCone.Structures;
using PineCone.Structures.Schemas;
using SisoDb.DbSchema;
using SisoDb.Providers;
using SisoDb.Resources;

namespace SisoDb.Sql2008
{
    public class Sql2008Database : ISisoDatabase
    {
        private readonly ISisoProviderFactory _providerFactory;
        private readonly ISisoConnectionInfo _connectionInfo;
        private readonly IDbSchemaManager _dbSchemaManager;
        private readonly IStructureSchemas _structureSchemas;
        private readonly IStructureBuilder _structureBuilder;

        public string Name
        {
            get { return _connectionInfo.DbName; }
        }

        public ISisoConnectionInfo ConnectionInfo
        {
            get { return _connectionInfo; }
        }

        protected internal Sql2008Database(ISisoConnectionInfo connectionInfo, ISisoProviderFactory providerFactory)
        {
            Ensure.That(() => connectionInfo).IsNotNull();
            Ensure.That(() => providerFactory).IsNotNull();

            _connectionInfo = connectionInfo;
            _providerFactory = providerFactory;

            _dbSchemaManager = _providerFactory.GetDbSchemaManager();
            _structureSchemas = SisoEnvironment.Resources.ResolveStructureSchemas();
            _structureBuilder = SisoEnvironment.Resources.ResolveStructureBuilder();
        }

        public virtual void EnsureNewDatabase()
        {
            using (var serverClient = _providerFactory.GetServerClient(_connectionInfo))
            {
                serverClient.DropDatabaseIfExists(Name);

                serverClient.CreateDatabase(Name);
            }
        }

        public virtual void CreateIfNotExists()
        {
            using (var serverClient = _providerFactory.GetServerClient(_connectionInfo))
            {
                if (!serverClient.DatabaseExists(Name))
                    serverClient.CreateDatabase(Name);
            }
        }

        public virtual void InitializeExisting()
        {
            using (var serverClient = _providerFactory.GetServerClient(_connectionInfo))
            {
                if (!serverClient.DatabaseExists(Name))
                    throw new SisoDbException(ExceptionMessages.SqlDatabase_InitializeExisting_DbDoesNotExist.Inject(Name));

                serverClient.InitializeExistingDb(Name);
            }
        }

        public virtual void DeleteIfExists()
        {
            using (var serverClient = _providerFactory.GetServerClient(_connectionInfo))
            {
                serverClient.DropDatabaseIfExists(Name);
            }
        }

        public virtual bool Exists()
        {
            using (var serverClient = _providerFactory.GetServerClient(_connectionInfo))
            {
                return serverClient.DatabaseExists(Name);
            }
        }

        public void DropStructureSet<T>() where T : class
        {
            using (var dbClient = _providerFactory.GetDbClient(_connectionInfo, true))
            {
                var structureSchema = _structureSchemas.GetSchema(TypeFor<T>.Type);

                _dbSchemaManager.DropStructureSet(structureSchema, dbClient);

                _structureSchemas.RemoveSchema(TypeFor<T>.Type);

                dbClient.Flush();
            }
        }

        public void UpsertStructureSet<T>() where T : class
        {
            using (var dbClient = _providerFactory.GetDbClient(_connectionInfo, true))
            {
                var dbSchemaUpserter = _providerFactory.GetDbSchemaUpserter(dbClient);
                var structureSchema = _structureSchemas.GetSchema(TypeFor<T>.Type);
                _dbSchemaManager.UpsertStructureSet(structureSchema, dbSchemaUpserter);

                dbClient.Flush();
            }
        }

        public void UpdateStructureSet<TOld, TNew>(Func<TOld, TNew, StructureSetUpdaterStatuses> onProcess)
            where TOld : class
            where TNew : class
        {
            var structureSchemaOld = _structureSchemas.GetSchema(TypeFor<TOld>.Type);
            _structureSchemas.RemoveSchema(TypeFor<TOld>.Type);

            var structureSchemaNew = _structureSchemas.GetSchema(TypeFor<TNew>.Type);

            using (var identityIdGenerator = _providerFactory.GetStructureIdGeneratorForIdentities(ConnectionInfo))
            {
                var structureBuilder = SisoEnvironment.Resources.ResolveStructureBuilder();
                var updater = new Sql2008StructureSetUpdater<TOld, TNew>(_connectionInfo, structureSchemaOld, structureSchemaNew, structureBuilder);
                updater.Process(onProcess);
            }
        }

        public IQueryEngine CreateQueryEngine()
        {
            var dbClient = _providerFactory.GetDbClient(_connectionInfo, false);
            var dbSchemaUpserter = _providerFactory.GetDbSchemaUpserter(dbClient);
            var queryGenerator = _providerFactory.GetDbQueryGenerator();
            var commandBuilderFactory = _providerFactory.GetCommandBuilderFactory();
            var jsonSerializer = SisoEnvironment.Resources.ResolveJsonSerializer();

            return new Sql2008QueryEngine(
                _providerFactory,
                dbClient,
                _dbSchemaManager,
                dbSchemaUpserter,
                _structureSchemas,
                jsonSerializer,
                queryGenerator,
                commandBuilderFactory);
        }

        public IUnitOfWork CreateUnitOfWork()
        {
            var dbClient = _providerFactory.GetDbClient(_connectionInfo, true);
            var dbClientNonTrans = _providerFactory.GetDbClient(_connectionInfo, false);
            var dbSchemaUpserter = _providerFactory.GetDbSchemaUpserter(dbClientNonTrans);
            var queryGenerator = _providerFactory.GetDbQueryGenerator();
            var commandBuilderFactory = _providerFactory.GetCommandBuilderFactory();
            var jsonSerializer = SisoEnvironment.Resources.ResolveJsonSerializer();
            
            return new Sql2008UnitOfWork(
                _providerFactory,
                dbClient,
                _dbSchemaManager,
                dbSchemaUpserter,
                _structureSchemas,
                _structureBuilder,
                jsonSerializer,
                queryGenerator,
                commandBuilderFactory);
        }
    }
}