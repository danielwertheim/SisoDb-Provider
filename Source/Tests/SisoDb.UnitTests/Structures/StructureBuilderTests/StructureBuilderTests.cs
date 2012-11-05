using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using SisoDb.Annotations;
using SisoDb.NCore;
using SisoDb.Structures;
using SisoDb.Structures.Schemas;
using SisoDb.UnitTests.Serialization;
using SisoDb.UnitTests.Structures.Schemas;

namespace SisoDb.UnitTests.Structures.StructureBuilderTests
{
    [TestFixture]
    public class StructureBuilderTests : StructureBuilderBaseTests
    {
        protected override void OnTestInitialize()
        {
            Builder = new StructureBuilder();
        }

        [Test]
        public void CreateStructures_WhenProcessing50Items_ItemsAreGettingGeneratedInCorrectOrder()
        {
            var schema = StructureSchemaTestFactory.CreateRealFrom<GuidItem>();
            var items = CreateGuidItems(50);

            var structures = Builder.CreateStructures(items, schema).ToArray();

            CollectionAssert.AreEqual(
                items.Select(i => i.StructureId).ToArray(),
                structures.Select(s => (Guid)s.Id.Value).ToArray());
        }

        [Test]
        public void CreateStructures_WhenProcessing1500Items_ItemsAreGettingGeneratedInCorrectOrder()
        {
            var schema = StructureSchemaTestFactory.CreateRealFrom<GuidItem>();
            var items = CreateGuidItems(1500);

            var structures = Builder.CreateStructures(items, schema).ToArray();

            CollectionAssert.AreEqual(
                items.Select(i => i.StructureId).ToArray(),
                structures.Select(s => (Guid)s.Id.Value).ToArray());
        }

        [Test]
        public void CreateStructure_WhenIdIsNotAssigned_IdIsAssigned()
        {
            var id = StructureId.Create(Guid.Parse("B551349B-53BD-4455-A509-A9B68B58700A"));
            var idGeneratorStub = new Mock<IStructureIdGenerator>();
            idGeneratorStub.Setup(s => s.Generate(It.IsAny<IStructureSchema>())).Returns(id);

            var item = new GuidItem { StructureId = Guid.Empty };
            var schema = StructureSchemaTestFactory.CreateRealFrom<GuidItem>();

            Builder.StructureIdGenerator = idGeneratorStub.Object;
            var structure = Builder.CreateStructure(item, schema);

            Assert.AreEqual(id, structure.Id);
            Assert.AreEqual(id.Value, item.StructureId);
        }

        [Test]
        public void CreateStructure_WhenIdIsAssigned_IdIsOverWritten()
        {
            var initialId = StructureId.Create(Guid.Parse("B551349B-53BD-4455-A509-A9B68B58700A"));
            var item = new GuidItem { StructureId = (Guid)initialId.Value };
            var schema = StructureSchemaTestFactory.CreateRealFrom<GuidItem>();

            var structure = Builder.CreateStructure(item, schema);

            Assert.AreNotEqual(initialId, structure.Id);
            Assert.AreNotEqual(initialId.Value, item.StructureId);
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

        [Test]
        public void CreateStructure_WhenIntOnFirstLevel_ReturnsSimpleValue()
        {
            var schema = StructureSchemaTestFactory.CreateRealFrom<TestItemForFirstLevel>();
            var item = new TestItemForFirstLevel { IntValue = 42 };

            var structure = Builder.CreateStructure(item, schema);

            var actual = structure.Indexes.Single(si => si.Path == "IntValue").Value;
            Assert.AreEqual(42, actual);
        }

        [Test]
        public void CreateStructure_WhenUIntOnFirstLevel_ReturnsSimpleValue()
        {
            var schema = StructureSchemaTestFactory.CreateRealFrom<TestItemForFirstLevel>();
            var item = new TestItemForFirstLevel { UIntValue = 42 };

            var structure = Builder.CreateStructure(item, schema);

            var actual = structure.Indexes.Single(si => si.Path == "UIntValue").Value;
            Assert.AreEqual(42, actual);
        }

        [Test]
        public void CreateStructure_WhenIntOnSecondLevel_ReturnsSimpleValue()
        {
            var schema = StructureSchemaTestFactory.CreateRealFrom<TestItemForSecondLevel>();
            var item = new TestItemForSecondLevel { Container = new Container { IntValue = 42 } };

            var structure = Builder.CreateStructure(item, schema);

            var actual = structure.Indexes.Single(si => si.Path == "Container.IntValue").Value;
            Assert.AreEqual(42, actual);
        }

        [Test]
        public void CreateStructure_WhenSpecificIdGeneratorIsPassed_SpecificIdGeneratorIsConsumed()
        {
            var idValue = new Guid("A058FCDE-A3D9-4EAA-AA41-0CE9D4A3FB1E");
            var idGeneratorMock = new Mock<IStructureIdGenerator>();
            idGeneratorMock.Setup(m => m.Generate(It.IsAny<IStructureSchema>())).Returns(StructureId.Create(idValue));

            var item = new TestItemForFirstLevel { IntValue = 42 };
            var schema = StructureSchemaTestFactory.CreateRealFrom<TestItemForFirstLevel>();

            Builder.StructureIdGenerator = idGeneratorMock.Object;
            var structure = Builder.CreateStructure(item, schema);

            idGeneratorMock.Verify(m => m.Generate(schema), Times.Once());
        }

        [Test]
        public void CreateStructures_WhenSpecificIdGeneratorIsPassed_SpecificIdGeneratorIsConsumed()
        {
            var idValues = new Queue<IStructureId>();
            idValues.Enqueue(StructureId.Create(Guid.Parse("A058FCDE-A3D9-4EAA-AA41-0CE9D4A3FB1E")));
            idValues.Enqueue(StructureId.Create(Guid.Parse("91D77A9D-C793-4F3D-9DD0-F1F336362C5C")));

            var items = new[] { new TestItemForFirstLevel { IntValue = 42 }, new TestItemForFirstLevel { IntValue = 43 } };
            var schema = StructureSchemaTestFactory.CreateRealFrom<TestItemForFirstLevel>();
            var idGeneratorMock = new Mock<IStructureIdGenerator>();
            idGeneratorMock
                .Setup(m => m.Generate(schema))
                .Returns(idValues.Dequeue);

            Builder.StructureIdGenerator = idGeneratorMock.Object;
            var structures = Builder.CreateStructures(items, schema).ToArray();

            idGeneratorMock.Verify(m => m.Generate(schema), Times.Exactly(2));
        }



        [Test]
        public void CreateStructure_WhenStructureContainsStructWithValue_ValueOfStructIsRepresentedInIndex()
        {
            var schema = StructureSchemaTestFactory.CreateRealFrom<IHaveStruct>();
            var item = new IHaveStruct { Content = "My content" };

            var structure = Builder.CreateStructure(item, schema);

            Assert.AreEqual(2, structure.Indexes.Count);
            Assert.AreEqual("Content", structure.Indexes[1].Path);
            Assert.AreEqual(typeof(MyText), structure.Indexes[1].DataType);
            Assert.AreEqual(new MyText("My content"), structure.Indexes[1].Value);
        }

        [Test]
        public void CreateStructure_WhenStructureContainsTimeStamp_TimeStampMemberIsAssignedAValue()
        {
            var fixedDateTime = new DateTime(2001, 01, 02, 03, 04, 05);
            SysDateTime.NowFn = () => fixedDateTime;
            var schema = StructureSchemaTestFactory.CreateRealFrom<IHaveTimeStamp>();
            var item = new IHaveTimeStamp();

            var structure = Builder.CreateStructure(item, schema);

            Assert.AreEqual("TimeStamp", structure.Indexes[1].Path);
            Assert.AreEqual(fixedDateTime, structure.Indexes[1].Value);
        }

        [Test]
        public void CreateStructure_WhenStructureContainsNullableTimeStamp_TimeStampMemberIsAssignedAValue()
        {
            var fixedDateTime = new DateTime(2001, 01, 02, 03, 04, 05);
            SysDateTime.NowFn = () => fixedDateTime;
            var schema = StructureSchemaTestFactory.CreateRealFrom<IHaveNullableTimeStamp>();
            var item = new IHaveNullableTimeStamp();

            var structure = Builder.CreateStructure(item, schema);

            Assert.AreEqual("TimeStamp", structure.Indexes[1].Path);
            Assert.AreEqual(fixedDateTime, structure.Indexes[1].Value);
        }

        [Test]
        public void CreateStructure_WhenStructureContainsConcurrencyTokenOfGuid_ItIsPresentInSchemaAsWellAsInStructure()
        {
            var schema = StructureSchemaTestFactory.CreateRealFrom<IHaveConcurrencyTokenOfGuid>();
            var item = new IHaveConcurrencyTokenOfGuid();

            var structure = Builder.CreateStructure(item, schema);

            Assert.IsTrue(schema.HasConcurrencyToken);
            Assert.AreEqual(2, structure.Indexes.Count);
            Assert.AreEqual("StructureId", structure.Indexes[0].Path);
            Assert.AreEqual("ConcurrencyToken", structure.Indexes[1].Path);
        }

        [Test]
        public void CreateStructure_When_structure_has_null_collection_It_should_create_structure_with_index_for_other_members()
        {
            var schema = StructureSchemaTestFactory.CreateRealFrom<WithNullCollection>();
            var item = new WithNullCollection {Temp = "Foo", Values = null};

            var structure = Builder.CreateStructure(item, schema);

            Assert.AreEqual(2, structure.Indexes.Count);
            Assert.AreEqual("StructureId", structure.Indexes[0].Path);
            Assert.AreEqual("Temp", structure.Indexes[1].Path);
        }

        [Test]
        public void CreateStructure_When_structure_has_null_collection_with_uniques_It_should_create_structure_with_index_for_other_members()
        {
            var schema = StructureSchemaTestFactory.CreateRealFrom<WithCollectionOfUniques>();
            var item = new WithCollectionOfUniques { Temp = "Foo", Values = null };

            var structure = Builder.CreateStructure(item, schema);

            Assert.AreEqual(2, structure.Indexes.Count);
            Assert.AreEqual("StructureId", structure.Indexes[0].Path);
            Assert.AreEqual("Temp", structure.Indexes[1].Path);
        }

        [Test]
        public void CreateStructure_When_structure_has_empty_collection_with_uniques_It_should_create_structure_with_index_for_other_members()
        {
            var schema = StructureSchemaTestFactory.CreateRealFrom<WithCollectionOfUniques>();
            var item = new WithCollectionOfUniques { Temp = "Foo", Values = new List<UniqueItem>() };

            var structure = Builder.CreateStructure(item, schema);

            Assert.AreEqual(2, structure.Indexes.Count);
            Assert.AreEqual("StructureId", structure.Indexes[0].Path);
            Assert.AreEqual("Temp", structure.Indexes[1].Path);
        }

        private class TestItemForFirstLevel
        {
            public Guid StructureId { get; set; }

            public int IntValue { get; set; }
            public uint UIntValue { get; set; }
        }

        private class TestItemForSecondLevel
        {
            public Guid StructureId { get; set; }

            public Container Container { get; set; }
        }

        private class Container
        {
            public int IntValue { get; set; }
        }

        private class IHaveStruct
        {
            public Guid StructureId { get; set; }

            public MyText Content { get; set; }
        }

        private class IHaveTimeStamp
        {
            public Guid StructureId { get; set; }
            public DateTime TimeStamp { get; set; }
        }

        private class IHaveNullableTimeStamp
        {
            public Guid StructureId { get; set; }
            public DateTime? TimeStamp { get; set; }
        }

        private class IHaveConcurrencyTokenOfGuid
        {
            public Guid StructureId { get; set; }
            public Guid ConcurrencyToken { get; set; }
        }

        public class WithNullCollection
        {
            public Guid StructureId { get; set; }
            public List<Item> Values { get; set; }
            public string Temp { get; set; }
        }

        public class Item
        {
            public string Value { get; set; }
        }

        public class WithCollectionOfUniques
        {
            public Guid StructureId { get; set; }
            public List<UniqueItem> Values { get; set; }
            public string Temp { get; set; }
        }

        public class UniqueItem
        {
            [Unique(UniqueModes.PerType)]
            public string Value { get; set; }
        }

        [Serializable]
        private struct MyText
        {
            private readonly string _value;

            public MyText(string value)
            {
                _value = value;
            }

            public static MyText Parse(string value)
            {
                return value == null ? null : new MyText(value);
            }

            public static implicit operator MyText(string value)
            {
                return new MyText(value);
            }

            public static implicit operator string(MyText item)
            {
                return item._value;
            }

            public override string ToString()
            {
                return _value;
            }
        }
    }
}