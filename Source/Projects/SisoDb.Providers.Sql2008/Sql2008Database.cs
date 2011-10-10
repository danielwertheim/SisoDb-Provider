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

        public IStructureSchemas StructureSchemas
        {
            get { return _structureSchemas; }
        }

        public IStructureBuilder StructureBuilder
        {
            get { return _structureBuilder; }
        }

        protected internal Sql2008Database(ISisoConnectionInfo connectionInfo)
        {
            Ensure.That(() => connectionInfo).IsNotNull();

            _connectionInfo = connectionInfo;
            _providerFactory = SisoEnvironment.ProviderFactories.Get(_connectionInfo.ProviderType);
            
            _dbSchemaManager = _providerFactory.GetDbSchemaManager();
            _structureSchemas = SisoEnvironment.Resources.ResolveStructureSchemas();
            _structureBuilder = SisoEnvironment.Resources.ResolveStructureBuilder();
            _providerFactory = SisoEnvironment.ProviderFactories.Get(connectionInfo.ProviderType);
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

            var updater = new Sql2008StructureSetUpdater<TOld, TNew>(_connectionInfo, structureSchemaOld, structureSchemaNew, _structureBuilder);
            updater.Process(onProcess);
        }

        public IQueryEngine CreateQueryEngine()
        {
            var jsonSerializer = SisoEnvironment.Resources.ResolveJsonSerializer();

            return new Sql2008QueryEngine(
                _connectionInfo,
                _dbSchemaManager,
                _structureSchemas,
                jsonSerializer);
        }

        public IUnitOfWork CreateUnitOfWork()
        {
            var jsonSerializer = SisoEnvironment.Resources.ResolveJsonSerializer();

            return new Sql2008UnitOfWork(
                _connectionInfo,
                _dbSchemaManager,
                _structureSchemas,
                jsonSerializer,
                _structureBuilder);
        }
    }
}