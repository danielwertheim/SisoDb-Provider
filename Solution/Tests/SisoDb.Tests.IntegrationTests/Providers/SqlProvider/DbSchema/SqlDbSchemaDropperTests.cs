using System;
using NUnit.Framework;
using SisoDb.Core;
using SisoDb.Providers.SqlProvider;
using SisoDb.Providers.SqlProvider.DbSchema;
using SisoDb.Structures.Schemas;

namespace SisoDb.Tests.IntegrationTests.Providers.SqlProvider.DbSchema
{
    [TestFixture]
    public class SqlDbSchemaDropperTests : SqlIntegrationTestBase
    {
        private IStructureSchema _structureSchema;
        private string _structureSetPrefix;
        private string _structureTableName;
        private string _indexesTableName;
        private string _uniquesTableName;
        private ISqlDatabase _sqlDb;

        protected override void OnFixtureInitialize()
        {
            DropStructureSet<Class_12E6E3A7_482C_4E1A_88BE_393D29253203>();

            _structureSchema = Database.StructureSchemas.GetSchema(TypeFor<Class_12E6E3A7_482C_4E1A_88BE_393D29253203>.Type);
            _structureSetPrefix = typeof(Class_12E6E3A7_482C_4E1A_88BE_393D29253203).Name;
            _structureTableName = _structureSetPrefix + "Structure";
            _indexesTableName = _structureSetPrefix + "Indexes";
            _uniquesTableName = _structureSetPrefix + "Uniques";

            _sqlDb = new SqlDatabase(Database.ConnectionInfo);
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
            using (var dbClient = new SqlDbClient(_sqlDb.ConnectionInfo, false))
            {
                var upserter = new SqlDbSchemaDropper(dbClient);
                upserter.Drop(_structureSchema);
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