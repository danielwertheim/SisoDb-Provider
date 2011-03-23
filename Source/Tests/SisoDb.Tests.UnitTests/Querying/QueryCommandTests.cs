using System.Collections.Generic;
using System.Collections.ObjectModel;
using Moq;
using NUnit.Framework;
using SisoDb.Querying;
using SisoDb.Querying.Lambdas;
using SisoDb.Querying.Lambdas.Nodes;

namespace SisoDb.Tests.UnitTests.Querying
{
    [TestFixture]
    public class QueryCommandTests : UnitTestBase
    {
        [Test]
        public void HasPaging_WhenNullPagingExists_ReturnsFalse()
        {
            var cmd = new QueryCommand { Paging = null };

            Assert.IsFalse(cmd.HasPaging);
        }

        [Test]
        public void HasPaging_WhenPagingExists_ReturnsTrue()
        {
            var cmd = new QueryCommand { Paging = new Paging(0, 1)};

            Assert.IsTrue(cmd.HasPaging);
        }

        [Test]
        public void HasWhere_WhenNullWhereExists_ReturnsFalse()
        {
            var cmd = new QueryCommand { Where = null };

            Assert.IsFalse(cmd.HasWhere);
        }

        [Test]
        public void HasWhere_WhenEmptyWhereExists_ReturnsFalse()
        {
            var whereFake = new Mock<IParsedLambda>();
            whereFake.Setup(f => f.Nodes).Returns(GetFakeNodes(0));

            var cmd = new QueryCommand { Where = whereFake.Object };

            Assert.IsFalse(cmd.HasWhere);
        }

        [Test]
        public void HasWhere_WhenWhereExists_ReturnsTrue()
        {
            var whereFake = new Mock<IParsedLambda>();
            whereFake.Setup(f => f.Nodes).Returns(GetFakeNodes(1));

            var cmd = new QueryCommand { Where = whereFake.Object };

            Assert.IsTrue(cmd.HasWhere);
        }

        [Test]
        public void HasIncludes_WhenNullIncludes_ReturnsFalse()
        {
            var cmd = new QueryCommand { Includes = null };

            Assert.IsFalse(cmd.HasIncludes);
        }

        [Test]
        public void HasIncludes_WhenEmptyIncludes_ReturnsFalse()
        {
            var cmd = new QueryCommand();
            cmd.Includes.Clear();

            Assert.IsFalse(cmd.HasIncludes);
        }

        [Test]
        public void HasIncludes_WhenIncludesExists_ReturnsTrue()
        {
            var cmd = new QueryCommand
            {
                Includes = new List<IParsedLambda> { new Mock<IParsedLambda>().Object }
            };

            Assert.IsTrue(cmd.HasIncludes);
        }

        [Test]
        public void HasSortings_WhenNullSortingsExists_ReturnsFalse()
        {
            var cmd = new QueryCommand { Sortings = null };

            Assert.IsFalse(cmd.HasSortings);
        }

        [Test]
        public void HasSortings_WhenEmptySortingsExists_ReturnsFalse()
        {
            var sortingsFake = new Mock<IParsedLambda>();
            sortingsFake.Setup(f => f.Nodes).Returns(GetFakeNodes(0));

            var cmd = new QueryCommand { Sortings = sortingsFake.Object };
            
            Assert.IsFalse(cmd.HasSortings);
        }

        [Test]
        public void HasSortings_WhenSelectExists_ReturnsTrue()
        {
            var sortingsFake = new Mock<IParsedLambda>();
            sortingsFake.Setup(f => f.Nodes).Returns(GetFakeNodes(1));

            var cmd = new QueryCommand { Sortings = sortingsFake.Object };

            Assert.IsTrue(cmd.HasSortings);
        }

        private static ReadOnlyCollection<INode> GetFakeNodes(int count)
        {
            var list = new List<INode>();

            for(var c = 0; c < count; c++)
                list.Add(new Mock<INode>().Object);

            return new ReadOnlyCollection<INode>(list);
        }
    }
}