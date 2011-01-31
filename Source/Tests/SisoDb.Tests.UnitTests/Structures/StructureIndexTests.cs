using System;
using NUnit.Framework;
using SisoDb.Structures;

namespace SisoDb.Tests.UnitTests.Structures
{
    [TestFixture]
    public class StructureIndexTests : UnitTestBase
    {
        [Test]
        public void Equals_WhenSameStructureIdNameAndValue_ReturnsTrue()
        {
            var guid = Guid.Parse("06E2FC67-AB9F-4E65-A2C8-5FC897597887");
            const bool isUniqueTemp = true;

            var structure1 = new StructureIndex(StructureId.NewGuidId(guid), "TheName", "TheValue", isUniqueTemp);
            var structure2 = new StructureIndex(StructureId.NewGuidId(guid), "TheName", "TheValue", isUniqueTemp);

            Assert.AreEqual(structure1, structure2);
        }

        [Test]
        public void Equals_WhenDifferentGuidStructureId_ReturnsFalse()
        {
            var guid1 = Guid.Parse("06E2FC67-AB9F-4E65-A2C8-5FC897597887");
            var guid2 = Guid.Parse("14D4D3EC-6E1E-4839-ACC7-EA3B4653CF96");
            const bool isUniqueTemp = true;

            var structure1 = new StructureIndex(StructureId.NewGuidId(guid1), "TheName", "TheValue", isUniqueTemp);
            var structure2 = new StructureIndex(StructureId.NewGuidId(guid2), "TheName", "TheValue", isUniqueTemp);

            Assert.AreNotEqual(structure1, structure2);
        }

        [Test]
        public void Equals_WhenDifferentIdentityStructureId_ReturnsFalse()
        {
            const bool isUniqueTemp = true;

            var structure1 = new StructureIndex(StructureId.NewIdentityId(1), "TheName", "TheValue", isUniqueTemp);
            var structure2 = new StructureIndex(StructureId.NewIdentityId(2), "TheName", "TheValue", isUniqueTemp);

            Assert.AreNotEqual(structure1, structure2);
        }

        [Test]
        public void Equals_WhenDifferentName_ReturnsFalse()
        {
            var guid1 = Guid.Parse("06E2FC67-AB9F-4E65-A2C8-5FC897597887");
            const bool isUniqueTemp = true;

            var structure1 = new StructureIndex(StructureId.NewGuidId(guid1), "TheName", "TheValue", isUniqueTemp);
            var structure2 = new StructureIndex(StructureId.NewGuidId(guid1), "OtherName", "TheValue", isUniqueTemp);

            Assert.AreNotEqual(structure1, structure2);
        }

        [Test]
        public void Equals_WhenDifferentValue_ReturnsFalse()
        {
            var guid1 = Guid.Parse("06E2FC67-AB9F-4E65-A2C8-5FC897597887");
            const bool isUniqueTemp = true;

            var structure1 = new StructureIndex(StructureId.NewGuidId(guid1), "TheName", "TheValue", isUniqueTemp);
            var structure2 = new StructureIndex(StructureId.NewGuidId(guid1), "TheName", "OtherValue", isUniqueTemp);

            Assert.AreNotEqual(structure1, structure2);
        }
    }
}