using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using SisoDb.Structures;
using SisoDb.Structures.Schemas;

namespace SisoDb.UnitTests.Structures
{
    [TestFixture]
    public class StructureTests : UnitTestBase
    {
        private IDataTypeConverter _converter;

        protected override void OnFixtureInitialize()
        {
            _converter = new DataTypeConverter();
        }

        private IStructureProperty CreateProperty(Type type)
        {
            var property = new Mock<IStructureProperty>();
            property.SetupGet(f => f.Name).Returns("Foo");
            property.SetupGet(f => f.DataType).Returns(type);

            return property.Object;
        }

        [Test]
        public void Ctor_WhenIndexesContainsNonUniqueUniqueIndex_ThrowsSisoDbException()
        {
            var structureId = StructureId.Create(1);
            var dataType = typeof (string);
            var dataTypeCode = _converter.Convert(CreateProperty(dataType));
            var indexes = new List<IStructureIndex>
            {
                new StructureIndex(structureId, "UniqueIndex1", "Value1", dataType, dataTypeCode, StructureIndexType.UniquePerInstance),
                new StructureIndex(structureId, "UniqueIndex1", "Value1", dataType, dataTypeCode,  StructureIndexType.UniquePerInstance)
            };
            
            Assert.Throws<SisoDbException>(() => new Structure("Name", structureId, indexes.ToArray()));
        }
    }
}