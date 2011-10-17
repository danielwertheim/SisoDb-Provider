using System;
using NUnit.Framework;
using SisoDb.Querying;

namespace SisoDb.UnitTests.Querying
{
    [TestFixture]
    public class PagingTests : UnitTestBase
    {
        [Test]
        public void Ctor_WhenNegativePageIndex_ThrowsArgumentOutOfRangeException()
        {
            var ex = Assert.Throws<ArgumentException>(() => new Paging(-1, 1));

            Assert.AreEqual("pageIndex", ex.ParamName);
        }

        [Test]
        public void Ctor_WhenZeroPageSize_ThrowsArgumentOutOfRangeException()
        {
            var ex = Assert.Throws<ArgumentException>(() => new Paging(pageIndex: 0, pageSize: 0));

            Assert.AreEqual("pageSize", ex.ParamName);
        }
    }
}