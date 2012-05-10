using EnsureThat;
using SisoDb.Structures;

namespace SisoDb.Configurations
{
    public class DbConfiguration
    {
        public ISisoDatabase Database { get; private set; }

        public DbConfiguration(ISisoDatabase database)
        {
            Ensure.That(database, "database").IsNotNull();
            Database = database;
        }

        public DbConfiguration UseManualStructureIdAssignment()
        {
            Database.StructureBuilders.ForInserts = (schema, getNextIdentity) =>
                StructureBuilders.ForManualStructureIdAssignment();

            return this;
        }
    }
}