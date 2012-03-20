using EnsureThat;

namespace SisoDb.Maintenance
{
    public class SisoDatabaseMaintenance : ISisoDatabaseMaintenance
    {
        private readonly ISisoDatabase _db;

        public SisoDatabaseMaintenance(ISisoDatabase db)
        {
            _db = db;
        }

        public virtual void Clear()
        {
            lock (_db.LockObject)
            {
                _db.SchemaManager.ClearCache();

                using (var dbClient = _db.ProviderFactory.GetTransactionalDbClient(_db.ConnectionInfo))
                {
                   dbClient.Reset();
                }
            }
        }

        public virtual void RenameStructure(string @from, string to)
        {
            Ensure.That(@from).IsNotNullOrWhiteSpace();
            Ensure.That(to).IsNotNullOrWhiteSpace();

            lock (_db.LockObject)
            {
                _db.SchemaManager.RemoveFromCache(@from);

                using (var dbClient = _db.ProviderFactory.GetTransactionalDbClient(_db.ConnectionInfo))
                {
                    dbClient.RenameStructureSet(@from, to);
                }
            }
        }

        public virtual void RegenerateQueryIndexes<T>() where T : class
        {
            throw new System.NotImplementedException();
        }
    }
}