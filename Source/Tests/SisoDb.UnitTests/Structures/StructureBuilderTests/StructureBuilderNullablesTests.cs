using System;
using NUnit.Framework;
using SisoDb.Structures;
using SisoDb.UnitTests.Structures.Schemas;
using SisoDb.UnitTests.TestFactories;

namespace SisoDb.UnitTests.Structures.StructureBuilderTests
{
    [TestFixture]
    public class StructureBuilderNullablesTests : StructureBuilderBaseTests
    {
        protected override void OnTestInitialize()
        {
            Builder = new StructureBuilder();
        }

        [Test]
        public void CreateStructure_WhenItemWithNullablesHasValues_IndexesAreCreated()
        {
            var schema = StructureSchemaTestFactory.CreateRealFrom<TestItemWithNullables>();
            var item = TestItemWithNullables.CreatePopulated();

            var structure = Builder.CreateStructure(item, schema);

            Assert.AreEqual(3, structure.Indexes.Count);
            Assert.AreEqual("StructureId", structure.Indexes[0].Path);

            Assert.AreEqual("NullableInt", structure.Indexes[1].Path);
            Assert.AreEqual(typeof(int?), structure.Indexes[1].DataType);
            Assert.AreEqual(item.NullableInt, structure.Indexes[1].Value);

            Assert.AreEqual("NullableGuid", structure.Indexes[2].Path);
            Assert.AreEqual(typeof(Guid?), structure.Indexes[2].DataType);
            Assert.AreEqual(item.NullableGuid, structure.Indexes[2].Value);
        }

        [Test]
        public void CreateStructure_WhenItemWithNullablesHasNullValues_IndexesAreCreated()
        {
            var schema = StructureSchemaTestFactory.CreateRealFrom<TestItemWithNullables>();
            var item = TestItemWithNullables.CreateWithNullValues();

            var structure = Builder.CreateStructure(item, schema);

            Assert.AreEqual(1, structure.Indexes.Count);
            Assert.AreEqual("StructureId", structure.Indexes[0].Path);
        }

        [Test]
        public void CreateStructure_WhenChildItemWithInheritedNullablesHasValues_IndexesAreCreated()
        {
            var schema = StructureSchemaTestFactory.CreateRealFrom<ChildWithNullables>();
            var item = ChildWithNullables.CreatePopulated();

            var structure = Builder.CreateStructure(item, schema);

            Assert.AreEqual(3, structure.Indexes.Count);
            Assert.AreEqual("StructureId", structure.Indexes[0].Path);

            Assert.AreEqual("NullableInt", structure.Indexes[1].Path);
            Assert.AreEqual(typeof(int?), structure.Indexes[1].DataType);
            Assert.AreEqual(item.NullableInt, structure.Indexes[1].Value);

            Assert.AreEqual("NullableGuid", structure.Indexes[2].Path);
            Assert.AreEqual(typeof(Guid?), structure.Indexes[2].DataType);
            Assert.AreEqual(item.NullableGuid, structure.Indexes[2].Value);
        }

        [Test]
        public void CreateStructure_WhenChildItemWithInheritedNullablesHasNullValues_IndexesAreCreated()
        {
            var schema = StructureSchemaTestFactory.CreateRealFrom<ChildWithNullables>();
            var item = ChildWithNullables.CreateWithNullValues();

            var structure = Builder.CreateStructure(item, schema);

            Assert.AreEqual(1, structure.Indexes.Count);
            Assert.AreEqual("StructureId", structure.Indexes[0].Path);
        }

        private abstract class RootWithNullables
        {
            public Guid StructureId { get; set; }
            public int? NullableInt { get; set; }
            public Guid? NullableGuid { get; set; } 
        }

        private class ChildWithNullables : RootWithNullables
        {
            public static ChildWithNullables CreateWithNullValues()
            {
                return new ChildWithNullables();
            }

            public static ChildWithNullables CreatePopulated()
            {
                return new ChildWithNullables
                {
                    NullableInt = 42,
                    NullableGuid = Guid.Parse("e327e168-c6f5-415a-9a7b-fa82dc73c5d9")
                };
            }
        }

        private class TestItemWithNullables
        {
            public Guid StructureId { get; set; }
            public int? NullableInt { get; set; }
            public Guid? NullableGuid { get; set; }

            public static TestItemWithNullables CreateWithNullValues()
            {
                return new TestItemWithNullables();
            }

            public static TestItemWithNullables CreatePopulated()
            {
                return new TestItemWithNullables
                {
                    NullableInt = 42,
                    NullableGuid = Guid.Parse("e327e168-c6f5-415a-9a7b-fa82dc73c5d9")
                };
            }
        }
    }
}