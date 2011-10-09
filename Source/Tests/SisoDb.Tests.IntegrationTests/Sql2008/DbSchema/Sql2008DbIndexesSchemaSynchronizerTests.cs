//using System;
//using NUnit.Framework;
//using PineCone.Structures.Schemas;
//using SisoDb.Sql2008;
//using SisoDb.Sql2008.Dac;
//using SisoDb.Sql2008.DbSchema;

//namespace SisoDb.Tests.IntegrationTests.Sql2008.DbSchema
//{
//    [TestFixture]
//    public class Sql2008DbIndexesSchemaSynchronizerTests : Sql2008IntegrationTestBase
//    {
//        private IStructureSchema _structureSchema;
//        private string _structureSetPrefix;
//        private string _indexesTableName;
//        private ISisoDatabase _sqlDb;

//        protected override void OnFixtureInitialize()
//        {
//            DropStructureSet<Class_2579AF20_51A0_475A_A24D_8056828DB1DC>();

//            _structureSchema = Database.StructureSchemas.GetSchema(
//                TypeFor<Class_2579AF20_51A0_475A_A24D_8056828DB1DC>.Type);
//            _structureSetPrefix = typeof(Class_2579AF20_51A0_475A_A24D_8056828DB1DC).Name;
//            _indexesTableName = _structureSetPrefix + "Indexes";

//            _sqlDb = new Sql2008Database(Database.ConnectionInfo);
//        }

//        protected override void OnTestFinalize()
//        {
//            DropStructureSet<Class_2579AF20_51A0_475A_A24D_8056828DB1DC>();
//        }

//        private class Class_2579AF20_51A0_475A_A24D_8056828DB1DC
//        {
//            public Guid SisoId { get; set; }

//            public string IndexableMember1 { get; set; }

//            public int IndexableMember2 { get; set; }
//        }

//        [Test]
//        public void Synhronize_WhenTableIsMissingColumn_ColumnIsAdded()
//        {
//            CreateStructureSet();
//            var hashForColumn = SisoEnvironment.Resources.ResolveMemberPathGenerator().Generate("IndexableMember2");
//            DbHelper.DropColumns(_indexesTableName, hashForColumn);

//            using (var dbClient = new Sql2008DbClient(_sqlDb.ConnectionInfo, false))
//            {
//                var synhronizer = new SqlDbIndexesSchemaSynchronizer(dbClient);
//                synhronizer.Synchronize(_structureSchema);
//            }

//            var columnExists = DbHelper.ColumnsExist(_indexesTableName, hashForColumn);
//            Assert.IsTrue(columnExists);
//        }

//        [Test]
//        public void Synhronize_WhenTableHasObsoleteColumn_ColumnIsDropped()
//        {
//            CreateStructureSet();
//            var memberPathForObsolete = SisoEnvironment.Resources.ResolveMemberPathGenerator().Generate("ExtraColumn");
//            var obsoleteColumnDefinition = string.Format("[{0}] [int] sparse null", memberPathForObsolete);
//            DbHelper.AddColumns(_indexesTableName, obsoleteColumnDefinition);

//            using (var dbClient = new Sql2008DbClient(_sqlDb.ConnectionInfo, false))
//            {
//                var synhronizer = new SqlDbIndexesSchemaSynchronizer(dbClient);
//                synhronizer.Synchronize(_structureSchema);
//            }

//            var columnExists = DbHelper.ColumnsExist(_indexesTableName, memberPathForObsolete);
//            Assert.IsFalse(columnExists);
//        }

//        private void CreateStructureSet()
//        {
//            using (var dbClient = new Sql2008DbClient(_sqlDb.ConnectionInfo, false))
//            {
//                var upserter = new Sql2008DbSchemaUpserter(dbClient);
//                upserter.Upsert(_structureSchema);
//            }
//        }
//    }
//}