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
        private readonly IDbProviderFactory _providerFactory;
        private readonly IDbSchemaManager _dbSchemaManager;
        private IDbSettings _settings;
        private IStructureSchemas _structureSchemas;
        private IStructureBuilders _structureBuilders;
        private ISisoDbSerializer _serializer;

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

        public ISisoDbSerializer Serializer
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
            Settings = ProviderFactory.GetSettings();
            ServerClient = ProviderFactory.GetServerClient(ConnectionInfo);
            Serializer = new ServiceStackJsonSerializer();
            StructureBuilders = new StructureBuilders(() => Serializer);
            StructureSchemas = new StructureSchemas(new StructureTypeFactory(), new AutoSchemaBuilder());
            Maintenance = new SisoDatabaseMaintenance(this);
            _dbSchemaManager = ProviderFactory.GetDbSchemaManagerFor(this);
        }

        public virtual ISisoDatabase EnsureNewDatabase()
        {
            lock (LockObject)
            {
                OnClearCache();
                ServerClient.EnsureNewDb();
            }

            return this;
        }

        public virtual ISisoDatabase CreateIfNotExists()
        {
            lock (LockObject)
            {
                OnClearCache();
                ServerClient.CreateDbIfItDoesNotExist();
            }

            return this;
        }

        public virtual ISisoDatabase InitializeExisting()
        {
            lock (LockObject)
            {
                OnClearCache();
                ServerClient.InitializeExistingDb();
            }

            return this;
        }

        public virtual ISisoDatabase DeleteIfExists()
        {
            lock (LockObject)
            {
                OnClearCache();
                ServerClient.DropDbIfItExists();
            }

            return this;
        }

        public virtual bool Exists()
        {
            lock (LockObject)
            {
                return ServerClient.DbExists();
            }
        }

        public virtual ISisoDatabase DropStructureSet<T>() where T : class
        {
            DropStructureSet(TypeFor<T>.Type);

            return this;
        }

        public virtual ISisoDatabase DropStructureSet(Type type)
        {
            Ensure.That(type, "type").IsNotNull();

            DropStructureSets(new[] { type });

            return this;
        }

        public virtual ISisoDatabase DropStructureSets(Type[] types)
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

            return this;
        }

        public virtual ISisoDatabase UpsertStructureSet<T>() where T : class
        {
            UpsertStructureSet(TypeFor<T>.Type);

            return this;
        }

        public virtual ISisoDatabase UpsertStructureSet(Type type)
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

            return this;
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