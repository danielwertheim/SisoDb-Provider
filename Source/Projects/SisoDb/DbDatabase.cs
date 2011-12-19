using System;
using System.Diagnostics;
using EnsureThat;
using PineCone.Structures.Schemas;
using PineCone.Structures.Schemas.Builders;
using SisoDb.Dac;
using SisoDb.DbSchema;
using SisoDb.Providers;
using SisoDb.Serialization;
using SisoDb.Structures;

namespace SisoDb
{
    public abstract class DbDatabase : ISisoDatabase
    {
		private readonly ISisoConnectionInfo _connectionInfo;
		private readonly ISisoProviderFactory _providerFactory;
		private IStructureSchemas _structureSchemas;
		private IStructureBuilders _structureBuilders;
		private IJsonSerializer _serializer;

        protected readonly object DbOperationsLock;
        protected readonly IDbSchemaManager DbSchemaManager;
    	protected readonly IServerClient ServerClient;
		
        public string Name
        {
            get { return _connectionInfo.DbName; }
        }

        public ISisoConnectionInfo ConnectionInfo
        {
            get { return _connectionInfo; }
        }

    	public ISisoProviderFactory ProviderFactory
    	{
    		get { return _providerFactory; }
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

    	protected DbDatabase(ISisoConnectionInfo connectionInfo)
        {
            Ensure.That(connectionInfo, "connectionInfo").IsNotNull();

            DbOperationsLock = new object();

            _connectionInfo = connectionInfo;
			_providerFactory = SisoEnvironment.ProviderFactories.Get(ConnectionInfo.ProviderType);
			
			DbSchemaManager = ProviderFactory.GetDbSchemaManager();
			ServerClient = ProviderFactory.GetServerClient(ConnectionInfo);

			StructureBuilders = new StructureBuilders();
            StructureSchemas = new StructureSchemas(new StructureTypeFactory(), new AutoSchemaBuilder());
            Serializer = SisoEnvironment.Resources.ResolveJsonSerializer();
        }

    	public virtual void EnsureNewDatabase()
        {
            lock (DbOperationsLock)
            {
                DbSchemaManager.ClearCache();
                ServerClient.EnsureNewDb();
            }
        }

    	public virtual void CreateIfNotExists()
        {
            lock (DbOperationsLock)
            {
                DbSchemaManager.ClearCache();
                ServerClient.CreateDbIfItDoesNotExist();
            }
        }

    	public virtual void InitializeExisting()
        {
            lock (DbOperationsLock)
            {
                DbSchemaManager.ClearCache();
                ServerClient.InitializeExistingDb();
            }
        }

    	public virtual void DeleteIfExists()
        {
            lock (DbOperationsLock)
            {
                DbSchemaManager.ClearCache();
                ServerClient.DropDbIfItExists();
            }
        }

    	public virtual bool Exists()
        {
            lock (DbOperationsLock)
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
                    var structureSchema = _structureSchemas.GetSchema(type);
                    DbSchemaManager.UpsertStructureSet(structureSchema, dbClient);

                    dbClient.Flush();
                }
            }
        }

    	public abstract IQueryEngine CreateQueryEngine();

    	public abstract IUnitOfWork CreateUnitOfWork();

    	[DebuggerStepThrough]
        public IReadOnce ReadOnce()
        {
            return new ReadOnce(this);
        }

    	[DebuggerStepThrough]
        public IWriteOnce WriteOnce()
        {
            return new WriteOnce(this);
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