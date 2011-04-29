using System;
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
            var sisoId = SisoId.NewGuidId(new Guid("D2F88EBA-A39C-4EF6-86D0-82658FD8E891"));
            var schema = StructureSchemaTestFactory.CreateSchema<WithNoArray>();
            var item = new WithNoArray { Value = "A" };

            var indexFactory = new StructureIndexesFactory(stringConverterFake.Object);
            indexFactory.GetIndexes(schema, item, sisoId);

            stringConverterFake.Verify(s => s.AsString<object>("A"), Times.Never());
        }

        [Test]
        public void GetIndexes_WhenItemWithEnumerable_WillConsumeStringConverter()
        {
            var stringConverterFake = new Mock<IStringConverter>();
            var sisoId = SisoId.NewGuidId(new Guid("EDD397F4-E637-4298-BCC5-21CCE0851E91"));
            var schema = StructureSchemaTestFactory.CreateSchema<WithArray>();
            var item = new WithArray { Values = new[] { "A", "B" } };

            var indexFactory = new StructureIndexesFactory(stringConverterFake.Object);
            indexFactory.GetIndexes(schema, item, sisoId);

            stringConverterFake.Verify(s => s.AsString<object>("A"), Times.Once());
            stringConverterFake.Verify(s => s.AsString<object>("B"), Times.Once());
        }

        private class WithNoArray
        {
            public Guid SisoId { get; set; }

            public string Value { get; set; }
        }

        private class WithArray
        {
            public Guid SisoId { get; set; }

            public string[] Values { get; set; }
        }
    }
}