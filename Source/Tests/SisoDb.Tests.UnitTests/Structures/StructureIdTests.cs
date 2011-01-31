using System;
using NUnit.Framework;
using SisoDb.Structures;

namespace SisoDb.Tests.UnitTests.Structures
{
    [TestFixture]
    public class StructureIdTests : UnitTestBase
    {
        [Test]
        public void NewIdentityId_WhenNegativeIdentity_ThrowsArgumentOutOfRange()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => StructureId.NewIdentityId(-1));
        }

        [Test]
        public void NewGuidId_WhenEmptyGuid_ThrowsArgumentOutOfRange()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => StructureId.NewGuidId(Guid.Empty));
        }

        [Test]
        public void ToString_WhenIdentity_ReturnsStringRepresentation()
        {
            var id = StructureId.NewIdentityId(1);

            Assert.AreEqual("1", id.ToString());
        }

        [Test]
        public void ToString_WhenGuid_ReturnsStringRepresentation()
        {
            const string guidString = "fc47a673-5a5b-419b-9a40-a756591aa7bf";
            var guid = Guid.Parse(guidString);

            var id = StructureId.NewGuidId(guid);

            Assert.AreEqual(guidString, id.ToString());
        }

        [Test]
        public void Equals_WhenSameIdentities_ReturnsTrue()
        {
            var id1 = StructureId.NewIdentityId(1);
            var id2 = StructureId.NewIdentityId(1);

            Assert.AreEqual(id1, id2);
        }

        [Test]
        public void Equals_WhenDifferentIdentities_ReturnsFalse()
        {
            var id1 = StructureId.NewIdentityId(1);
            var id2 = StructureId.NewIdentityId(2);

            Assert.AreNotEqual(id1, id2);
        }

        [Test]
        public void Equals_WhenSameGuids_ReturnsTrue()
        {
            var g = Guid.Parse("fc47a673-5a5b-419b-9a40-a756591aa7bf");
            var id1 = StructureId.NewGuidId(g);
            var id2 = StructureId.NewGuidId(g);

            Assert.AreEqual(id1, id2);
        }

        [Test]
        public void Equals_WhenDifferentGuids_ReturnsFalse()
        {
            var guid1 = Guid.Parse("06E2FC67-AB9F-4E65-A2C8-5FC897597887");
            var guid2 = Guid.Parse("14D4D3EC-6E1E-4839-ACC7-EA3B4653CF96");

            var id1 = StructureId.NewGuidId(guid1);
            var id2 = StructureId.NewGuidId(guid2);

            Assert.AreNotEqual(id1, id2);
        }
    }
}