using System;
using NUnit.Framework;
using SisoDb.DbSchema;
using SisoDb.Structures;
using SisoDb.Structures.Schemas;

namespace SisoDb.UnitTests.DbSchema
{
    [TestFixture]
    public class ModelTableNamesTests : UnitTestBase
    {
        private readonly IDataTypeConverter _dataTypeConverter = new DataTypeConverter();

        protected override void OnTestFinalize()
        {
            DbSchemaNamingPolicy.Reset();
        }

        private DataTypeCode GetAsDataTypeCode(Type type, string memberName = null)
        {
            return _dataTypeConverter.Convert(type, memberName ?? "Foo");
        }

        [Test]
        public void AllTableNames_WhenRegistreredManualStructureNameGenerator_AllTableNamesShouldHaveGainedThePrefix()
        {
            DbSchemaNamingPolicy.StructureNamePrefix = "Gooofy_";

            var names = new ModelTableNames("MyStructure");

            foreach (var tableName in names.AllTableNames)
                Assert.IsTrue(tableName.StartsWith("Gooofy_"));
        }

        [Test]
        public void AllTableNames_Should_ContainAllTableNamesInCorrectOrder()
        {
            var names = new ModelTableNames("MyStructure");

            Assert.AreEqual(9, names.AllTableNames.Length);
            Assert.AreEqual(names.StructureTableName, names.AllTableNames[0]);
            Assert.AreEqual(names.UniquesTableName, names.AllTableNames[1]);
            Assert.AreEqual(names.IndexesTableNames.IntegersTableName, names.AllTableNames[2]);
            Assert.AreEqual(names.IndexesTableNames.FractalsTableName, names.AllTableNames[3]);
            Assert.AreEqual(names.IndexesTableNames.BooleansTableName, names.AllTableNames[4]);
            Assert.AreEqual(names.IndexesTableNames.DatesTableName, names.AllTableNames[5]);
            Assert.AreEqual(names.IndexesTableNames.GuidsTableName, names.AllTableNames[6]);
            Assert.AreEqual(names.IndexesTableNames.StringsTableName, names.AllTableNames[7]);
            Assert.AreEqual(names.IndexesTableNames.TextsTableName, names.AllTableNames[8]);
        }

        [Test]
        public void StructureTableName_WhenStructureIsNamed_MyStructure_it_ShouldBe_MyStructureStructure()
        {
            Assert.AreEqual("MyStructureStructure", new ModelTableNames("MyStructure").StructureTableName);
        }

        [Test]
        public void UniquesTableName_WhenStructureIsNamed_MyStructure_it_ShouldBe_MyStructureUniques()
        {
            Assert.AreEqual("MyStructureUniques", new ModelTableNames("MyStructure").UniquesTableName);
        }

        [Test]
        public void IntegersTableName_WhenStructureIsNamed_MyStructure_it_ShouldBe_MyStructureIntegers()
        {
            Assert.AreEqual("MyStructureIntegers", new ModelTableNames("MyStructure").IndexesTableNames.IntegersTableName);
        }

        [Test]
        public void FractalsTableName_WhenStructureIsNamed_MyStructure_it_ShouldBe_MyStructureFractals()
        {
            Assert.AreEqual("MyStructureFractals", new ModelTableNames("MyStructure").IndexesTableNames.FractalsTableName);
        }

        [Test]
        public void BooleansTableName_WhenStructureIsNamed_MyStructure_it_ShouldBe_MyStructureBooleans()
        {
            Assert.AreEqual("MyStructureBooleans", new ModelTableNames("MyStructure").IndexesTableNames.BooleansTableName);
        }

        [Test]
        public void DatesTableName_WhenStructureIsNamed_MyStructure_it_ShouldBe_MyStructureDates()
        {
            Assert.AreEqual("MyStructureDates", new ModelTableNames("MyStructure").IndexesTableNames.DatesTableName);
        }

        [Test]
        public void GuidsTableName_WhenStructureIsNamed_MyStructure_it_ShouldBe_MyStructureGuids()
        {
            Assert.AreEqual("MyStructureGuids", new ModelTableNames("MyStructure").IndexesTableNames.GuidsTableName);
        }

        [Test]
        public void StringsTableName_WhenStructureIsNamed_MyStructure_it_ShouldBe_MyStructureStrings()
        {
            Assert.AreEqual("MyStructureStrings", new ModelTableNames("MyStructure").IndexesTableNames.StringsTableName);
        }

        [Test]
        public void TextsTableName_WhenStructureIsNamed_MyStructure_it_ShouldBe_MyStructureTexts()
        {
            Assert.AreEqual("MyStructureTexts", new ModelTableNames("MyStructure").IndexesTableNames.TextsTableName);
        }

        [Test]
        public void GetNameByType_WhenIntegerType_and_StructureIsNamed_MyStructure_it_ShouldBe_MyStructureIntegers()
        {
            Assert.AreEqual("MyStructureIntegers", new ModelTableNames("MyStructure").IndexesTableNames.GetNameByType(GetAsDataTypeCode(typeof(int))));
        }

        [Test]
        public void GetNameByType_WhenIntegerType_and_StructureIsNamed_MyStructure_it_ShouldBe_MyStructureFractals()
        {
            Assert.AreEqual("MyStructureFractals", new ModelTableNames("MyStructure").IndexesTableNames.GetNameByType(GetAsDataTypeCode(typeof(decimal))));
        }

        [Test]
        public void GetNameByType_WhenBoolType_and_StructureIsNamed_MyStructure_it_ShouldBe_MyStructureBooleans()
        {
            Assert.AreEqual("MyStructureBooleans", new ModelTableNames("MyStructure").IndexesTableNames.GetNameByType(GetAsDataTypeCode(typeof(bool))));
        }

        [Test]
        public void GetNameByType_WhenDateTimeType_and_StructureIsNamed_MyStructure_it_ShouldBe_MyStructureDates()
        {
            Assert.AreEqual("MyStructureDates", new ModelTableNames("MyStructure").IndexesTableNames.GetNameByType(GetAsDataTypeCode(typeof(DateTime))));
        }

        [Test]
        public void GetNameByType_WhenGuidType_and_StructureIsNamed_MyStructure_it_ShouldBe_MyStructureGuids()
        {
            Assert.AreEqual("MyStructureGuids", new ModelTableNames("MyStructure").IndexesTableNames.GetNameByType(GetAsDataTypeCode(typeof(Guid))));
        }

        [Test]
        public void GetNameByType_WhenStringType_and_StructureIsNamed_MyStructure_it_ShouldBe_MyStructureStrings()
        {
            Assert.AreEqual("MyStructureStrings", new ModelTableNames("MyStructure").IndexesTableNames.GetNameByType(GetAsDataTypeCode(typeof(string))));
        }

        [Test]
        public void GetNameByType_WhenStringType_and_member_endswith_Text_and_StructureIsNamed_MyStructure_it_ShouldBe_MyStructureTexts()
        {
            Assert.AreEqual("MyStructureTexts", new ModelTableNames("MyStructure").IndexesTableNames.GetNameByType(GetAsDataTypeCode(typeof(string), "FooText")));
        }

        [Test]
        public void GetNameByType_WhenStringType_and_member_endswith_Content_and_StructureIsNamed_MyStructure_it_ShouldBe_MyStructureTexts()
        {
            Assert.AreEqual("MyStructureTexts", new ModelTableNames("MyStructure").IndexesTableNames.GetNameByType(GetAsDataTypeCode(typeof(string), "FooContent")));
        }

        [Test]
        public void GetNameByType_WhenStringType_and_member_endswith_Description_and_StructureIsNamed_MyStructure_it_ShouldBe_MyStructureTexts()
        {
            Assert.AreEqual("MyStructureTexts", new ModelTableNames("MyStructure").IndexesTableNames.GetNameByType(GetAsDataTypeCode(typeof(string), "FooDescription")));
        }

        [Test]
        public void GetNameByType_WhenEnumType_and_StructureIsNamed_MyStructure_it_ShouldBe_MyStructureStrings()
        {
            Assert.AreEqual("MyStructureStrings", new ModelTableNames("MyStructure").IndexesTableNames.GetNameByType(GetAsDataTypeCode(typeof(StringComparison))));
        }
    }
}