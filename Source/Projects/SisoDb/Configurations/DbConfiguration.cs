using EnsureThat;
using PineCone.Structures.IdGenerators;

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
            Database.StructureBuilders.StructureIdGeneratorFn = (schema) => new EmptyStructureIdGenerator();

            return this;
        }
    }
}