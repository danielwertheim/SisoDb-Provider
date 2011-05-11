using System;
using NUnit.Framework;
using SisoDb.Core;
using SisoDb.Providers.SqlProvider;
using SisoDb.Providers.SqlProvider.DbSchema;
using SisoDb.Structures.Schemas;

namespace SisoDb.Tests.IntegrationTests.Providers.SqlProvider.DbSchema
{
    [TestFixture]
    public class SqlDbIndexesSchemaSynchronizerTests : SqlIntegrationTestBase
    {
        private IStructureSchema _structureSchema;
        private string _structureSetPrefix;
        private string _indexesTableName;
        private ISqlDatabase _sqlDb;

        protected override void OnFixtureInitialize()
        {
            DropStructureSet<Class_2579AF20_51A0_475A_A24D_8056828DB1DC>();

            _structureSchema = Database.StructureSchemas.GetSchema(
                TypeFor<Class_2579AF20_51A0_475A_A24D_8056828DB1DC>.Type);
            _structureSetPrefix = typeof(Class_2579AF20_51A0_475A_A24D_8056828DB1DC).Name;
            _indexesTableName = _structureSetPrefix + "Indexes";

            _sqlDb = new SqlDatabase(Database.ConnectionInfo);
        }

        protected override void OnTestFinalize()
        {
            DropStructureSet<Class_2579AF20_51A0_475A_A24D_8056828DB1DC>();
        }

        private class Class_2579AF20_51A0_475A_A24D_8056828DB1DC
        {
            public Guid SisoId { get; set; }

            public string IndexableMember1 { get; set; }

            public int IndexableMember2 { get; set; }
        }

        [Test]
        public void Synhronize_WhenTableIsMissingColumn_ColumnIsAdded()
        {
            CreateStructureSet();
            var hashForColumn = SisoEnvironment.Resources.ResolveMemberNameGenerator().Generate("IndexableMember2");
            DbHelper.DropColumns(_indexesTableName, hashForColumn);

            using (var dbClient = new SqlDbClient(_sqlDb.ConnectionInfo, false))
            {
                var columnGenerator = SisoEnvironment.ProviderFactories.Get(dbClient.ConnectionInfo.ProviderType).GetDbColumnGenerator();
                var synhronizer = new SqlDbIndexesSchemaSynchronizer(dbClient, columnGenerator);
                synhronizer.Synchronize(_structureSchema);
            }

            var columnExists = DbHelper.ColumnsExist(_indexesTableName, hashForColumn);
            Assert.IsTrue(columnExists);
        }

        [Test]
        public void Synhronize_WhenTableHasObsoleteColumn_ColumnIsDropped()
        {
            CreateStructureSet();
            var hashForObsoleteColumn = SisoEnvironment.Resources.ResolveHashService().GenerateHash("ExtraColumn");
            var obsoleteColumnDefinition = string.Format("[{0}] [int] sparse null", hashForObsoleteColumn);
            DbHelper.AddColumns(_indexesTableName, obsoleteColumnDefinition);

            using (var dbClient = new SqlDbClient(_sqlDb.ConnectionInfo, false))
            {
                var columnGenerator = SisoEnvironment.ProviderFactories.Get(dbClient.ConnectionInfo.ProviderType).GetDbColumnGenerator();
                var synhronizer = new SqlDbIndexesSchemaSynchronizer(dbClient, columnGenerator);
                synhronizer.Synchronize(_structureSchema);
            }

            var columnExists = DbHelper.ColumnsExist(_indexesTableName, hashForObsoleteColumn);
            Assert.IsFalse(columnExists);
        }

        private void CreateStructureSet()
        {
            using (var dbClient = new SqlDbClient(_sqlDb.ConnectionInfo, false))
            {
                var upserter = new SqlDbSchemaUpserter(dbClient);
                upserter.Upsert(_structureSchema);
            }
        }
    }
}