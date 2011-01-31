using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Moq;
using NUnit.Framework;
using SisoDb.Structures;
using SisoDb.Structures.Schemas;

namespace SisoDb.Tests.UnitTests.Structures
{
    [TestFixture]
    public class StructureBuilderTests : UnitTestBase
    {
        protected override void OnTestFinalize()
        {
            SisoDbEnvironment.StringConverter = new StringConverter();
        }

        [Test]
        public void CreateStructure_WhenItemWithNoEnumerable_WillNotConsumeStringConverter()
        {
            var stringConverterFake = new Mock<IStringConverter>();
            SisoDbEnvironment.StringConverter = stringConverterFake.Object;
            var schema = new AutoSchemaBuilder<WithNoArray>().CreateSchema();
            var item = new WithNoArray { Value = "A" };

            var builder = new StructureBuilder();
            builder.CreateStructure(item, schema);


            stringConverterFake.Verify(s => s.AsString<object>("A"), Times.Never());
        }

        [Test]
        public void CreateStructure_WhenItemWithEnumerable_WillConsumeStringConverter()
        {
            var stringConverterFake = new Mock<IStringConverter>();
            SisoDbEnvironment.StringConverter = stringConverterFake.Object;
            var schema = new AutoSchemaBuilder<WithArray>().CreateSchema();
            var item = new WithArray { Values = new[] { "A", "B" } };

            var builder = new StructureBuilder();
            builder.CreateStructure(item, schema);

            stringConverterFake.Verify(s => s.AsString<object>("A"), Times.Once());
            stringConverterFake.Verify(s => s.AsString<object>("B"), Times.Once());
        }

        [Test]
        public void CreateStructure_WhenItemWithNoEnumerable_ReturnsIndexWithStringValue()
        {
            var schema = new AutoSchemaBuilder<WithNoArray>().CreateSchema();
            var item = new WithNoArray { Value = "A" };

            var builder = new StructureBuilder();
            var structure = builder.CreateStructure(item, schema);

            var indexes = structure.Indexes.ToList();
            Assert.AreEqual("A", indexes[0].Value);
        }

        [Test]
        public void CreateStructure_WhenItemWithEnumerableWithOneElement_ReturnsIndexWithStringsAsOneValuedDelimitedString()
        {
            var schema = new AutoSchemaBuilder<WithArray>().CreateSchema();
            var item = new WithArray { Values = new[] { "A" } };

            var builder = new StructureBuilder();
            var structure = builder.CreateStructure(item, schema);

            var indexes = structure.Indexes.ToList();
            Assert.AreEqual("<$A$>", indexes[0].Value);
        }

        [Test]
        public void CreateStructure_WhenItemWithEnumerableWithTwoDifferentElements_ReturnsIndexWithStringsAsTwoValuedDelimitedString()
        {
            var schema = new AutoSchemaBuilder<WithArray>().CreateSchema();
            var item = new WithArray { Values = new[] { "A", "B" } };

            var builder = new StructureBuilder();
            var structure = builder.CreateStructure(item, schema);

            var indexes = structure.Indexes.ToList();
            Assert.AreEqual("<$A$><$B$>", indexes[0].Value);
        }

        [Test]
        public void CreateStructure_WhenItemWithEnumerableWithTwoEqualElements_ReturnsIndexWithStringsAsOneValuedDelimitedString()
        {
            var schema = new AutoSchemaBuilder<WithArray>().CreateSchema();
            var item = new WithArray { Values = new[] { "A", "A" } };

            var builder = new StructureBuilder();
            var structure = builder.CreateStructure(item, schema);

            var indexes = structure.Indexes.ToList();
            Assert.AreEqual("<$A$>", indexes[0].Value);
        }

        [Test]
        public void CreateStructure_WhenItemWithByteArray_NoIndexShouldBeCreatedForByteArray()
        {
            var schema = new AutoSchemaBuilder<WithBytes>().CreateSchema();
            var item = new WithBytes { Bytes1 = BitConverter.GetBytes(242) };

            var builder = new StructureBuilder();
            var structure = builder.CreateStructure(item, schema);

            var indexes = structure.Indexes.ToList();
            Assert.AreEqual(1, indexes.Count);
            Assert.IsTrue(indexes[0].Name.StartsWith("DummyMember_"));
        }

        private class WithBytes
        {
            public Guid Id { get; set; }

            public int DummyMember { get; set; }

            public byte[] Bytes1 { get; set; }

            public IEnumerable<byte> Bytes2 { get; set; }

            public IList<byte> Bytes3 { get; set; }

            public List<byte> Bytes4 { get; set; }

            public ICollection<byte> Bytes5 { get; set; }

            public Collection<byte> Bytes6 { get; set; }
        }

        private class WithByte
        {
            public Guid Id { get; set; }

            public byte Byte { get; set; }
        }

        private class WithArray
        {
            public Guid Id { get; set; }

            public string[] Values { get; set; }
        }

        private class WithNoArray
        {
            public Guid Id { get; set; }

            public string Value { get; set; }
        }
    }
}