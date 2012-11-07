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
        protected readonly ISet<string> TransientSchemas = new HashSet<string>();
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
                TransientSchemas.Clear();
            }
        }

        public virtual void RemoveFromCache(string structureSchemaName)
        {
            lock (Lock)
            {
                UpsertedSchemas.Remove(structureSchemaName);
                TransientSchemas.Remove(structureSchemaName);
            }
        }

        public virtual void RemoveFromCache(IStructureSchema structureSchema)
        {
            lock (Lock)
            {
                UpsertedSchemas.Remove(structureSchema.Name);
                TransientSchemas.Remove(structureSchema.Name);
            }
        }

        public virtual void Drop(IStructureSchema structureSchema, IDbClient dbClient)
        {
            lock (Lock)
            {
                UpsertedSchemas.Remove(structureSchema.Name);
                TransientSchemas.Remove(structureSchema.Name);
                dbClient.Drop(structureSchema);
            }
        }

        public virtual void Upsert(IStructureSchema structureSchema, IDbClient dbClient)
        {
            if (!Db.Settings.AllowsAnyDynamicSchemaChanges())
                return;

            if (SchemaIsAllreadyUpserted(structureSchema, dbClient)) 
                return;

            lock (Lock)
            {
                RegisterDbClient(dbClient);
                DbSchemaUpserter.Upsert(structureSchema, dbClient, Db.Settings.AllowDynamicSchemaCreation, Db.Settings.AllowDynamicSchemaUpdates);
                UpsertedSchemasByDbClient[dbClient.Id].Add(structureSchema.Name);
                TransientSchemas.Add(structureSchema.Name);
            }
        }

        private bool SchemaIsAllreadyUpserted(IStructureSchema structureSchema, IDbClient dbClient)
        {
            if (UpsertedSchemas.Contains(structureSchema.Name) || TransientSchemas.Contains(structureSchema.Name))
                return true;

            return (UpsertedSchemasByDbClient.ContainsKey(dbClient.Id) && UpsertedSchemasByDbClient[dbClient.Id].Contains(structureSchema.Name));
        }

        protected virtual void RegisterDbClient(IDbClient dbClient)
        {
            if (UpsertedSchemasByDbClient.ContainsKey(dbClient.Id))
                return;

            dbClient.AfterCommit = () => CleanupAfterDbClient(dbClient, s =>
            {
                UpsertedSchemas.Add(s);
                TransientSchemas.Remove(s);
            });
            dbClient.OnCompleted = () => CleanupAfterDbClient(dbClient, s => TransientSchemas.Remove(s));
            UpsertedSchemasByDbClient.Add(dbClient.Id, new HashSet<string>());
        }

        protected virtual void CleanupAfterDbClient(IDbClient dbClient, Action<string> schemaCallback)
        {
            lock (Lock)
            {
                if (!UpsertedSchemasByDbClient.ContainsKey(dbClient.Id))
                    return;

                var schemasForClient = UpsertedSchemasByDbClient[dbClient.Id];
                foreach (var schema in schemasForClient)
                    schemaCallback(schema);

                UpsertedSchemasByDbClient.Remove(dbClient.Id);   
            }
        }
    }
}