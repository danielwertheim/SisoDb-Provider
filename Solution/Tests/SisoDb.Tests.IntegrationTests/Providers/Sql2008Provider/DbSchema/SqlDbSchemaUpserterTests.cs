using System;
using NUnit.Framework;
using SisoDb.Core;
using SisoDb.Providers.Sql2008Provider;
using SisoDb.Providers.Sql2008Provider.DbSchema;
using SisoDb.Structures.Schemas;

namespace SisoDb.Tests.IntegrationTests.Providers.Sql2008Provider.DbSchema
{
    [TestFixture]
    public class SqlDbSchemaUpserterTests : SqlIntegrationTestBase
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
        public void Upsert_WhenNoSetExists_TablesAreCreated()
        {
            using(var dbClient = new SqlDbClient(_sqlDb.ConnectionInfo, false))
            {
                var upserter = new SqlDbSchemaUpserter(dbClient);
                upserter.Upsert(_structureSchema);   
            }

            var structureTableExists = DbHelper.TableExists(_structureTableName);
            var indexesTableExists = DbHelper.TableExists(_indexesTableName);
            var uniquesTableExists = DbHelper.TableExists(_uniquesTableName);

            Assert.IsTrue(structureTableExists);
            Assert.IsTrue(indexesTableExists);
            Assert.IsTrue(uniquesTableExists);
        }

        [Test]
        public void Upsert_WhenClassHasOneNewMember_ColumnIsAddedToIndexesTable()
        {
            var hashForColumn = SisoEnvironment.Resources.ResolveMemberNameGenerator().Generate("IndexableMember2");
            
            using (var dbClient = new SqlDbClient(_sqlDb.ConnectionInfo, false))
            {
                var upserter = new SqlDbSchemaUpserter(dbClient);

                upserter.Upsert(_structureSchema);

                DbHelper.DropColumns(_indexesTableName, hashForColumn);

                upserter.Upsert(_structureSchema);
            }
            
            var columnExists = DbHelper.ColumnsExist(_indexesTableName, hashForColumn);
            Assert.IsTrue(columnExists);
        }

        [Test]
        public void Upsert_WhenDbHasOneObsoleteMember_ColumnIsDroppedFromIndexesTable()
        {
            var hashForObsoleteColumn = SisoEnvironment.Resources.ResolveMemberNameGenerator().Generate("ExtraColumn");
            using (var dbClient = new SqlDbClient(_sqlDb.ConnectionInfo, false))
            {
                var upserter = new SqlDbSchemaUpserter(dbClient);

                upserter.Upsert(_structureSchema);

                var obsoleteColumnDefinition = string.Format("[{0}] [int] sparse null", hashForObsoleteColumn);
                DbHelper.AddColumns(_indexesTableName, obsoleteColumnDefinition);

                upserter.Upsert(_structureSchema);
            }

            var columnExists = DbHelper.ColumnsExist(_indexesTableName, hashForObsoleteColumn);
            Assert.IsFalse(columnExists);
        }
    }
}