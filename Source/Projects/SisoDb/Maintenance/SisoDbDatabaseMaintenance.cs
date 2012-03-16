using EnsureThat;

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
                   dbClient.Reset();
                }
            }
        }

        public void RenameStructure(string @from, string to)
        {
            Ensure.That(@from).IsNotNullOrWhiteSpace();
            Ensure.That(to).IsNotNullOrWhiteSpace();

            lock (_db.DbOperationsLock)
            {
                _db.SchemaManager.RemoveFromCache(@from);

                using (var dbClient = _db.ProviderFactory.GetTransactionalDbClient(_db.ConnectionInfo))
                {
                    dbClient.RenameStructureSet(@from, to);
                }
            }
        }
    }
}