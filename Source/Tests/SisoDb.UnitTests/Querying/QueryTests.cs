using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using SisoDb.Querying;
using SisoDb.Querying.Lambdas;
using SisoDb.Querying.Lambdas.Nodes;
using SisoDb.UnitTests.TestFactories;

namespace SisoDb.UnitTests.Querying
{
	[TestFixture]
	public class QueryTests : UnitTestBase
	{
		[Test]
		public void HasPaging_WhenNullPagingExists_ReturnsFalse()
		{
			var cmd = new Query(StructureSchemaTestFactory.Stub()) { Paging = null };

			Assert.IsFalse(cmd.HasPaging);
		}

		[Test]
		public void HasPaging_WhenPagingExists_ReturnsTrue()
		{
			var cmd = new Query(StructureSchemaTestFactory.Stub()) { Paging = new Paging(0, 1) };

			Assert.IsTrue(cmd.HasPaging);
		}

		[Test]
		public void HasWhere_WhenNullWhereExists_ReturnsFalse()
		{
			var cmd = new Query(StructureSchemaTestFactory.Stub()) { Where = null };

			Assert.IsFalse(cmd.HasWhere);
		}

		[Test]
		public void HasWhere_WhenEmptyWhereExists_ReturnsFalse()
		{
			var whereFake = new Mock<IParsedLambda>();
			whereFake.Setup(f => f.Nodes).Returns(GetFakeNodes(0));

			var cmd = new Query(StructureSchemaTestFactory.Stub()) { Where = whereFake.Object };

			Assert.IsFalse(cmd.HasWhere);
		}

		[Test]
		public void HasWhere_WhenWhereExists_ReturnsTrue()
		{
			var whereFake = new Mock<IParsedLambda>();
			whereFake.Setup(f => f.Nodes).Returns(GetFakeNodes(1));

			var cmd = new Query(StructureSchemaTestFactory.Stub()) { Where = whereFake.Object };

			Assert.IsTrue(cmd.HasWhere);
		}

		[Test]
		public void HasSortings_WhenNullSortingsExists_ReturnsFalse()
		{
			var cmd = new Query(StructureSchemaTestFactory.Stub()) { Sortings = null };

			Assert.IsFalse(cmd.HasSortings);
		}

		[Test]
		public void HasSortings_WhenEmptySortingsExists_ReturnsFalse()
		{
			var sortingsFake = new Mock<IParsedLambda>();
			sortingsFake.Setup(f => f.Nodes).Returns(GetFakeNodes(0));

			var cmd = new Query(StructureSchemaTestFactory.Stub()) { Sortings = sortingsFake.Object };

			Assert.IsFalse(cmd.HasSortings);
		}

		[Test]
		public void HasSortings_WhenSelectExists_ReturnsTrue()
		{
			var sortingsFake = new Mock<IParsedLambda>();
			sortingsFake.Setup(f => f.Nodes).Returns(GetFakeNodes(1));

			var cmd = new Query(StructureSchemaTestFactory.Stub()) { Sortings = sortingsFake.Object };

			Assert.IsTrue(cmd.HasSortings);
		}

		[Test]
		public void IsEmpty_WhenNoWhereSortingOrPaging_ReturnsTrue()
		{
			var query = new Query(StructureSchemaTestFactory.Stub());

			Assert.IsTrue(query.IsEmpty);
		}

		[Test]
		public void IsEmpty_WhenWhereButNothingElse_ReturnsFalse()
		{
			var whereFake = new Mock<IParsedLambda>();
			whereFake.Setup(f => f.Nodes).Returns(GetFakeNodes(1));

			var query = new Query(StructureSchemaTestFactory.Stub()) { Where = whereFake.Object };

			Assert.IsFalse(query.IsEmpty);
		}

		[Test]
		public void IsEmpty_WhenSortingsButNothingElse_ReturnsFalse()
		{
			var sortingsFake = new Mock<IParsedLambda>();
			sortingsFake.Setup(f => f.Nodes).Returns(GetFakeNodes(1));

			var query = new Query(StructureSchemaTestFactory.Stub()) { Sortings = sortingsFake.Object };

			Assert.IsFalse(query.IsEmpty);
		}

		private static INode[] GetFakeNodes(int count)
		{
			var list = new List<INode>();

			for (var c = 0; c < count; c++)
				list.Add(new Mock<INode>().Object);

			return list.ToArray();
		}
	}
}