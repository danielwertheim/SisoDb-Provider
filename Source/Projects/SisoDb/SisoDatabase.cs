using System;
using System.Diagnostics;
using EnsureThat;
using PineCone.Structures.Schemas;
using SisoDb.Dac;
using SisoDb.DbSchema;
using SisoDb.Providers;
using SisoDb.Serialization;
using SisoDb.Structures;

namespace SisoDb
{
    public abstract class SisoDatabase : ISisoDatabase
    {
        protected readonly object DbOperationsLock;

        protected readonly ISisoProviderFactory ProviderFactory;
        protected readonly IDbSchemaManager DbSchemaManager;

        private readonly ISisoConnectionInfo _connectionInfo;
        private IStructureSchemas _structureSchemas;
        private IStructureBuilders _structureBuilders;
        private IJsonSerializer _serializer;
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

        protected SisoDatabase(ISisoConnectionInfo connectionInfo)
        {
            Ensure.That(connectionInfo, "connectionInfo").IsNotNull();

            DbOperationsLock = new object();

            _connectionInfo = connectionInfo;
            ProviderFactory = SisoEnvironment.ProviderFactories.Get(ConnectionInfo.ProviderType);
            StructureSchemas = SisoEnvironment.Resources.ResolveStructureSchemas();
            StructureBuilders = SisoEnvironment.Resources.ResolveStructureBuilders();
            Serializer = SisoEnvironment.Resources.ResolveJsonSerializer();

            DbSchemaManager = ProviderFactory.GetDbSchemaManager();
            _serverClient = ProviderFactory.GetServerClient(ConnectionInfo);
        }

        public virtual void EnsureNewDatabase()
        {
            lock (DbOperationsLock)
            {
                DbSchemaManager.ClearCache();
                _serverClient.EnsureNewDb();
            }
        }

        public virtual void CreateIfNotExists()
        {
            lock (DbOperationsLock)
            {
                DbSchemaManager.ClearCache();
                _serverClient.CreateDbIfItDoesNotExist();
            }
        }

        public virtual void InitializeExisting()
        {
            lock (DbOperationsLock)
            {
                DbSchemaManager.ClearCache();
                _serverClient.InitializeExistingDb();
            }
        }

        public virtual void DeleteIfExists()
        {
            lock (DbOperationsLock)
            {
                DbSchemaManager.ClearCache();
                _serverClient.DropDbIfItExists();
            }
        }

        public virtual bool Exists()
        {
            lock (DbOperationsLock)
            {
                return _serverClient.DbExists();
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

            lock (DbOperationsLock)
            {
                using (var dbClient = ProviderFactory.GetTransactionalDbClient(_connectionInfo))
                {
                    foreach (var type in types)
                    {
                        var structureSchema = _structureSchemas.GetSchema(type);

                        DbSchemaManager.DropStructureSet(structureSchema, dbClient);

                        dbClient.Flush();

                        _structureSchemas.RemoveSchema(type);
                    }
                }
            }
        }

        public virtual void UpdateStructureSet<TOld, TNew>(Func<TOld, TNew, StructureSetUpdaterStatuses> onProcess)
            where TOld : class
            where TNew : class
        {
            lock (DbOperationsLock)
            {
                var structureSchemaOld = _structureSchemas.GetSchema(TypeFor<TOld>.Type);
                _structureSchemas.RemoveSchema(TypeFor<TOld>.Type);

                var structureSchemaNew = _structureSchemas.GetSchema(TypeFor<TNew>.Type);

                var updater = new StructureSetUpdater<TOld, TNew>(_connectionInfo, structureSchemaOld,
                                                                  structureSchemaNew, StructureBuilders);
                updater.Process(onProcess);
            }
        }

        public virtual void UpsertStructureSet<T>() where T : class
        {
            UpsertStructureSet(TypeFor<T>.Type);
        }

        public virtual void UpsertStructureSet(Type type)
        {
            Ensure.That(type, "type").IsNotNull();

            lock (DbOperationsLock)
            {
                using (var dbClient = ProviderFactory.GetTransactionalDbClient(_connectionInfo))
                {
                    var dbSchemaUpserter = ProviderFactory.GetDbSchemaUpserter(dbClient);
                    var structureSchema = _structureSchemas.GetSchema(type);
                    DbSchemaManager.UpsertStructureSet(structureSchema, dbSchemaUpserter);

                    dbClient.Flush();
                }
            }
        }

        public abstract IQueryEngine CreateQueryEngine();

        public abstract IUnitOfWork CreateUnitOfWork();

        [DebuggerStepThrough]
        public DbQueryExtensionPoint ReadOnce()
        {
            return new DbQueryExtensionPoint(this);
        }

        [DebuggerStepThrough]
        public DbUnitOfWorkExtensionPoint WriteOnce()
        {
            return new DbUnitOfWorkExtensionPoint(this);
        }

        [DebuggerStepThrough]
        public void WithUnitOfWork(Action<IUnitOfWork> consumer)
        {
            using (var uow = CreateUnitOfWork())
            {
                consumer.Invoke(uow);
            }
        }

        [DebuggerStepThrough]
        public void WithQueryEngine(Action<IQueryEngine> consumer)
        {
            using (var qe = CreateQueryEngine())
            {
                consumer.Invoke(qe);
            }
        }
    }
}