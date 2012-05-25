using System;
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
            UpsertStructureSet(structureSchema, () => dbClient);
        }

        public virtual void UpsertStructureSet(IStructureSchema structureSchema, Func<IDbClient> dbClientFn)
        {
            lock (_upsertedSchemas)
            {
                if (_upsertedSchemas.Contains(structureSchema.Name))
                    return;

                _dbSchemaUpserter.Upsert(structureSchema, dbClientFn);

                _upsertedSchemas.Add(structureSchema.Name);
            }
        }
    }
}