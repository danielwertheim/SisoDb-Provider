using SisoDb.Providers.AzureProvider;
using SisoDb.Providers.SqlProvider;
using SisoDb.Resources;
using SisoDb.Structures.Schemas;

namespace SisoDb
{
    public class SisoDatabase : ISisoDatabase
    {
        private readonly ISisoDatabase _innerDatabase;

        public string Name
        {
            get { return _innerDatabase.Name; }
        }

        public ISisoConnectionInfo ServerConnectionInfo
        {
            get { return _innerDatabase.ServerConnectionInfo; }
        }

        public ISisoConnectionInfo ConnectionInfo
        {
            get { return _innerDatabase.ConnectionInfo; }
        }

        public IStructureSchemas StructureSchemas
        {
            get { return _innerDatabase.StructureSchemas; }
        }

        public SisoDatabase(ISisoConnectionInfo connectionInfo)
        {
            switch (connectionInfo.ProviderType)
            {
                case StorageProviders.Sql2008:
                    _innerDatabase = new SqlDatabase(connectionInfo);
                    break;
                case StorageProviders.SqlAzure:
                    _innerDatabase = new AzureDatabase(connectionInfo);
                    break;
                default:
                    throw new SisoDbException(
                        ExceptionMessages.SisoDatabase_UnknownStorageProvider.Inject(connectionInfo.ProviderType));
            }
        }

        public void EnsureNewDatabase()
        {
            _innerDatabase.EnsureNewDatabase();
        }

        public void CreateIfNotExists()
        {
            _innerDatabase.CreateIfNotExists();
        }

        public void InitializeExisting()
        {
            _innerDatabase.InitializeExisting();
        }

        public void DeleteIfExists()
        {
            _innerDatabase.DeleteIfExists();
        }

        public bool Exists()
        {
            return _innerDatabase.Exists();
        }

        public void DropStructureSet<T>() where T : class
        {
            _innerDatabase.DropStructureSet<T>();
        }

        public void UpsertStructureSet<T>() where T : class
        {
            _innerDatabase.UpsertStructureSet<T>();
        }

        public IUnitOfWork CreateUnitOfWork()
        {
            return _innerDatabase.CreateUnitOfWork();
        }
    }
}