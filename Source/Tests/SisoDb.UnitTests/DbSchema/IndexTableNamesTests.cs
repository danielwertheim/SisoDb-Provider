using System;
using System.Linq;
using NUnit.Framework;
using SisoDb.DbSchema;

namespace SisoDb.UnitTests.DbSchema
{
    [TestFixture]
    public class IndexTableNamesTests : UnitTestBase
    {
        protected override void OnTestFinalize()
        {
            DbSchemaNamingPolicy.Reset();
        }

        [Test]
        public void AllTableNames_WhenRegistreredManualStructureNameGenerator_AllTableNamesShouldHaveGainedThePrefix()
        {
            DbSchemaNamingPolicy.StructureNameGenerator = s => string.Concat("Gooofy_", s);

            var allTableNames = new IndexesTableNames("MyStructure").AllTableNames;

            foreach (var tableName in allTableNames)
                Assert.IsTrue(tableName.StartsWith("Gooofy_"));
        }

        [Test]
        public void AllTableNames_Should_ContainAllTableNames()
        {
            var tableNames = new IndexesTableNames("MyStructure");

            Assert.AreEqual(7, tableNames.AllTableNames.Length);
            Assert.AreEqual(1, tableNames.AllTableNames.Count(t => t == tableNames.IntegersTableName));
            Assert.AreEqual(1, tableNames.AllTableNames.Count(t => t == tableNames.FractalsTableName));
            Assert.AreEqual(1, tableNames.AllTableNames.Count(t => t == tableNames.BooleansTableName));
            Assert.AreEqual(1, tableNames.AllTableNames.Count(t => t == tableNames.DatesTableName));
            Assert.AreEqual(1, tableNames.AllTableNames.Count(t => t == tableNames.GuidsTableName));
            Assert.AreEqual(1, tableNames.AllTableNames.Count(t => t == tableNames.StringsTableName));
            Assert.AreEqual(1, tableNames.AllTableNames.Count(t => t == tableNames.TextsTableName));
        }

        [Test]
        public void IntegersTableName_WhenStructureIsNamed_MyStructure_it_ShouldBe_MyStructureIntegers()
        {
            Assert.AreEqual("MyStructureIntegers", new IndexesTableNames("MyStructure").IntegersTableName);
        }

        [Test]
        public void FractalsTableName_WhenStructureIsNamed_MyStructure_it_ShouldBe_MyStructureFractals()
        {
            Assert.AreEqual("MyStructureFractals", new IndexesTableNames("MyStructure").FractalsTableName);
        }

        [Test]
        public void BooleansTableName_WhenStructureIsNamed_MyStructure_it_ShouldBe_MyStructureBooleans()
        {
            Assert.AreEqual("MyStructureBooleans", new IndexesTableNames("MyStructure").BooleansTableName);
        }

        [Test]
        public void DatesTableName_WhenStructureIsNamed_MyStructure_it_ShouldBe_MyStructureDates()
        {
            Assert.AreEqual("MyStructureDates", new IndexesTableNames("MyStructure").DatesTableName);
        }

        [Test]
        public void GuidsTableName_WhenStructureIsNamed_MyStructure_it_ShouldBe_MyStructureGuids()
        {
            Assert.AreEqual("MyStructureGuids", new IndexesTableNames("MyStructure").GuidsTableName);
        }

        [Test]
        public void StringsTableName_WhenStructureIsNamed_MyStructure_it_ShouldBe_MyStructureStrings()
        {
            Assert.AreEqual("MyStructureStrings", new IndexesTableNames("MyStructure").StringsTableName);
        }

        [Test]
        public void TextsTableName_WhenStructureIsNamed_MyStructure_it_ShouldBe_MyStructureTexts()
        {
            Assert.AreEqual("MyStructureTexts", new IndexesTableNames("MyStructure").TextsTableName);
        }

        [Test]
        public void GetNameByType_WhenIntegerType_and_StructureIsNamed_MyStructure_it_ShouldBe_MyStructureIntegers()
        {
            Assert.AreEqual("MyStructureIntegers", new IndexesTableNames("MyStructure").GetNameByType(typeof(int)));
        }

        [Test]
        public void GetNameByType_WhenIntegerType_and_StructureIsNamed_MyStructure_it_ShouldBe_MyStructureFractals()
        {
            Assert.AreEqual("MyStructureFractals", new IndexesTableNames("MyStructure").GetNameByType(typeof(decimal)));
        }

        [Test]
        public void GetNameByType_WhenBoolType_and_StructureIsNamed_MyStructure_it_ShouldBe_MyStructureBooleans()
        {
            Assert.AreEqual("MyStructureBooleans", new IndexesTableNames("MyStructure").GetNameByType(typeof(bool)));
        }

        [Test]
        public void GetNameByType_WhenDateTimeType_and_StructureIsNamed_MyStructure_it_ShouldBe_MyStructureDates()
        {
            Assert.AreEqual("MyStructureDates", new IndexesTableNames("MyStructure").GetNameByType(typeof(DateTime)));
        }

        [Test]
        public void GetNameByType_WhenGuidType_and_StructureIsNamed_MyStructure_it_ShouldBe_MyStructureGuids()
        {
            Assert.AreEqual("MyStructureGuids", new IndexesTableNames("MyStructure").GetNameByType(typeof(Guid)));
        }

        [Test]
        public void GetNameByType_WhenStringType_and_StructureIsNamed_MyStructure_it_ShouldBe_MyStructureStrings()
        {
            Assert.AreEqual("MyStructureStrings", new IndexesTableNames("MyStructure").GetNameByType(typeof(string)));
        }

        [Test]
        public void GetNameByType_WhenTextType_and_StructureIsNamed_MyStructure_it_ShouldBe_MyStructureTexts()
        {
            Assert.AreEqual("MyStructureTexts", new IndexesTableNames("MyStructure").GetNameByType(typeof(Text)));
        }

        [Test]
        public void GetNameByType_WhenEnumType_and_StructureIsNamed_MyStructure_it_ShouldBe_MyStructureStrings()
        {
            Assert.AreEqual("MyStructureStrings", new IndexesTableNames("MyStructure").GetNameByType(typeof(StringComparison)));
        }
    }
}