using System;
using NUnit.Framework;
using SisoDb.Structures;
using SisoDb.UnitTests.Structures.Schemas;

namespace SisoDb.UnitTests.Structures.StructureBuilderTests
{
    [TestFixture]
    public class StructureBuilderNestedStructuresTests : StructureBuilderBaseTests
    {
        protected override void OnTestInitialize()
        {
            Builder = new StructureBuilder();
        }

        [Test]
        public void CreateStructure_WhenNestedStructureExists_NestedStructureWillNotBePartOfStructure()
        {
            var schema = StructureSchemaTestFactory.CreateRealFrom<Structure1>();
            var item = new Structure1 { IntOnStructure1 = 142, Nested = new Structure2 { IntOnStructure2 = 242 } };

            var structure = Builder.CreateStructure(item, schema);

            Assert.AreEqual(2, structure.Indexes.Count);
            Assert.AreEqual("StructureId", structure.Indexes[0].Path);
            Assert.AreEqual("IntOnStructure1", structure.Indexes[1].Path);
        }

        private class Structure1
        {
            public Guid StructureId { get; set; }

            public int IntOnStructure1 { get; set; }

            public Structure2 Nested { get; set; }
        }

        private class Structure2
        {
            public Guid StructureId { get; set; }

            public int IntOnStructure2 { get; set; }
        }
    }
}