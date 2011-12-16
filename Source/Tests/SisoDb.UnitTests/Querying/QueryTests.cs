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
			var cmd = new Query(StructureSchemaStubFactory.Stub()) { Paging = null };

			Assert.IsFalse(cmd.HasPaging);
		}

		[Test]
		public void HasPaging_WhenPagingExists_ReturnsTrue()
		{
			var cmd = new Query(StructureSchemaStubFactory.Stub()) { Paging = new Paging(0, 1) };

			Assert.IsTrue(cmd.HasPaging);
		}

		[Test]
		public void HasWhere_WhenNullWhereExists_ReturnsFalse()
		{
			var cmd = new Query(StructureSchemaStubFactory.Stub()) { Where = null };

			Assert.IsFalse(cmd.HasWhere);
		}

		[Test]
		public void HasWhere_WhenEmptyWhereExists_ReturnsFalse()
		{
			var whereFake = new Mock<IParsedLambda>();
			whereFake.Setup(f => f.Nodes).Returns(GetFakeNodes(0));

			var cmd = new Query(StructureSchemaStubFactory.Stub()) { Where = whereFake.Object };

			Assert.IsFalse(cmd.HasWhere);
		}

		[Test]
		public void HasWhere_WhenWhereExists_ReturnsTrue()
		{
			var whereFake = new Mock<IParsedLambda>();
			whereFake.Setup(f => f.Nodes).Returns(GetFakeNodes(1));

			var cmd = new Query(StructureSchemaStubFactory.Stub()) { Where = whereFake.Object };

			Assert.IsTrue(cmd.HasWhere);
		}

		[Test]
		public void HasIncludes_WhenNullIncludes_ReturnsFalse()
		{
			var cmd = new Query(StructureSchemaStubFactory.Stub()) { Includes = null };

			Assert.IsFalse(cmd.HasIncludes);
		}

		[Test]
		public void HasIncludes_WhenEmptyIncludes_ReturnsFalse()
		{
			var cmd = new Query(StructureSchemaStubFactory.Stub());
			cmd.Includes.Clear();

			Assert.IsFalse(cmd.HasIncludes);
		}

		[Test]
		public void HasIncludes_WhenIncludesExists_ReturnsTrue()
		{
			var cmd = new Query(StructureSchemaStubFactory.Stub())
			{
				Includes = new List<IParsedLambda> { new Mock<IParsedLambda>().Object }
			};

			Assert.IsTrue(cmd.HasIncludes);
		}

		[Test]
		public void HasSortings_WhenNullSortingsExists_ReturnsFalse()
		{
			var cmd = new Query(StructureSchemaStubFactory.Stub()) { Sortings = null };

			Assert.IsFalse(cmd.HasSortings);
		}

		[Test]
		public void HasSortings_WhenEmptySortingsExists_ReturnsFalse()
		{
			var sortingsFake = new Mock<IParsedLambda>();
			sortingsFake.Setup(f => f.Nodes).Returns(GetFakeNodes(0));

			var cmd = new Query(StructureSchemaStubFactory.Stub()) { Sortings = sortingsFake.Object };

			Assert.IsFalse(cmd.HasSortings);
		}

		[Test]
		public void HasSortings_WhenSelectExists_ReturnsTrue()
		{
			var sortingsFake = new Mock<IParsedLambda>();
			sortingsFake.Setup(f => f.Nodes).Returns(GetFakeNodes(1));

			var cmd = new Query(StructureSchemaStubFactory.Stub()) { Sortings = sortingsFake.Object };

			Assert.IsTrue(cmd.HasSortings);
		}

		[Test]
		public void IsEmpty_WhenNoWhereSortingOrIncludes_ReturnsTrue()
		{
			var query = new Query(StructureSchemaStubFactory.Stub());

			Assert.IsTrue(query.IsEmpty);
		}

		[Test]
		public void IsEmpty_WhenWhereButNothingElse_ReturnsFalse()
		{
			var whereFake = new Mock<IParsedLambda>();
			whereFake.Setup(f => f.Nodes).Returns(GetFakeNodes(1));

			var query = new Query(StructureSchemaStubFactory.Stub()) { Where = whereFake.Object };

			Assert.IsFalse(query.IsEmpty);
		}

		[Test]
		public void IsEmpty_WhenSortingsButNothingElse_ReturnsFalse()
		{
			var sortingsFake = new Mock<IParsedLambda>();
			sortingsFake.Setup(f => f.Nodes).Returns(GetFakeNodes(1));

			var query = new Query(StructureSchemaStubFactory.Stub()) { Sortings = sortingsFake.Object };

			Assert.IsFalse(query.IsEmpty);
		}

		[Test]
		public void IsEmpty_WhenIncludesButNothingElse_ReturnsFalse()
		{
			var includesFake = new Mock<IParsedLambda>();
			includesFake.Setup(f => f.Nodes).Returns(GetFakeNodes(1));

			var query = new Query(StructureSchemaStubFactory.Stub()) { Includes = new []{ includesFake.Object } };

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