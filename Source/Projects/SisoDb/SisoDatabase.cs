using System;
using System.Diagnostics;
using EnsureThat;
using PineCone.Structures.Schemas;
using PineCone.Structures.Schemas.Builders;
using SisoDb.Dac;
using SisoDb.DbSchema;
using SisoDb.Maintenance;
using SisoDb.Serialization;
using SisoDb.Structures;
using SisoDb.Caching;

namespace SisoDb
{
    public abstract class SisoDatabase : ISisoDatabase
    {
        private readonly object _lockObject;
        private readonly ISisoConnectionInfo _connectionInfo;
        private IDbSettings _settings;
        private readonly IDbProviderFactory _providerFactory;
        private readonly IDbSchemaManager _dbSchemaManager;
        private IStructureSchemas _structureSchemas;
        private IStructureBuilders _structureBuilders;
        private IJsonSerializer _serializer;

        protected readonly IServerClient ServerClient;
        
        public object LockObject
        {
            get { return _lockObject; }
        }

        public string Name
        {
            get { return _connectionInfo.DbName; }
        }

        public ISisoConnectionInfo ConnectionInfo
        {
            get { return _connectionInfo; }
        }

        public IDbSettings Settings
        {
            get { return _settings; }
            set 
            {
                Ensure.That(value, "Settings").IsNotNull();
                _settings = value;
            }
        }

        public ICacheProvider CacheProvider { get; set; }
        public IDbProviderFactory ProviderFactory
        {
            get { return _providerFactory; }
        }

        public IDbSchemaManager SchemaManager
        {
            get { return _dbSchemaManager; }
        }
        public bool CachingIsEnabled
        {
            get { return CacheProvider != null; }
        }

        public IStructureSchemas StructureSchemas
        {
            get { return _structureSchemas; }
            set
            {
                Ensure.That(value, "StructureSchemas").IsNotNull();

                _structureSchemas = value;
            }
        }

        public IStructureBuilders StructureBuilders
        {
            get { return _structureBuilders; }
            set
            {
                Ensure.That(value, "StructureBuilderss").IsNotNull();
                _structureBuilders = value;
            }
        }

        public IJsonSerializer Serializer
        {
            get { return _serializer; }
            set
            {
                Ensure.That(value, "Serializer").IsNotNull();

                _serializer = value;
            }
        }

        public ISisoDatabaseMaintenance Maintenance { get; private set; }

        protected SisoDatabase(ISisoConnectionInfo connectionInfo, IDbProviderFactory dbProviderFactory)
        {
            Ensure.That(connectionInfo, "connectionInfo").IsNotNull();
            Ensure.That(dbProviderFactory, "dbProviderFactory").IsNotNull();

            _lockObject = new object();
            _connectionInfo = connectionInfo;
            _providerFactory = dbProviderFactory;
            _dbSchemaManager = ProviderFactory.GetDbSchemaManagerFor(this);
            Settings = ProviderFactory.GetSettings();
            ServerClient = ProviderFactory.GetServerClient(ConnectionInfo);
            StructureBuilders = new StructureBuilders();
            StructureSchemas = new StructureSchemas(new StructureTypeFactory(), new AutoSchemaBuilder());
            Serializer = SisoEnvironment.Resources.ResolveJsonSerializer();
            Maintenance = new SisoDatabaseMaintenance(this);
        }

        public virtual void EnsureNewDatabase()
        {
            lock (LockObject)
            {
                OnClearCache();
                ServerClient.EnsureNewDb();
            }
        }

        public virtual void CreateIfNotExists()
        {
            lock (LockObject)
            {
                OnClearCache();
                ServerClient.CreateDbIfItDoesNotExist();
            }
        }

        public virtual void InitializeExisting()
        {
            lock (LockObject)
            {
                OnClearCache();
                ServerClient.InitializeExistingDb();
            }
        }

        public virtual void DeleteIfExists()
        {
            lock (LockObject)
            {
                OnClearCache();
                ServerClient.DropDbIfItExists();
            }
        }

        public virtual bool Exists()
        {
            lock (LockObject)
            {
                return ServerClient.DbExists();
            }
        }

        public virtual void DropStructureSet<T>() where T : class
        {
            DropStructureSet(TypeFor<T>.Type);
        }

        public virtual void DropStructureSet(Type type)
        {
            Ensure.That(type, "type").IsNotNull();

            DropStructureSets(new[] { type });
        }

        public virtual void DropStructureSets(Type[] types)
        {
            Ensure.That(types, "types").HasItems();

            lock (LockObject)
            {
                using (var dbClient = ProviderFactory.GetTransactionalDbClient(_connectionInfo))
                {
                    foreach (var type in types)
                    {
                        CacheProvider.NotifyOfPurge(type);

                        var structureSchema = _structureSchemas.GetSchema(type);

                        SchemaManager.DropStructureSet(structureSchema, dbClient);

                        _structureSchemas.RemoveSchema(type);
                    }
                }
            }
        }

        public virtual void UpsertStructureSet<T>() where T : class
        {
            UpsertStructureSet(TypeFor<T>.Type);
        }

        public virtual void UpsertStructureSet(Type type)
        {
            Ensure.That(type, "type").IsNotNull();

            lock (LockObject)
            {
                CacheProvider.NotifyOfPurge(type);

                var structureSchema = _structureSchemas.GetSchema(type);

                using (var dbClient = ProviderFactory.GetTransactionalDbClient(_connectionInfo))
                {
                    SchemaManager.UpsertStructureSet(structureSchema, dbClient);
                }
            }
        }

        public virtual ISession BeginSession()
        {
            return CreateSession();
        }

        protected abstract DbSession CreateSession();

        [DebuggerStepThrough]
        public ISingleOperationSession UseOnceTo()
        {
            return new SingleOperationSession(this);
        }

        protected virtual void OnClearCache()
        {
            CacheProvider.NotifyOfPurgeAll();
            SchemaManager.ClearCache();
        }
    }
}