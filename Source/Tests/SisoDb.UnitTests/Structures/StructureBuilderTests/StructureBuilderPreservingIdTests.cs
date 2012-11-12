using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using SisoDb.Structures;
using SisoDb.Structures.Schemas;
using SisoDb.UnitTests.Structures.Schemas;

namespace SisoDb.UnitTests.Structures.StructureBuilderTests
{
    [TestFixture]
    public class StructureBuilderPreservingIdTests : StructureBuilderBaseTests
    {
        protected override void OnTestInitialize()
        {
            Builder = new StructureBuilderPreservingId();
        }

        [Test]
        public void CreateStructure_WhenNoIdIsAssigned_IdIsNotGenerated()
        {
            var idGeneratorFake = new Mock<IStructureIdGenerator>();
            var schema = StructureSchemaTestFactory.CreateRealFrom<GuidItem>();
            var initialIdValue = Guid.Empty;
            var item = new GuidItem { StructureId = initialIdValue };

            Builder.StructureIdGenerator = idGeneratorFake.Object;
            var structure = Builder.CreateStructure(item, schema);

            Assert.AreEqual(initialIdValue, structure.Id.Value);
            Assert.AreEqual(initialIdValue, item.StructureId);
            idGeneratorFake.Verify(f => f.Generate(schema), Times.Never());
            idGeneratorFake.Verify(f => f.Generate(schema, It.IsAny<int>()), Times.Never());
        }

        [Test]
        public void CreateStructures_inSerial_WhenNoIdIsAssigned_IdsAreNotGenerated()
        {
            var idGeneratorFake = new Mock<IStructureIdGenerator>();
            var schema = StructureSchemaTestFactory.CreateRealFrom<GuidItem>();
            var initialIdValue = Guid.Empty;
            var items = CreateGuidItems(StructureBuilder.LimitForSerialStructureBuilding);

            Builder.StructureIdGenerator = idGeneratorFake.Object;
            var structures = Builder.CreateStructures(items, schema);

            idGeneratorFake.Verify(f => f.Generate(schema), Times.Never());
            idGeneratorFake.Verify(f => f.Generate(schema, It.IsAny<int>()), Times.Never());
            for (var i = 0; i < items.Length; i++)
            {
                Assert.AreEqual(initialIdValue, structures[i].Id.Value);
                Assert.AreEqual(initialIdValue, items[i].StructureId);    
            }
        }

        [Test]
        public void CreateStructures_inParallel_WhenNoIdIsAssigned_IdsAreNotGenerated()
        {
            var idGeneratorFake = new Mock<IStructureIdGenerator>();
            var schema = StructureSchemaTestFactory.CreateRealFrom<GuidItem>();
            var initialIdValue = Guid.Empty;
            var items = CreateGuidItems(StructureBuilder.LimitForSerialStructureBuilding + 1);

            Builder.StructureIdGenerator = idGeneratorFake.Object;
            var structures = Builder.CreateStructures(items, schema);

            idGeneratorFake.Verify(f => f.Generate(schema), Times.Never());
            idGeneratorFake.Verify(f => f.Generate(schema, It.IsAny<int>()), Times.Never());
            for (var i = 0; i < items.Length; i++)
            {
                Assert.AreEqual(initialIdValue, structures[i].Id.Value);
                Assert.AreEqual(initialIdValue, items[i].StructureId);
            }
        }

        [Test]
        public void CreateStructure_WhenIdIsAssigned_IdIsNotOverWritten()
        {
            var schema = StructureSchemaTestFactory.CreateRealFrom<GuidItem>();
            var initialId = StructureId.Create(Guid.Parse("B551349B-53BD-4455-A509-A9B68B58700A"));
            var item = new GuidItem { StructureId = (Guid)initialId.Value };

            var structure = Builder.CreateStructure(item, schema);

            Assert.AreEqual(initialId, structure.Id);
            Assert.AreEqual(initialId.Value, item.StructureId);
        }

        [Test]
        public void CreateStructure_WhenSpecificIdGeneratorIsPassed_SpecificIdGeneratorIsNotConsumed()
        {
            var idValue = new Guid("A058FCDE-A3D9-4EAA-AA41-0CE9D4A3FB1E");
            var idGeneratorMock = new Mock<IStructureIdGenerator>();
            idGeneratorMock.Setup(m => m.Generate(It.IsAny<IStructureSchema>())).Returns(StructureId.Create(idValue));

            var item = new GuidItem { StructureId = idValue };
            var schema = StructureSchemaTestFactory.CreateRealFrom<GuidItem>();

            Builder.StructureIdGenerator = idGeneratorMock.Object;
            Builder.CreateStructure(item, schema);

            idGeneratorMock.Verify(m => m.Generate(schema), Times.Never());
        }

        [Test]
        public void CreateStructures_WhenSpecificIdGeneratorIsPassed_SpecificIdGeneratorIsNotConsumed()
        {
            var items = new[] { new GuidItem { Value = 42 }, new GuidItem { Value = 43 } };
            var schema = StructureSchemaTestFactory.CreateRealFrom<GuidItem>();
            var idGeneratorMock = new Mock<IStructureIdGenerator>();

            Builder.StructureIdGenerator = idGeneratorMock.Object;
            var structures = Builder.CreateStructures(items, schema).ToArray();

            idGeneratorMock.Verify(m => m.Generate(schema, It.IsAny<int>()), Times.Never());
        }

        [Test]
        public void CreateStructures_WhenSerializerIsSpecified_SerializerIsConsumed()
        {
            var schema = StructureSchemaTestFactory.CreateRealFrom<GuidItem>();
            var serializer = new FakeSerializer
            {
                OnSerialize = (i, s) =>
                {
                    var itm = (GuidItem)i;
                    return itm.StructureId + ";" + itm.Value;
                }
            };
            var items = CreateGuidItems(3).ToArray();

            Builder.StructureSerializer = serializer;
            var structures = Builder.CreateStructures(items, schema).ToList();

            Assert.AreEqual(3, structures.Count);
            Assert.AreEqual(items[0].StructureId + ";" + items[0].Value, structures[0].Data);
            Assert.AreEqual(items[1].StructureId + ";" + items[1].Value, structures[1].Data);
            Assert.AreEqual(items[2].StructureId + ";" + items[2].Value, structures[2].Data);
        }
    }
}