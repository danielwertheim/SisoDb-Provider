using System.Collections.Generic;
using SisoDb.Dac;
using SisoDb.EnsureThat;
using SisoDb.Structures.Schemas;

namespace SisoDb.DbSchema
{
    public class DbSchemaManager : IDbSchemaManager
    {
        private readonly ISisoDatabase _db;
    	private readonly IDbSchemaUpserter _dbSchemaUpserter;
    	private readonly ISet<string> _upsertedSchemas;
        
        public DbSchemaManager(ISisoDatabase db, IDbSchemaUpserter dbSchemaUpserter)
        {
            Ensure.That(db, "db").IsNotNull();
        	Ensure.That(dbSchemaUpserter, "dbSchemaUpserter").IsNotNull();

            _db = db;
			_dbSchemaUpserter = dbSchemaUpserter;
            _upsertedSchemas = new HashSet<string>();
        }

        public virtual void ClearCache()
        {
            lock (_upsertedSchemas)
            {
                _upsertedSchemas.Clear();
            }
        }

        public virtual void RemoveFromCache(string structureSchemaName)
        {
            lock (_upsertedSchemas)
            {
                _upsertedSchemas.Remove(structureSchemaName);
            }
        }

        public virtual void RemoveFromCache(IStructureSchema structureSchema)
		{
			lock (_upsertedSchemas)
			{
				_upsertedSchemas.Remove(structureSchema.Name);
			}
		}

        public virtual void DropStructureSet(IStructureSchema structureSchema, IDbClient dbClient)
        {
            lock (_upsertedSchemas)
            {
                _upsertedSchemas.Remove(structureSchema.Name);

                dbClient.Drop(structureSchema);
            }
        }

        public virtual void UpsertStructureSet(IStructureSchema structureSchema, IDbClient dbClient)
        {
            if (!_db.Settings.AllowsAnyDynamicSchemaChanges())
                return;

            lock (_upsertedSchemas)
            {
                if (_upsertedSchemas.Contains(structureSchema.Name))
                    return;

                _dbSchemaUpserter.Upsert(structureSchema, dbClient, _db.Settings.AllowDynamicSchemaCreation, _db.Settings.AllowDynamicSchemaUpdates);

                _upsertedSchemas.Add(structureSchema.Name);
            }
        }
    }
}