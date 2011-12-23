using System.Collections.Generic;
using EnsureThat;
using PineCone.Structures.Schemas;
using SisoDb.Dac;

namespace SisoDb.DbSchema
{
    public class DbSchemaManager : IDbSchemaManager
    {
    	private readonly IDbSchemaUpserter _dbSchemaUpserter;
    	private readonly ISet<string> _upsertedSchemas;

        public DbSchemaManager(IDbSchemaUpserter dbSchemaUpserter)
        {
        	Ensure.That(dbSchemaUpserter, "dbSchemaUpserter").IsNotNull();

			_dbSchemaUpserter = dbSchemaUpserter;
            _upsertedSchemas = new HashSet<string>();
        }

        public void ClearCache()
        {
            lock (_upsertedSchemas)
            {
                _upsertedSchemas.Clear();
            }
        }

		public void RemoveFromCache(IStructureSchema structureSchema)
		{
			lock (_upsertedSchemas)
			{
				_upsertedSchemas.Remove(structureSchema.Name);
			}
		}

        public void DropStructureSet(IStructureSchema structureSchema, IDbClient dbClient)
        {
            lock (_upsertedSchemas)
            {
                _upsertedSchemas.Remove(structureSchema.Name);

                dbClient.Drop(structureSchema);
            }
        }

        public void UpsertStructureSet(IStructureSchema structureSchema, IDbClient dbClient)
        {
            lock (_upsertedSchemas)
            {
                if (_upsertedSchemas.Contains(structureSchema.Name))
                    return;

                _dbSchemaUpserter.Upsert(structureSchema, dbClient);

                _upsertedSchemas.Add(structureSchema.Name);
            }
        }
    }
}