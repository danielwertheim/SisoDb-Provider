using System;
using Moq;
using NUnit.Framework;
using SisoDb.Structures;
using SisoDb.Structures.Schemas;

namespace SisoDb.UnitTests.Structures
{
    [TestFixture]
    public class StructureIndexTests : UnitTestBase
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
        public void Equals_WhenSameStructureIdNameAndValue_ReturnsTrue()
        {
            var structureId = StructureId.Create(Guid.Parse("06E2FC67-AB9F-4E65-A2C8-5FC897597887"));
            const string thevalue = "TheValue";
            
            var structure1 = new StructureIndex(structureId, "TheName", thevalue, thevalue.GetType(), _converter.Convert(CreateProperty(thevalue.GetType())));
            var structure2 = new StructureIndex(structureId, "TheName", thevalue, thevalue.GetType(), _converter.Convert(CreateProperty(thevalue.GetType())));

            Assert.AreEqual(structure1, structure2);
        }

        [Test]
        public void Equals_WhenDifferentGuidStructureId_ReturnsFalse()
        {
            var structureId1 = StructureId.Create(Guid.Parse("06E2FC67-AB9F-4E65-A2C8-5FC897597887"));
            var structureId2 = StructureId.Create(Guid.Parse("14D4D3EC-6E1E-4839-ACC7-EA3B4653CF96"));
            const string thevalue = "TheValue";
            
            var structure1 = new StructureIndex(structureId1, "TheName", thevalue, thevalue.GetType(), _converter.Convert(CreateProperty(thevalue.GetType())));
            var structure2 = new StructureIndex(structureId2, "TheName", thevalue, thevalue.GetType(), _converter.Convert(CreateProperty(thevalue.GetType())));

            Assert.AreNotEqual(structure1, structure2);
        }

        [Test]
        public void Equals_WhenDifferentName_ReturnsFalse()
        {
            var structureId = StructureId.Create(Guid.Parse("06E2FC67-AB9F-4E65-A2C8-5FC897597887"));
            const string thevalue = "TheValue";
            
            var structure1 = new StructureIndex(structureId, "TheName", thevalue, thevalue.GetType(), _converter.Convert(CreateProperty(thevalue.GetType())));
            var structure2 = new StructureIndex(structureId, "TheOtherName", thevalue, thevalue.GetType(), _converter.Convert(CreateProperty(thevalue.GetType())));

            Assert.AreNotEqual(structure1, structure2);
        }

        [Test]
        public void Equals_WhenDifferentValue_ReturnsFalse()
        {
            var structureId = StructureId.Create(Guid.Parse("06E2FC67-AB9F-4E65-A2C8-5FC897597887"));
            var dataType = typeof (string);
            var dataTypeCode = _converter.Convert(CreateProperty(dataType));
            
            var structure1 = new StructureIndex(structureId, "TheName", "TheValue", dataType, dataTypeCode);
            var structure2 = new StructureIndex(structureId, "TheName", "OtherValue", dataType, dataTypeCode);

            Assert.AreNotEqual(structure1, structure2);
        }
    }
}