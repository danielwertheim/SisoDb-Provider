using System;
using System.Data.SqlServerCe;
using EnsureThat;
using NCore;
using PineCone.Structures;
using PineCone.Structures.Schemas;
using SisoDb.Core.Io;
using SisoDb.DbSchema;
using SisoDb.Providers;
using SisoDb.Resources;

namespace SisoDb.SqlCe4
{
    public class SqlCe4Database : ISisoDatabase
    {
        private readonly ISisoProviderFactory _providerFactory;
        private readonly SqlCe4ConnectionInfo _connectionInfo;
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

        protected internal SqlCe4Database(SqlCe4ConnectionInfo connectionInfo)
        {
            Ensure.That(connectionInfo, "connectionInfo").IsNotNull();

            _connectionInfo = connectionInfo;
            _providerFactory = SisoEnvironment.ProviderFactories.Get(_connectionInfo.ProviderType);

            _dbSchemaManager = _providerFactory.GetDbSchemaManager();
            _structureSchemas = SisoEnvironment.Resources.ResolveStructureSchemas();
            _structureBuilder = SisoEnvironment.Resources.ResolveStructureBuilder();
            _providerFactory = SisoEnvironment.ProviderFactories.Get(connectionInfo.ProviderType);
        }

        public void EnsureNewDatabase()
        {
            IoHelper.DeleteIfFileExists(_connectionInfo.FilePath);

            using (var engine = new SqlCeEngine(ConnectionInfo.ConnectionString.PlainString))
            {
                engine.CreateDatabase();
            }

            InitializeExisting();
        }

        //public virtual void EnsureNewDatabase()
        //{
        //    using (var serverClient = _providerFactory.GetServerClient(_connectionInfo))
        //    {
        //        serverClient.DropDatabaseIfExists(Name);

        //        serverClient.CreateDatabase(Name);
        //    }
        //}

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
            DropStructureSet(TypeFor<T>.Type);
        }

        public void DropStructureSet(Type type)
        {
            using (var dbClient = _providerFactory.GetDbClient(_connectionInfo, true))
            {
                var structureSchema = _structureSchemas.GetSchema(type);

                _dbSchemaManager.DropStructureSet(structureSchema, dbClient);

                dbClient.Flush();
            }

            _structureSchemas.RemoveSchema(type);
        }

        public void DropStructureSets()
        {
            using (var dbClient = _providerFactory.GetDbClient(_connectionInfo, true))
            {
                foreach (var structureSchema in StructureSchemas.GetSchemas())
                {
                    _dbSchemaManager.DropStructureSet(structureSchema, dbClient);
                }

                dbClient.Flush();
            }

            StructureSchemas.Clear();
        }

        public void UpdateStructureSet<TOld, TNew>(Func<TOld, TNew, StructureSetUpdaterStatuses> onProcess)
            where TOld : class
            where TNew : class
        {
            var structureSchemaOld = _structureSchemas.GetSchema(TypeFor<TOld>.Type);
            _structureSchemas.RemoveSchema(TypeFor<TOld>.Type);

            var structureSchemaNew = _structureSchemas.GetSchema(TypeFor<TNew>.Type);

            var updater = new StructureSetUpdater<TOld, TNew>(_connectionInfo, structureSchemaOld, structureSchemaNew, _structureBuilder);
            updater.Process(onProcess);
        }

        public void UpsertStructureSet<T>() where T : class
        {
            UpsertStructureSet(TypeFor<T>.Type);
        }

        public void UpsertStructureSet(Type type)
        {
            using (var dbClient = _providerFactory.GetDbClient(_connectionInfo, true))
            {
                var dbSchemaUpserter = _providerFactory.GetDbSchemaUpserter(dbClient);
                var structureSchema = _structureSchemas.GetSchema(type);
                _dbSchemaManager.UpsertStructureSet(structureSchema, dbSchemaUpserter);

                dbClient.Flush();
            }
        }

        public IQueryEngine CreateQueryEngine()
        {
            throw new NotImplementedException();
            //var jsonSerializer = SisoEnvironment.Resources.ResolveJsonSerializer();

            //return new Sql2008QueryEngine(
            //    _connectionInfo,
            //    _dbSchemaManager,
            //    _structureSchemas,
            //    jsonSerializer);
        }

        public IUnitOfWork CreateUnitOfWork()
        {
            throw new NotImplementedException();
            //var jsonSerializer = SisoEnvironment.Resources.ResolveJsonSerializer();

            //return new Sql2008UnitOfWork(
            //    _connectionInfo,
            //    _dbSchemaManager,
            //    _structureSchemas,
            //    jsonSerializer,
            //    _structureBuilder);
        }
    }
}