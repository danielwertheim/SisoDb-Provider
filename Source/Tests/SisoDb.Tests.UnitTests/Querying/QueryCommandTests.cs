using System.Collections.Generic;
using System.Collections.ObjectModel;
using Moq;
using NUnit.Framework;
using SisoDb.Lambdas;
using SisoDb.Lambdas.Nodes;
using SisoDb.Querying;

namespace SisoDb.Tests.UnitTests.Querying
{
    [TestFixture]
    public class QueryCommandTests : UnitTestBase
    {
        [Test]
        public void HasSelect_WhenNullSelectExists_ReturnsFalse()
        {
            var cmd = new QueryCommand { Select = null };

            Assert.IsFalse(cmd.HasSelect);
        }

        [Test]
        public void HasSelect_WhenEmptySelectExists_ReturnsFalse()
        {
            var selectFake = new Mock<IParsedLambda>();
            selectFake.Setup(f => f.Nodes).Returns(GetFakeNodes(0));

            var cmd = new QueryCommand { Select = selectFake.Object };

            Assert.IsFalse(cmd.HasSelect);
        }

        [Test]
        public void HasSelect_WhenSelectExists_ReturnsTrue()
        {
            var selectFake = new Mock<IParsedLambda>();
            selectFake.Setup(f => f.Nodes).Returns(GetFakeNodes(1));

            var cmd = new QueryCommand { Select = selectFake.Object };

            Assert.IsTrue(cmd.HasSelect);
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