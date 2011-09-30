using System;
using NUnit.Framework;
using SisoDb.Querying;

namespace SisoDb.Tests.UnitTests.Querying
{
    [TestFixture]
    public class PagingTests : UnitTestBase
    {
        [Test]
        public void CTor_WhenNegativePageIndex_ThrowsArgumentOutOfRangeException()
        {
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => new Paging(-1, 1));

            Assert.AreEqual("value '-1' is not >= limit '0'.\r\nParameter name: pageIndex", ex.Message);
        }

        [Test]
        public void CTor_WhenZeroPageSize_ThrowsArgumentOutOfRangeException()
        {
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => new Paging(pageIndex: 0, pageSize: 0));

            Assert.AreEqual("value '0' is <= limit '0'.\r\nParameter name: pageSize", ex.Message);
        }
    }
}