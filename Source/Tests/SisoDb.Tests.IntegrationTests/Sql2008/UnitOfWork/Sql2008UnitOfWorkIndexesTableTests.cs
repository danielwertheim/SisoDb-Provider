using System.Data;
using System.Linq;
using NUnit.Framework;

namespace SisoDb.Tests.IntegrationTests.Sql2008.UnitOfWork
{
    [TestFixture]
    public class Sql2008UnitOfWorkIndexesTableTests : Sql2008IntegrationTestBase
    {
        private readonly IMemberNameGenerator _memberNameGenerator;
        private readonly string _rootInt1Name;
        private readonly string _rootString1Name;
        private readonly string _nestedInt1Name;
        private readonly string _nestedString1Name;

        public Sql2008UnitOfWorkIndexesTableTests()
        {
            _memberNameGenerator = SisoEnvironment.Resources.ResolveMemberNameGenerator();
            _rootInt1Name = _memberNameGenerator.Generate("Int1");
            _rootString1Name = _memberNameGenerator.Generate("String1");
            _nestedInt1Name = _memberNameGenerator.Generate("Nested.Int1");
            _nestedString1Name = _memberNameGenerator.Generate("Nested.String1");
        }

        protected override void OnTestFinalize()
        {
            DropStructureSet<Root>();
            Database.StructureSchemas.StructureTypeFactory.Configurations.Clear();
        }

        [Test]
        public void Insert_WhenNestedValuesExists_ObjectGraphValuesAreIndexed()
        {
            var item = Root.CreateFullyPopulated();

            using (var uow = Database.CreateUnitOfWork())
            {
                uow.Insert(item);
                uow.Commit();
            }


            var table = DbHelper.GetTableBySql("select * from dbo.RootIndexes");
            var rows = table.AsEnumerable();
            Assert.AreEqual(1, rows.Count(), "Should have generated one row only.");
            Assert.AreEqual(5, table.Columns.Count, "Wrong num of columns.");

            Assert.AreEqual(142, rows.Select(r => r[_rootInt1Name]).SingleOrDefault());
            Assert.AreEqual("RootString1", rows.Select(r => r[_rootString1Name]).SingleOrDefault());
            
            Assert.AreEqual(242, rows.Select(r => r[_nestedInt1Name]).SingleOrDefault());
            Assert.AreEqual("NestedString1", rows.Select(r => r[_nestedString1Name]).SingleOrDefault());
        }

        [Test]
        public void Insert_WhenNestedValuesExists_ButSomeAreExcluded_ExcludedObjectGraphValuesAreNotIndexed()
        {
            var item = Root.CreateFullyPopulated();
            Database.StructureSchemas.StructureTypeFactory.Configurations.NewForType<Root>()
                .DoNotIndexThis(r => r.Int1, r => r.Nested.String1);

            using (var uow = Database.CreateUnitOfWork())
            {
                uow.Insert(item);
                uow.Commit();
            }

            var table = DbHelper.GetTableBySql("select * from dbo.RootIndexes");
            var rows = table.AsEnumerable();
            Assert.AreEqual(1, rows.Count(), "Should have generated one row only.");
            Assert.AreEqual(3, table.Columns.Count, "Wrong num of columns.");

            Assert.IsFalse(table.Columns.Contains(_rootInt1Name));
            Assert.IsFalse(table.Columns.Contains(_nestedString1Name));
            
            Assert.AreEqual("RootString1", rows.Select(r => r[_rootString1Name]).SingleOrDefault());
            Assert.AreEqual(242, rows.Select(r => r[_nestedInt1Name]).SingleOrDefault());
        }

        [Test]
        public void Insert_WhenNestedValuesExists_ButSomeAreExcluded_UsingNonGenericAPI_ExcludedObjectGraphValuesAreNotIndexed()
        {
            var item = Root.CreateFullyPopulated();
            Database.StructureSchemas.StructureTypeFactory.Configurations.NewForType<Root>()
                .DoNotIndexThis("Int1", "Nested.String1");

            using (var uow = Database.CreateUnitOfWork())
            {
                uow.Insert(item);
                uow.Commit();
            }

            var table = DbHelper.GetTableBySql("select * from dbo.RootIndexes");
            var rows = table.AsEnumerable();
            Assert.AreEqual(1, rows.Count(), "Should have generated one row only.");
            Assert.AreEqual(3, table.Columns.Count, "Wrong num of columns.");

            Assert.IsFalse(table.Columns.Contains(_rootInt1Name));
            Assert.IsFalse(table.Columns.Contains(_nestedString1Name));

            Assert.AreEqual("RootString1", rows.Select(r => r[_rootString1Name]).SingleOrDefault());
            Assert.AreEqual(242, rows.Select(r => r[_nestedInt1Name]).SingleOrDefault());
        }

        [Test]
        public void Insert_WhenNestedValuesExists_SpecificMembersAreIndexed_OnlySpecificIndexesAreStored()
        {
            var item = Root.CreateFullyPopulated();
            Database.StructureSchemas.StructureTypeFactory.Configurations.NewForType<Root>()
                .OnlyIndexThis(r => r.Int1, r => r.Nested, r => r.Nested.String1);

            using (var uow = Database.CreateUnitOfWork())
            {
                uow.Insert(item);
                uow.Commit();
            }

            var table = DbHelper.GetTableBySql("select * from dbo.RootIndexes");
            var rows = table.AsEnumerable();
            Assert.AreEqual(1, rows.Count(), "Should have generated one row only.");
            Assert.AreEqual(3, table.Columns.Count, "Wrong num of columns.");

            Assert.AreEqual(142, rows.Select(r => r[_rootInt1Name]).SingleOrDefault());
            Assert.AreEqual("NestedString1", rows.Select(r => r[_nestedString1Name]).SingleOrDefault());

            Assert.IsFalse(table.Columns.Contains(_rootString1Name));
            Assert.IsFalse(table.Columns.Contains(_nestedInt1Name));
        }

        [Test]
        public void Insert_WhenNestedValuesExists_SpecificMembersAreIndexed_UsingNonGenericAPI_OnlySpecificIndexesAreStored()
        {
            var item = Root.CreateFullyPopulated();
            Database.StructureSchemas.StructureTypeFactory.Configurations.NewForType<Root>()
                .OnlyIndexThis("Int1", "Nested", "Nested.String1");

            using (var uow = Database.CreateUnitOfWork())
            {
                uow.Insert(item);
                uow.Commit();
            }

            var table = DbHelper.GetTableBySql("select * from dbo.RootIndexes");
            var rows = table.AsEnumerable();
            Assert.AreEqual(1, rows.Count(), "Should have generated one row only.");
            Assert.AreEqual(3, table.Columns.Count, "Wrong num of columns.");

            Assert.AreEqual(142, rows.Select(r => r[_rootInt1Name]).SingleOrDefault());
            Assert.AreEqual("NestedString1", rows.Select(r => r[_nestedString1Name]).SingleOrDefault());

            Assert.IsFalse(table.Columns.Contains(_rootString1Name));
            Assert.IsFalse(table.Columns.Contains(_nestedInt1Name));
        }

        private class Root
        {
            public int SisoId { get; set; }

            public int Int1 { get; set; }

            public string String1 { get; set; }

            public Nested Nested { get; set; }

            public static Root CreateFullyPopulated()
            {
                return new Root
                       {
                           Int1 = 142,
                           String1 = "RootString1",
                           Nested = new Nested
                                    {
                                        Int1 = 242,
                                        String1 = "NestedString1"
                                    }
                       };
            }
        }

        private class Nested
        {
            public int Int1 { get; set; }

            public string String1 { get; set; }
        }
    }
}