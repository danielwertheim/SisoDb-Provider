using System;
using NUnit.Framework;
using PineCone.Structures.Schemas;
using SisoDb.Sql2008;
using SisoDb.Sql2008.Dac;

namespace SisoDb.Tests.IntegrationTests.Sql2008.Dac
{
    [TestFixture]
    public class Sql2008DbClientDropTests : Sql2008IntegrationTestBase
    {
        private IStructureSchema _structureSchema;
        private string _structureSetPrefix;
        private string _structureTableName;
        private string _indexesTableName;
        private string _uniquesTableName;
        private ISisoDatabase _sqlDb;

        protected override void OnFixtureInitialize()
        {
            DropStructureSet<Class_12E6E3A7_482C_4E1A_88BE_393D29253203>();

            _structureSchema = Database.StructureSchemas.GetSchema(TypeFor<Class_12E6E3A7_482C_4E1A_88BE_393D29253203>.Type);
            _structureSetPrefix = typeof(Class_12E6E3A7_482C_4E1A_88BE_393D29253203).Name;
            _structureTableName = _structureSetPrefix + "Structure";
            _indexesTableName = _structureSetPrefix + "Indexes";
            _uniquesTableName = _structureSetPrefix + "Uniques";

            _sqlDb = new Sql2008Database(Database.ConnectionInfo);
        }

        protected override void OnTestFinalize()
        {
            DropStructureSet<Class_12E6E3A7_482C_4E1A_88BE_393D29253203>();
        }

        private class Class_12E6E3A7_482C_4E1A_88BE_393D29253203
        {
            public Guid SisoId { get; set; }

            public string IndexableMember1 { get; set; }

            public int IndexableMember2 { get; set; }
        }

        [Test]
        public void DropStructureSet_WhenTablesExists_AllTablesAreDropped()
        {
            using (var dbClient = new Sql2008DbClient(_sqlDb.ConnectionInfo, false))
            {
                dbClient.Drop(_structureSchema);
            }

            var structureTableExists = DbHelper.TableExists(_structureTableName);
            var indexesTableExists = DbHelper.TableExists(_indexesTableName);
            var uniquesTableExists = DbHelper.TableExists(_uniquesTableName);

            Assert.IsFalse(structureTableExists);
            Assert.IsFalse(indexesTableExists);
            Assert.IsFalse(uniquesTableExists);
        }
    }
}