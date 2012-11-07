using System;
using System.Collections.Generic;
using SisoDb.Dac;
using SisoDb.EnsureThat;
using SisoDb.Structures.Schemas;

namespace SisoDb.DbSchema
{
    public class DbSchemas : IDbSchemas
    {
        protected readonly object Lock = new object();
        protected readonly ISisoDatabase Db;
    	protected readonly IDbSchemaUpserter DbSchemaUpserter;
    	protected readonly ISet<string> UpsertedSchemas = new HashSet<string>();
        protected readonly IDictionary<Guid, ISet<string>> UpsertedSchemasByDbClient = new Dictionary<Guid, ISet<string>>(); 
        
        public DbSchemas(ISisoDatabase db, IDbSchemaUpserter dbSchemaUpserter)
        {
            Ensure.That(db, "db").IsNotNull();
        	Ensure.That(dbSchemaUpserter, "dbSchemaUpserter").IsNotNull();

            Db = db;
			DbSchemaUpserter = dbSchemaUpserter;
        }

        public virtual void ClearCache()
        {
            lock (Lock)
            {
                UpsertedSchemas.Clear();
            }
        }

        public virtual void RemoveFromCache(string structureSchemaName)
        {
            lock (Lock)
            {
                UpsertedSchemas.Remove(structureSchemaName);
            }
        }

        public virtual void RemoveFromCache(IStructureSchema structureSchema)
		{
            lock (Lock)
			{
				UpsertedSchemas.Remove(structureSchema.Name);
			}
		}

        public virtual void Drop(IStructureSchema structureSchema, IDbClient dbClient)
        {
            lock (Lock)
            {
                UpsertedSchemas.Remove(structureSchema.Name);

                dbClient.Drop(structureSchema);
            }
        }

        public virtual void Upsert(IStructureSchema structureSchema, IDbClient dbClient)
        {
            if (!Db.Settings.AllowsAnyDynamicSchemaChanges())
                return;

            lock (Lock)
            {
                if (UpsertedSchemas.Contains(structureSchema.Name))
                    return;

                if (dbClient is ITransactionalDbClient)
                    RegisterDbClient((ITransactionalDbClient)dbClient);

                DbSchemaUpserter.Upsert(structureSchema, dbClient, Db.Settings.AllowDynamicSchemaCreation, Db.Settings.AllowDynamicSchemaUpdates);

                UpsertedSchemas.Add(structureSchema.Name);
                UpsertedSchemasByDbClient[dbClient.Id].Add(structureSchema.Name);
            }
        }

        private void RegisterDbClient(ITransactionalDbClient dbClient)
        {
            if(UpsertedSchemasByDbClient.ContainsKey(dbClient.Id)) return;
            
            UpsertedSchemasByDbClient.Add(dbClient.Id, new HashSet<string>());

            dbClient.AfterRollback = () =>
            {
                lock (Lock)
                {
                    var schemasForClient = UpsertedSchemasByDbClient[dbClient.Id];
                    if (schemasForClient == null) return;
                    foreach (var schema in schemasForClient)
                        UpsertedSchemas.Remove(schema);
                }
            };
            dbClient.OnCompleted = () =>
            {
                lock(Lock)
                {
                    UpsertedSchemasByDbClient.Remove(dbClient.Id);
                }
            };
        }
    }
}