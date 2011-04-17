using System.Data;
using System.Linq;
using NUnit.Framework;

namespace SisoDb.Tests.IntegrationTests.Providers.SqlProvider.UnitOfWork
{
    [TestFixture]
    public class SqlUnitOfWorkIndexesTableTests : SqlIntegrationTestBase
    {
        protected override void OnTestFinalize()
        {
            DropStructureSet<Root>();
        }

        [Test]
        public void Insert_WhenNestedValuesExists_ObjectGraphValuesAreIndexed()
        {
            var item = Root.CreateFullyPopulated();

            using(var uow = Database.CreateUnitOfWork())
            {
                uow.Insert(item);
                uow.Commit();
            }

            var table = DbHelper.GetTableBySql("select * from dbo.RootIndexes").AsEnumerable();
            var memberNameGenerator = SisoEnvironment.Resources.ResolveMemberNameGenerator();
            var rootInt1Name = memberNameGenerator.Generate("Int1");
            var rootString1Name = memberNameGenerator.Generate("String1");
            var nestedInt1Name = memberNameGenerator.Generate("Nested.Int1");
            var nestedString1Name = memberNameGenerator.Generate("Nested.String1");
            Assert.AreEqual(1, table.Count(), "Should have generated one row only.");
            Assert.AreEqual(142, table.Select(r => r[rootInt1Name]).SingleOrDefault());
            Assert.AreEqual("RootString1", table.Select(r => r[rootString1Name]).SingleOrDefault());
            Assert.AreEqual(242, table.Select(r => r[nestedInt1Name]).SingleOrDefault());
            Assert.AreEqual("NestedString1", table.Select(r => r[nestedString1Name]).SingleOrDefault());
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