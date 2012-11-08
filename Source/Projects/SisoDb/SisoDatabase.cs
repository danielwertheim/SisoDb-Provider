using System;
using System.Diagnostics;
using SisoDb.Dac;
using SisoDb.DbSchema;
using SisoDb.EnsureThat;
using SisoDb.Maintenance;
using SisoDb.Serialization;
using SisoDb.Structures;
using SisoDb.Caching;
using SisoDb.Structures.Schemas;
using SisoDb.Structures.Schemas.Builders;

namespace SisoDb
{
    public abstract class SisoDatabase : ISisoDatabase
    {
        private readonly object _lockObject;
        private readonly ISisoConnectionInfo _connectionInfo;
        private readonly IDbProviderFactory _providerFactory;
        private readonly IDbSchemas _dbSchemas;
        private IDbSettings _settings;
        private IStructureSchemas _structureSchemas;
        private IStructureBuilders _structureBuilders;
        private ISisoSerializer _serializer;

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

        public IDbSchemas DbSchemas
        {
            get { return _dbSchemas; }
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
                Ensure.That(value, "StructureBuilders").IsNotNull();
                _structureBuilders = value;
            }
        }

        public ISisoSerializer Serializer
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
            StructureSchemas = new StructureSchemas(new StructureTypeFactory(), new AutoStructureSchemaBuilder());
            Serializer = null;
            StructureBuilders = new StructureBuilders(() => Serializer, schema => ProviderFactory.GetGuidStructureIdGenerator(), (schema, dbClient) => ProviderFactory.GetIdentityStructureIdGenerator(dbClient));
            Maintenance = new SisoDatabaseMaintenance(this);
            _dbSchemas = ProviderFactory.GetDbSchemaManagerFor(this);
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

        protected virtual void OnClearCache()
        {
            CacheProvider.NotifyOfPurgeAll();
            DbSchemas.ClearCache();
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
            lock (LockObject)
            {
                OnDropStructureSets(new[] { typeof(T) });
            }

            return this;
        }

        public virtual ISisoDatabase DropStructureSet(Type type)
        {
            Ensure.That(type, "type").IsNotNull();

            lock (LockObject)
            {
                OnDropStructureSets(new[] { type });
            }

            return this;
        }

        public virtual ISisoDatabase DropStructureSets(Type[] types)
        {
            Ensure.That(types, "types").HasItems();

            lock (LockObject)
            {
                OnDropStructureSets(types);
            }

            return this;
        }

        protected virtual void OnDropStructureSets(Type[] types)
        {
            using (var dbClient = ProviderFactory.GetTransactionalDbClient(_connectionInfo))
            {
                foreach (var type in types)
                {
                    CacheProvider.NotifyOfPurge(type);

                    var structureSchema = StructureSchemas.GetSchema(type);
                    DbSchemas.Drop(structureSchema, dbClient);
                    StructureSchemas.RemoveSchema(type);
                }
            }
        }

        public virtual ISisoDatabase UpsertStructureSet<T>() where T : class
        {
            lock (LockObject)
            {
                OnUpsertStructureSets(new[] { typeof(T) });
            }

            return this;
        }

        public virtual ISisoDatabase UpsertStructureSet(Type type)
        {
            Ensure.That(type, "type").IsNotNull();

            lock (LockObject)
            {
                OnUpsertStructureSets(new[] { type });
            }

            return this;
        }

        public virtual ISisoDatabase UpsertStructureSet(Type[] types)
        {
            Ensure.That(types, "types").IsNotNull();

            lock (LockObject)
            {
                OnUpsertStructureSets(types);
            }

            return this;
        }

        protected virtual void OnUpsertStructureSets(Type[] types)
        {
            if(!Settings.AllowsAnyDynamicSchemaChanges())
                return;

            using (var dbClient = ProviderFactory.GetTransactionalDbClient(_connectionInfo))
            {
                foreach (var type in types)
                {
                    CacheProvider.NotifyOfPurge(type);

                    var structureSchema = StructureSchemas.GetSchema(type);
                    DbSchemas.Upsert(structureSchema, dbClient);
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
    }
}