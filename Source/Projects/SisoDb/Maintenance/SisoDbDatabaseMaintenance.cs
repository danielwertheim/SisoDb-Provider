namespace SisoDb.Maintenance
{
    public class SisoDbDatabaseMaintenance : ISisoDatabaseMaintenance
    {
        private readonly ISisoDbDatabase _db;

        public SisoDbDatabaseMaintenance(ISisoDbDatabase db)
        {
            _db = db;
        }

        public void Clear()
        {
            lock (_db.DbOperationsLock)
            {
                _db.SchemaManager.ClearCache();

                using (var dbClient = _db.ProviderFactory.GetTransactionalDbClient(_db.ConnectionInfo))
                {
                   dbClient.DropAllStructureSets();
                }
            }
        }
    }
}