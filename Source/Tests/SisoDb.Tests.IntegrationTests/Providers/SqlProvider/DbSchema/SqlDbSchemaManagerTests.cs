using System;
using NUnit.Framework;
using SisoDb.Providers.SqlProvider;
using SisoDb.Providers.SqlProvider.DbSchema;
using SisoDb.Structures.Schemas;

namespace SisoDb.Tests.IntegrationTests.Providers.SqlProvider.DbSchema
{
    [TestFixture]
    public class SqlDbSchemaManagerTests : SqlIntegrationTestBase
    {
        private IStructureSchema _structureSchema;
        private string _structureTableName;
        private string _indexesTableName;
        private string _uniquesTableName;
        private ISqlDatabase _sqlDb;

        protected override void OnFixtureInitialize()
        {
            DropStructureSet<Class_53966417_B25D_49E1_966B_58754110781C>();

            _structureSchema = Database.StructureSchemas.GetSchema<Class_53966417_B25D_49E1_966B_58754110781C>();
            _structureTableName = _structureSchema.GetStructureTableName();
            _indexesTableName = _structureSchema.GetIndexesTableName();
            _uniquesTableName = _structureSchema.GetUniquesTableName();

            _sqlDb = new SqlDatabase(Database.ConnectionInfo);
        }

        protected override void OnTestFinalize()
        {
            DropStructureSet<Class_53966417_B25D_49E1_966B_58754110781C>();
        }

        [Test]
        public void DropStructureSet_WhenTablesExists_AllTablesAreDropped()
        {
            var manager = new SqlDbSchemaManager(_sqlDb.ConnectionInfo);
            manager.UpsertStructureSet(_structureSchema);

            manager.DropStructureSet(_structureSchema);

            var structureTableExists = DbHelper.TableExists(_structureTableName);
            var indexesTableExists = DbHelper.TableExists(_indexesTableName);
            var uniquesTableExists = DbHelper.TableExists(_uniquesTableName);

            Assert.IsFalse(structureTableExists);
            Assert.IsFalse(indexesTableExists);
            Assert.IsFalse(uniquesTableExists);
        }

        [Test]
        public void Upsert_WhenNoSetExists_TablesAreCreated()
        {
            var manager = new SqlDbSchemaManager(_sqlDb.ConnectionInfo);
            manager.UpsertStructureSet(_structureSchema);

            var structureTableExists = DbHelper.TableExists(_structureTableName);
            var indexesTableExists = DbHelper.TableExists(_indexesTableName);
            var uniquesTableExists = DbHelper.TableExists(_uniquesTableName);

            Assert.IsTrue(structureTableExists);
            Assert.IsTrue(indexesTableExists);
            Assert.IsTrue(uniquesTableExists);
        }

        [Test]
        public void Upsert_OnSameManagerInstanceWhenClassHasOneNewMember_ColumnIsNotAddedToIndexesTable()
        {
            var hashForColumn = SisoDbEnvironment.MemberNameGenerator.Generate("IndexableMember2");
            var manager = new SqlDbSchemaManager(_sqlDb.ConnectionInfo);
            manager.UpsertStructureSet(_structureSchema);
            DbHelper.DropColumns(_indexesTableName, hashForColumn);

            manager.UpsertStructureSet(_structureSchema);

            var columnExists = DbHelper.ColumnsExist(_indexesTableName, hashForColumn);
            Assert.IsFalse(columnExists);
        }

        [Test]
        public void Upsert_OnNewManagerInstanceWhenClassHasOneNewMember_ColumnIsAddedToIndexesTable()
        {
            var hashForColumn = SisoDbEnvironment.MemberNameGenerator.Generate("IndexableMember2");
            var manager = new SqlDbSchemaManager(_sqlDb.ConnectionInfo);
            manager.UpsertStructureSet(_structureSchema);
            DbHelper.DropColumns(_indexesTableName, hashForColumn);

            var manager2 = new SqlDbSchemaManager(_sqlDb.ConnectionInfo);
            manager2.UpsertStructureSet(_structureSchema);

            var columnExists = DbHelper.ColumnsExist(_indexesTableName, hashForColumn);
            Assert.IsTrue(columnExists);
        }

        [Test]
        public void Upsert_OnSameManagerInstanceWhenDbHasOneObsoleteMember_ColumnIsNotDroppedFromIndexesTable()
        {
            var manager = new SqlDbSchemaManager(_sqlDb.ConnectionInfo);
            manager.UpsertStructureSet(_structureSchema);

            var hashForObsoleteColumn = SisoDbEnvironment.MemberNameGenerator.Generate("ExtraColumn");
            var obsoleteColumnDefinition = string.Format("[{0}] [int] sparse null", hashForObsoleteColumn);
            DbHelper.AddColumns(_indexesTableName, obsoleteColumnDefinition);

            manager.UpsertStructureSet(_structureSchema);

            var columnExists = DbHelper.ColumnsExist(_indexesTableName, hashForObsoleteColumn);
            Assert.IsTrue(columnExists);
        }

        [Test]
        public void Upsert_OnNewManagerInstanceWhenDbHasOneObsoleteMember_ColumnIsDroppedFromIndexesTable()
        {
            var manager = new SqlDbSchemaManager(_sqlDb.ConnectionInfo);
            manager.UpsertStructureSet(_structureSchema);

            var hashForObsoleteColumn = SisoDbEnvironment.MemberNameGenerator.Generate("ExtraColumn");
            var obsoleteColumnDefinition = string.Format("[{0}] [int] sparse null", hashForObsoleteColumn);
            DbHelper.AddColumns(_indexesTableName, obsoleteColumnDefinition);

            var manager2 = new SqlDbSchemaManager(_sqlDb.ConnectionInfo);
            manager2.UpsertStructureSet(_structureSchema);

            var columnExists = DbHelper.ColumnsExist(_indexesTableName, hashForObsoleteColumn);
            Assert.IsFalse(columnExists);
        }

        private class Class_53966417_B25D_49E1_966B_58754110781C
        {
            public Guid Id { get; set; }

            public string IndexableMember1 { get; set; }

            public int IndexableMember2 { get; set; }
        }
    }
}