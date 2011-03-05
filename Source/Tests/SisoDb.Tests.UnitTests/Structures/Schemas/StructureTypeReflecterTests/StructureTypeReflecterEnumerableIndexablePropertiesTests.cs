using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using NUnit.Framework;
using SisoDb.Structures.Schemas;
using SisoDb.TestUtils;

namespace SisoDb.Tests.UnitTests.Structures.Schemas.TypeInfoTests
{
    [TestFixture]
    public class StructureTypeReflecterEnumerableIndexablePropertiesTests : UnitTestBase
    {
        private readonly IStructureTypeReflecter _reflecter = new StructureTypeReflecter();

        [Test]
        public void GetEnumerableIndexablePropertyInfos_WhenNoEnumerableIndexesExists_ReturnsEmptyList()
        {
            var properties = _reflecter.GetEnumerableIndexablePropertyInfos(typeof(WithNoEnumerableMembers));

            CustomAssert.IsEmpty(properties);
        }

        [Test]
        public void GetEnumerableIndexablePropertyInfos_WhenIListOfTIndexesExists_ReturnsTheMember()
        {
            var properties = _reflecter.GetEnumerableIndexablePropertyInfos(typeof(WithCollectionIndexes))
                .Where(p => p.Name == "IList1")
                .SingleOrDefault();

            Assert.IsNotNull(properties);
        }

        [Test]
        public void GetEnumerableIndexablePropertyInfos_WhenIEnumerableOfTIndexesExists_ReturnsTheMember()
        {
            var properties = _reflecter.GetEnumerableIndexablePropertyInfos(typeof(WithCollectionIndexes))
                .Where(p => p.Name == "IEnumerable1")
                .SingleOrDefault();

            Assert.IsNotNull(properties);
        }

        [Test]
        public void GetEnumerableIndexablePropertyInfos_WhenIEnumerableOfTIndexesExists_DoesNotReturnTheElementMembers()
        {
            var properties = _reflecter.GetEnumerableIndexablePropertyInfos(typeof(WithCollectionIndexes))
                .Where(p => p.Name == "ElementInt1")
                .SingleOrDefault();

            Assert.IsNull(properties);
        }

        [Test]
        public void GetEnumerableIndexablePropertyInfos_WhenEnumerableOfBytes_NoPropertiesAreReturned()
        {
            var properties = _reflecter.GetEnumerableIndexablePropertyInfos(typeof(WithEnumarbleBytes));

            Assert.AreEqual(0, properties.Count());
        }

        private class WithEnumarbleBytes
        {
            public byte[] Bytes1 { get; set; }

            public IEnumerable<byte> Bytes2 { get; set; }

            public IList<byte> Bytes3 { get; set; }

            public List<byte> Bytes4 { get; set; }

            public ICollection<byte> Bytes5 { get; set; }

            public Collection<byte> Bytes6 { get; set; }
        }

        private class WithNoEnumerableMembers
        {
            public int Int1 { get; set; }
        }

        private class Element
        {
            public int ElementInt1 { get; set; }
        }

        private class WithCollectionIndexes
        {
            public int Int1 { get; set; }

            public IEnumerable<Element> IEnumerable1 { get; set; }

            public IList<Element> IList1 { get; set; }
        }
    }
}