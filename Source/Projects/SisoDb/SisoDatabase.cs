using System;
using EnsureThat;
using PineCone.Structures;
using PineCone.Structures.Schemas;
using SisoDb.Dac;
using SisoDb.DbSchema;
using SisoDb.Providers;

namespace SisoDb
{
    public abstract class SisoDatabase : ISisoDatabase
    {
        private readonly ISisoProviderFactory _providerFactory;
        private readonly ISisoConnectionInfo _connectionInfo;
        protected readonly IDbSchemaManager DbSchemaManager;
        private readonly IStructureSchemas _structureSchemas;
        private readonly IStructureBuilder _structureBuilder;
        private readonly IServerClient _serverClient;

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

        protected SisoDatabase(ISisoConnectionInfo connectionInfo)
        {
            Ensure.That(connectionInfo, "connectionInfo").IsNotNull();

            _connectionInfo = connectionInfo;
            _providerFactory = SisoEnvironment.ProviderFactories.Get(ConnectionInfo.ProviderType);
            
            DbSchemaManager = _providerFactory.GetDbSchemaManager();
            _structureSchemas = SisoEnvironment.Resources.ResolveStructureSchemas();
            _structureBuilder = SisoEnvironment.Resources.ResolveStructureBuilder();
            _providerFactory = SisoEnvironment.ProviderFactories.Get(ConnectionInfo.ProviderType);
            _serverClient = _providerFactory.GetServerClient(ConnectionInfo);
        }

        public virtual void EnsureNewDatabase()
        {
            _serverClient.EnsureNewDb();
        }

        public virtual void CreateIfNotExists()
        {
            _serverClient.CreateDbIfDoesNotExists();
        }

        public virtual void InitializeExisting()
        {
            _serverClient.InitializeExistingDb();
        }

        public virtual void DeleteIfExists()
        {
            _serverClient.DropDbIfItExists();
        }

        public virtual bool Exists()
        {
            return _serverClient.DbExists();
        }

        public virtual void DropStructureSet<T>() where T : class
        {
            DropStructureSet(TypeFor<T>.Type);
        }

        public virtual void DropStructureSet(Type type)
        {
            using (var dbClient = _providerFactory.GetDbClient(_connectionInfo, true))
            {
                var structureSchema = _structureSchemas.GetSchema(type);

                DbSchemaManager.DropStructureSet(structureSchema, dbClient);

                dbClient.Flush();
            }

            _structureSchemas.RemoveSchema(type);
        }

        public virtual void DropStructureSets()
        {
            using (var dbClient = _providerFactory.GetDbClient(_connectionInfo, true))
            {
                foreach (var structureSchema in StructureSchemas.GetSchemas())
                {
                    DbSchemaManager.DropStructureSet(structureSchema, dbClient);
                }

                dbClient.Flush();
            }

            StructureSchemas.Clear();
        }

        public virtual void UpdateStructureSet<TOld, TNew>(Func<TOld, TNew, StructureSetUpdaterStatuses> onProcess)
            where TOld : class
            where TNew : class
        {
            var structureSchemaOld = _structureSchemas.GetSchema(TypeFor<TOld>.Type);
            _structureSchemas.RemoveSchema(TypeFor<TOld>.Type);

            var structureSchemaNew = _structureSchemas.GetSchema(TypeFor<TNew>.Type);

            var updater = new StructureSetUpdater<TOld, TNew>(_connectionInfo, structureSchemaOld, structureSchemaNew, _structureBuilder);
            updater.Process(onProcess);
        }

        public virtual void UpsertStructureSet<T>() where T : class
        {
            UpsertStructureSet(TypeFor<T>.Type);
        }

        public virtual void UpsertStructureSet(Type type)
        {
            using (var dbClient = _providerFactory.GetDbClient(_connectionInfo, true))
            {
                var dbSchemaUpserter = _providerFactory.GetDbSchemaUpserter(dbClient);
                var structureSchema = _structureSchemas.GetSchema(type);
                DbSchemaManager.UpsertStructureSet(structureSchema, dbSchemaUpserter);

                dbClient.Flush();
            }
        }

        public abstract IQueryEngine CreateQueryEngine();

        public abstract IUnitOfWork CreateUnitOfWork();
    }
}