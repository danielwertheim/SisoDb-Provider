using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using SisoDb.Core;
using SisoDb.Structures;

namespace SisoDb.Tests.UnitTests.Structures
{
    [TestFixture]
    public class StructureIndexesFactoryTests : UnitTestBase
    {
        [Test]
        public void GetIndexes_WhenItemWithNoEnumerable_WillNotConsumeStringConverter()
        {
            var stringConverterFake = new Mock<IStringConverter>();
            var item = new WithNoArray { Value = "A" };
            var schemaStub = StructureSchemaTestFactory.Stub<WithNoArray>(indexAccessorsPaths: new[] { "Value" });

            var factory = new StructureIndexesFactory(stringConverterFake.Object);
            var indexes = factory.GetIndexes(schemaStub, item, SisoId.NewIdentityId(1)).ToList();

            stringConverterFake.Verify(s => s.AsString<object>("A"), Times.Never());
        }

        [Test]
        public void GetIndexes_WhenItemWithEnumerable_WillConsumeStringConverter()
        {
            var stringConverterFake = new Mock<IStringConverter>();
            var item = new WithArray { Values = new[] { "A", "B" } };
            var schemaStub = StructureSchemaTestFactory.Stub<WithArray>(indexAccessorsPaths: new[] { "Values" });

            var factory = new StructureIndexesFactory(stringConverterFake.Object);
            var indexes = factory.GetIndexes(schemaStub, item, SisoId.NewIdentityId(1)).ToList();

            stringConverterFake.Verify(s => s.AsString<object>("A"), Times.Once());
            stringConverterFake.Verify(s => s.AsString<object>("B"), Times.Once());
        }

        [Test]
        public void GetIndexes_WhenItemWithNoEnumerable_ReturnsIndexWithStringValue()
        {
            var item = new WithNoArray { Value = "A" };
            var schemaStub = StructureSchemaTestFactory.Stub<WithNoArray>(indexAccessorsPaths: new[] {"Value"});

            var factory = new StructureIndexesFactory(SisoEnvironment.Formatting.StringConverter);
            var indexes = factory.GetIndexes(schemaStub, item, SisoId.NewIdentityId(1)).ToList();

            Assert.AreEqual("A", indexes[0].Value);
        }

        [Test]
        public void GetIndexes_WhenItemWithEnumerableWithOneElement_ReturnsIndexWithStringsAsOneValuedDelimitedString()
        {
            var item = new WithArray { Values = new[] { "A" } };
            var schemaStub = StructureSchemaTestFactory.Stub<WithArray>(indexAccessorsPaths: new[] {"Values"});
            
            var factory = new StructureIndexesFactory(SisoEnvironment.Formatting.StringConverter);
            var indexes = factory.GetIndexes(schemaStub, item, SisoId.NewIdentityId(1)).ToList();

            Assert.AreEqual("<$A$>", indexes[0].Value);
        }

        [Test]
        public void GetIndexes_WhenItemWithEnumerableWithTwoDifferentElements_ReturnsIndexWithStringsAsTwoValuedDelimitedString()
        {
            var item = new WithArray { Values = new[] { "A", "B" } };
            var schemaStub = StructureSchemaTestFactory.Stub<WithArray>(indexAccessorsPaths: new[] { "Values" });

            var factory = new StructureIndexesFactory(SisoEnvironment.Formatting.StringConverter);
            var indexes = factory.GetIndexes(schemaStub, item, SisoId.NewIdentityId(1)).ToList();

            Assert.AreEqual("<$A$><$B$>", indexes[0].Value);
        }

        [Test]
        public void GetIndexes_WhenItemWithEnumerableWithTwoEqualElements_ReturnsIndexWithStringsAsOneValuedDelimitedString()
        {
            var item = new WithArray { Values = new[] { "A", "A" } };
            var schemaStub = StructureSchemaTestFactory.Stub<WithArray>(indexAccessorsPaths: new[] { "Values" });

            var factory = new StructureIndexesFactory(SisoEnvironment.Formatting.StringConverter);
            var indexes = factory.GetIndexes(schemaStub, item, SisoId.NewIdentityId(1)).ToList();

            Assert.AreEqual("<$A$>", indexes[0].Value);
        }

        private class WithNoArray
        {
            public string Value { get; set; }
        }

        private class WithArray
        {
            public string[] Values { get; set; }
        }
    }
}