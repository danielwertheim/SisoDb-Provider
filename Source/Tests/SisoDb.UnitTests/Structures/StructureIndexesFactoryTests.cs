using System;
using System.Linq;
using NUnit.Framework;
using SisoDb.Structures;
using SisoDb.UnitTests.Structures.Schemas;

namespace SisoDb.UnitTests.Structures
{
    [TestFixture]
    public class StructureIndexesFactoryTests : UnitTestBase
    {
        private Func<IStructureId> _structureIdGenerator;

        protected override void OnFixtureInitialize()
        {
            _structureIdGenerator = () => StructureId.Create(SequentialGuid.New());
        }

        [Test]
        public void GetIndexes_WhenItemHasGuidId_ReturnsId()
        {
            var id = Guid.Parse("1F0E8C1D-7AF5-418F-A6F6-A40B7F31CB00");
            var item = new WithGuidId { StructureId = id };
            var schemaStub = StructureSchemaTestFactory.CreateRealFrom<WithGuidId>();

            var factory = new StructureIndexesFactory();
            var indexes = factory.CreateIndexes(schemaStub, item, _structureIdGenerator.Invoke()).ToList();

            Assert.AreEqual(id, indexes.Single(i => i.Path == "StructureId").Value);
        }

        [Test]
        public void GetIndexes_WhenItemHasNulledGuidId_ReturnsNoIndex()
        {
            var item = new WithNullableGuidId { StructureId = null };
            var schemaStub = StructureSchemaTestFactory.CreateRealFrom<WithNullableGuidId>();

            var factory = new StructureIndexesFactory();
            var indexes = factory.CreateIndexes(schemaStub, item, _structureIdGenerator.Invoke()).ToList();

            Assert.AreEqual(0, indexes.Count);
        }

        [Test]
        public void GetIndexes_WhenNullableGuidIdHasValue_ReturnsId()
        {
            var id = Guid.Parse("1F0E8C1D-7AF5-418F-A6F6-A40B7F31CB00");
            var item = new WithNullableGuidId { StructureId = id };
            var schemaStub = StructureSchemaTestFactory.CreateRealFrom<WithNullableGuidId>();

            var factory = new StructureIndexesFactory();
            var indexes = factory.CreateIndexes(schemaStub, item, _structureIdGenerator.Invoke()).ToList();

            Assert.AreEqual(id, indexes.Single(i => i.Path == "StructureId").Value);
        }

        [Test]
        public void GetIndexes_WhenItemWithNullString_ReturnsNoIndex()
        {
            var item = new WithNoArray { StringValue = null };
            var schemaStub = StructureSchemaTestFactory.CreateRealFrom<WithNoArray>();

            var factory = new StructureIndexesFactory();
            var indexes = factory.CreateIndexes(schemaStub, item, _structureIdGenerator.Invoke()).ToList();

            Assert.IsNull(indexes.SingleOrDefault(i => i.Path == "StringValue"));
        }

        [Test]
        public void GetIndexes_WhenItemWithAssignedString_ReturnsIndexWithStringValue()
        {
            var item = new WithNoArray { StringValue = "A" };
            var schemaStub = StructureSchemaTestFactory.CreateRealFrom<WithNoArray>();

            var factory = new StructureIndexesFactory();
            var indexes = factory.CreateIndexes(schemaStub, item, _structureIdGenerator.Invoke()).ToList();

            Assert.AreEqual("A", indexes.Single(i => i.Path == "StringValue").Value);
        }

        [Test]
        public void GetIndexes_WhenItemWithAssignedInt_ReturnsIndexWithIntValue()
        {
            var item = new WithNoArray { IntValue = 42 };
            var schemaStub = StructureSchemaTestFactory.CreateRealFrom<WithNoArray>();

            var factory = new StructureIndexesFactory();
            var indexes = factory.CreateIndexes(schemaStub, item, _structureIdGenerator.Invoke()).ToList();

            Assert.AreEqual(42, indexes.Single(i => i.Path == "IntValue").Value);
        }

        [Test]
        public void GetIndexes_WhenItemWithEnumerableWithOneNullInt_ReturnsNullIndex()
        {
            var item = new WithArray { NullableIntValues = new int?[] { null } };
            var schemaStub = StructureSchemaTestFactory.CreateRealFrom<WithArray>();

            var factory = new StructureIndexesFactory();
            var indexes = factory.CreateIndexes(schemaStub, item, _structureIdGenerator.Invoke()).ToList();

            Assert.IsNull(indexes.SingleOrDefault(i => i.Path == "NullableIntValues"));
        }

        [Test]
        public void GetIndexes_WhenItemWithEnumerableWithOneNullString_ReturnsNullIndex()
        {
            var item = new WithArray { StringValues = new string[] { null } };
            var schemaStub = StructureSchemaTestFactory.CreateRealFrom<WithArray>();

            var factory = new StructureIndexesFactory();
            var indexes = factory.CreateIndexes(schemaStub, item, _structureIdGenerator.Invoke()).ToList();

            Assert.IsNull(indexes.SingleOrDefault(i => i.Path == "StringValues"));
        }

        [Test]
        public void GetIndexes_WhenItemWithEnumerableWithOneString_ReturnsIndexWithString()
        {
            var item = new WithArray { StringValues = new[] { "A" } };
            var schemaStub = StructureSchemaTestFactory.CreateRealFrom<WithArray>();

            var factory = new StructureIndexesFactory();
            var indexes = factory.CreateIndexes(schemaStub, item, _structureIdGenerator.Invoke()).ToList();

            Assert.AreEqual("A", indexes.Single(i => i.Path == "StringValues").Value);
        }

        [Test]
        public void GetIndexes_WhenItemWithEnumerableWithOneString_ReturnsIndexWithDataTypeOfStringElement()
        {
            var item = new WithArray { StringValues = new[] { "A" } };
            var schemaStub = StructureSchemaTestFactory.CreateRealFrom<WithArray>();

            var factory = new StructureIndexesFactory();
            var indexes = factory.CreateIndexes(schemaStub, item, _structureIdGenerator.Invoke()).ToList();

            Assert.AreEqual(DataTypeCode.String, indexes.Single(i => i.Path == "StringValues").DataTypeCode);
        }

        [Test]
        public void GetIndexes_WhenItemWithComplexEnumerable_ReturnsIndexWithDataTypeOfStringElement()
        {
            var item = new WithComplexArray { Items = new[] { new Complex {Name = "Foo", Value = 42} } };
            var schemaStub = StructureSchemaTestFactory.CreateRealFrom<WithComplexArray>();

            var factory = new StructureIndexesFactory();
            var indexes = factory.CreateIndexes(schemaStub, item, _structureIdGenerator.Invoke()).ToList();

            Assert.AreEqual(DataTypeCode.String, indexes.Single(i => i.Path == "Items.Name").DataTypeCode);
            Assert.AreEqual(DataTypeCode.IntegerNumber, indexes.Single(i => i.Path == "Items.Value").DataTypeCode);
        }

        [Test]
        public void GetIndexes_WhenItemWithEnumerableWithOneInt_ReturnsIndexWithInt()
        {
            var item = new WithArray { IntValues = new[] { 42 } };
            var schema = StructureSchemaTestFactory.CreateRealFrom<WithArray>();

            var factory = new StructureIndexesFactory();
            var indexes = factory.CreateIndexes(schema, item, _structureIdGenerator.Invoke()).ToList();

            Assert.AreEqual(42, indexes.Single(i => i.Path == "IntValues").Value);
        }

        [Test]
        public void GetIndexes_WhenItemWithEnumerableWithTwoDifferentStrings_ReturnsTwoStringIndexes()
        {
            var item = new WithArray { StringValues = new[] { "A", "B" } };
            var schemaStub = StructureSchemaTestFactory.CreateRealFrom<WithArray>();

            var factory = new StructureIndexesFactory();
            var indexes = factory.CreateIndexes(schemaStub, item, _structureIdGenerator.Invoke()).ToList();

            Assert.AreEqual("A", indexes[1].Value);
            Assert.AreEqual("B", indexes[2].Value);
        }

        [Test]
        public void GetIndexes_WhenItemWithEnumerableWithTwoDifferentInts_ReturnsTwoIntIndexes()
        {
            var item = new WithArray { IntValues = new[] { 42, 43 } };
            var schemaStub = StructureSchemaTestFactory.CreateRealFrom<WithArray>();

            var factory = new StructureIndexesFactory();
            var indexes = factory.CreateIndexes(schemaStub, item, _structureIdGenerator.Invoke()).ToList();

            Assert.AreEqual(42, indexes[1].Value);
            Assert.AreEqual(43, indexes[2].Value);
        }

        [Test]
        public void GetIndexes_WhenItemWithEnumerableWithTwoEqualElements_ReturnsTwoStringIndexes()
        {
            var item = new WithArray { StringValues = new[] { "A", "A" } };
            var schemaStub = StructureSchemaTestFactory.CreateRealFrom<WithArray>();

            var factory = new StructureIndexesFactory();
            var indexes = factory.CreateIndexes(schemaStub, item, _structureIdGenerator.Invoke()).ToList();

            Assert.AreEqual("A", indexes[1].Value);
            Assert.AreEqual("A", indexes[2].Value);
        }

        [Test]
        public void GetIndexes_WhenItemWithEnumerableWithTwoEqualElements_ReturnsTwoIntIndexes()
        {
            var item = new WithArray { IntValues = new[] { 42, 42 } };
            var schemaStub = StructureSchemaTestFactory.CreateRealFrom<WithArray>();

            var factory = new StructureIndexesFactory();
            var indexes = factory.CreateIndexes(schemaStub, item, _structureIdGenerator.Invoke()).ToList();

            Assert.AreEqual(42, indexes[1].Value);
            Assert.AreEqual(42, indexes[2].Value);
        }

        private class WithGuidId
        {
            public Guid StructureId { get; set; }
        }

        private class WithNullableGuidId
        {
            public Guid? StructureId { get; set; }
        }

        private class WithNoArray
        {
            public Guid StructureId { get; set; }

            public string StringValue { get; set; }

            public int IntValue { get; set; }
        }

        private class WithArray
        {
            public Guid StructureId { get; set; }

            public string[] StringValues { get; set; }

            public int[] IntValues { get; set; }

            public int?[] NullableIntValues { get; set; }
        }

        private class WithComplexArray
        {
            public Guid StructureId { get; set; }

            public Complex[] Items { get; set; }
        }

        private class Complex
        {
            public string Name { get; set; }
            public int Value { get; set; }
        }
    }
}