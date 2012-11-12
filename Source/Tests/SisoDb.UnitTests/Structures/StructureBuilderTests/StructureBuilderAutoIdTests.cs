using System;
using System.Linq;
using System.Threading;
using Moq;
using NUnit.Framework;
using SisoDb.Structures;
using SisoDb.Structures.IdGenerators;
using SisoDb.Structures.Schemas;
using SisoDb.UnitTests.Structures.Schemas;

namespace SisoDb.UnitTests.Structures.StructureBuilderTests
{
    [TestFixture]
    public class StructureBuilderAutoIdTests : StructureBuilderBaseTests
    {
        protected override void OnTestInitialize()
        {
            Builder = new StructureBuilderAutoId();
        }

        [Test]
        public void CreateStructure_WhenNoIdIsAssigned_IdIsGenerated()
        {
            var schema = StructureSchemaTestFactory.CreateRealFrom<GuidItem>();
            var initialIdValue = Guid.Empty;
            var item = new GuidItem { StructureId = initialIdValue };

            var structure = Builder.CreateStructure(item, schema);

            Assert.AreNotEqual(initialIdValue, structure.Id.Value);
            Assert.AreNotEqual(initialIdValue, item.StructureId);
        }

        [Test]
        public void CreateStructures_inSerial_WhenNoIdsAreAssigned_IdsAreGenerated()
        {
            var schema = StructureSchemaTestFactory.CreateRealFrom<GuidItem>();
            var initialIdValue = Guid.Empty;
            var items = CreateGuidItems(StructureBuilder.LimitForSerialStructureBuilding);
            var idGenerator = new GuidStructureIdGeneratorProxy();
            Builder.StructureIdGenerator = idGenerator;

            var structures = Builder.CreateStructures(items, schema);

            Assert.AreEqual(items.Length, idGenerator.CallsToGenerateSingle);
            Assert.AreEqual(0, idGenerator.CallsToGenerateMany);
            for (var i = 0; i < items.Length; i++)
            {
                Assert.AreNotEqual(initialIdValue, structures[i].Id.Value);
                Assert.AreNotEqual(initialIdValue, items[i].StructureId);
            }
        }

        [Test]
        public void CreateStructures_inParallel_WhenNoIdAreAssigned_IdsAreGenerated()
        {
            var schema = StructureSchemaTestFactory.CreateRealFrom<GuidItem>();
            var initialIdValue = Guid.Empty;
            var items = CreateGuidItems(StructureBuilder.LimitForSerialStructureBuilding + 1);
            var idGenerator = new GuidStructureIdGeneratorProxy();
            Builder.StructureIdGenerator = idGenerator;

            var structures = Builder.CreateStructures(items, schema);

            Assert.AreEqual(items.Length, idGenerator.CallsToGenerateSingle);
            Assert.AreEqual(0, idGenerator.CallsToGenerateMany);
            for (var i = 0; i < items.Length; i++)
            {
                Assert.AreNotEqual(initialIdValue, structures[i].Id.Value);
                Assert.AreNotEqual(initialIdValue, items[i].StructureId);
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
        public void CreateStructures_inSerial_WhenIdsAreAssigned_IdsAreNotGenerated()
        {
            var schema = StructureSchemaTestFactory.CreateRealFrom<GuidItem>();
            var items = CreateGuidItems(StructureBuilder.LimitForSerialStructureBuilding, true);
            var idGenerator = new GuidStructureIdGeneratorProxy();
            Builder.StructureIdGenerator = idGenerator;

            var structures = Builder.CreateStructures(items, schema);

            Assert.AreEqual(0, idGenerator.CallsToGenerateSingle);
            Assert.AreEqual(0, idGenerator.CallsToGenerateMany);
            for (var i = 0; i < items.Length; i++)
            {
                Assert.AreNotEqual(Guid.Empty, structures[i].Id.Value);
                Assert.AreNotEqual(Guid.Empty, items[i].StructureId);
            }
        }

        [Test]
        public void CreateStructures_inParallel_WhenIdsAreAssigned_IdsAreNotGenerated()
        {
            var schema = StructureSchemaTestFactory.CreateRealFrom<GuidItem>();
            var items = CreateGuidItems(StructureBuilder.LimitForSerialStructureBuilding + 1, true);
            var idGenerator = new GuidStructureIdGeneratorProxy();
            Builder.StructureIdGenerator = idGenerator;

            var structures = Builder.CreateStructures(items, schema);

            Assert.AreEqual(0, idGenerator.CallsToGenerateSingle);
            Assert.AreEqual(0, idGenerator.CallsToGenerateMany);
            for (var i = 0; i < items.Length; i++)
            {
                Assert.AreNotEqual(Guid.Empty, structures[i].Id.Value);
                Assert.AreNotEqual(Guid.Empty, items[i].StructureId);
            }
        }

        [Test]
        public void CreateStructure_WhenSpecificIdGeneratorIsPassed_SpecificIdGeneratorIsNotConsumed()
        {
            var idValue = new Guid("A058FCDE-A3D9-4EAA-AA41-0CE9D4A3FB1E");
            var item = new GuidItem { StructureId = idValue };
            var schema = StructureSchemaTestFactory.CreateRealFrom<GuidItem>();
            var idGenerator = new GuidStructureIdGeneratorProxy();
            Builder.StructureIdGenerator = idGenerator;

            Builder.StructureIdGenerator = idGenerator;
            var structure = Builder.CreateStructure(item, schema);

            Assert.AreEqual(0, idGenerator.CallsToGenerateSingle);
            Assert.AreEqual(0, idGenerator.CallsToGenerateMany);
        }

        [Test]
        public void CreateStructures_WhenSpecificIdGeneratorIsPassed_SpecificIdGeneratorIsConsumed()
        {
            var items = new[] { new GuidItem { Value = 42 }, new GuidItem { Value = 43 } };
            var schema = StructureSchemaTestFactory.CreateRealFrom<GuidItem>();
            var idGenerator = new GuidStructureIdGeneratorProxy();
            Builder.StructureIdGenerator = idGenerator;

            var structures = Builder.CreateStructures(items, schema);

            Assert.AreEqual(2, idGenerator.CallsToGenerateSingle);
            Assert.AreEqual(0, idGenerator.CallsToGenerateMany);
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