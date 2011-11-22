using NUnit.Framework;
using SisoDb.Sql2008;
using SisoDb.Querying;

namespace SisoDb.UnitTests.Querying.QueryGeneration
{
    [TestFixture]
    public class Sql2008QueryGeneratorTests : QueryGeneratorTests
    {
        protected override IDbQueryGenerator GetQueryGenerator()
        {
            return new Sql2008QueryGenerator();
        }

        [Test]
        public override void GenerateQuery_WithWhere_GeneratesCorrectQuery()
        {
            var sqlQuery = On_GenerateQuery_WithWhere_GeneratesCorrectQuery();

            Assert.AreEqual(
                "select min(s.[Json]) [Json] from [MyClassStructure] s " +
                "inner join [MyClassIndexes] si on si.[StructureId] = s.[StructureId] " +
                "inner join [MyClassIndexes] mem0 on mem0.[StructureId] = s.[StructureId] and mem0.[MemberPath] = 'Int1' " +
                "where (mem0.[IntegerValue] = @p0) " +
                "group by s.[StructureId];",
                sqlQuery.Sql);

            Assert.AreEqual("@p0", sqlQuery.Parameters[0].Name);
            Assert.AreEqual(42, sqlQuery.Parameters[0].Value);
        }

        [Test]
        public override void GenerateQuery_WithSorting_GeneratesCorrectQuery()
        {
            var sqlQuery = On_GenerateQuery_WithSorting_GeneratesCorrectQuery();

            Assert.AreEqual(
                "select min(s.[Json]) [Json] from [MyClassStructure] s " +
                "inner join [MyClassIndexes] si on si.[StructureId] = s.[StructureId] " +
                "inner join [MyClassIndexes] mem0 on mem0.[StructureId] = s.[StructureId] and mem0.[MemberPath] = 'Int1' " +
                "group by s.[StructureId] " +
                "order by min(mem0.[IntegerValue]) Asc;",
                sqlQuery.Sql);
        }

        [Test]
        public override void GenerateQuery_WithWhereAndSorting_GeneratesCorrectQuery()
        {
            var sqlQuery = On_GenerateQuery_WithWhereAndSorting_GeneratesCorrectQuery();

            Assert.AreEqual(
                "select min(s.[Json]) [Json] from [MyClassStructure] s " +
                "inner join [MyClassIndexes] si on si.[StructureId] = s.[StructureId] " +
                "inner join [MyClassIndexes] mem0 on mem0.[StructureId] = s.[StructureId] and mem0.[MemberPath] = 'Int1' " +
                "where (mem0.[IntegerValue] = @p0) " +
                "group by s.[StructureId] " +
                "order by min(mem0.[IntegerValue]) Asc;",
                sqlQuery.Sql);

            Assert.AreEqual("@p0", sqlQuery.Parameters[0].Name);
            Assert.AreEqual(42, sqlQuery.Parameters[0].Value);
        }

        [Test]
        public override void GenerateQuery_WithTake_GeneratesCorrectQuery()
        {
            var sqlQuery = On_GenerateQuery_WithTake_GeneratesCorrectQuery();

            Assert.AreEqual(
                "select top(11) min(s.[Json]) [Json] from [MyClassStructure] s " +
                "inner join [MyClassIndexes] si on si.[StructureId] = s.[StructureId];",
                sqlQuery.Sql);
        }

        [Test]
        public override void GenerateQuery_WithTakeAndSorting_GeneratesCorrectQuery()
        {
            var sqlQuery = On_GenerateQuery_WithTakeAndSorting_GeneratesCorrectQuery();

            Assert.AreEqual(
                "select top(11) min(s.[Json]) [Json] from [MyClassStructure] s " +
                "inner join [MyClassIndexes] si on si.[StructureId] = s.[StructureId] " +
                "inner join [MyClassIndexes] mem0 on mem0.[StructureId] = s.[StructureId] and mem0.[MemberPath] = 'Int1' " +
                "group by s.[StructureId] " +
                "order by min(mem0.[IntegerValue]) Asc;",
                sqlQuery.Sql);
        }

        [Test]
        public override void GenerateQuery_WithTakeAndWhereAndSorting_GeneratesCorrectQuery()
        {
            var sqlQuery = On_GenerateQuery_WithTakeAndWhereAndSorting_GeneratesCorrectQuery();

            Assert.AreEqual(
                "select top(11) min(s.[Json]) [Json] from [MyClassStructure] s " +
                "inner join [MyClassIndexes] si on si.[StructureId] = s.[StructureId] " +
                "inner join [MyClassIndexes] mem0 on mem0.[StructureId] = s.[StructureId] and mem0.[MemberPath] = 'Int1' " +
                "where (mem0.[IntegerValue] = @p0) " +
                "group by s.[StructureId] " +
                "order by min(mem0.[IntegerValue]) Asc;",
                sqlQuery.Sql);

            Assert.AreEqual("@p0", sqlQuery.Parameters[0].Name);
            Assert.AreEqual(42, sqlQuery.Parameters[0].Value);
        }

        [Test]
        public override void GenerateQuery_WithPagingAndWhereAndSorting_GeneratesCorrectQuery()
        {
            var sqlQuery = On_GenerateQuery_WithPagingAndWhereAndSorting_GeneratesCorrectQuery();

            Assert.AreEqual("with pagedRs as (select min(s.[Json]) [Json], row_number() over (order by min(mem0.[IntegerValue]) Asc) as RowNum " +
                "from [MyClassStructure] s " +
                "inner join [MyClassIndexes] si on si.[StructureId] = s.[StructureId] " +
                "inner join [MyClassIndexes] mem0 on mem0.[StructureId] = s.[StructureId] and mem0.[MemberPath] = 'Int1' " +
                "where (mem0.[IntegerValue] = @p0) " +
                "group by s.[StructureId]) " +
                "select pagedRs.[Json] from pagedRs where pagedRs.[RowNum] between @pagingFrom and @pagingTo;",
                sqlQuery.Sql);

            Assert.AreEqual("@p0", sqlQuery.Parameters[0].Name);
            Assert.AreEqual(42, sqlQuery.Parameters[0].Value);

            Assert.AreEqual("@pagingFrom", sqlQuery.Parameters[1].Name);
            Assert.AreEqual(1, sqlQuery.Parameters[1].Value);
            
            Assert.AreEqual("@pagingTo", sqlQuery.Parameters[2].Name);
            Assert.AreEqual(10, sqlQuery.Parameters[2].Value);
        }

        [Test]
        public override void GenerateQuery_WithExplicitSortingOnTwoDifferentMemberTypesAndSorting_GeneratesCorrectQuery()
        {
            var sqlQuery = On_GenerateQuery_WithExplicitSortingOnTwoDifferentMemberTypesAndSorting_GeneratesCorrectQuery();

            Assert.AreEqual(
                "select min(s.[Json]) [Json] from [MyClassStructure] s " +
                "inner join [MyClassIndexes] si on si.[StructureId] = s.[StructureId] " +
                "inner join [MyClassIndexes] mem0 on mem0.[StructureId] = s.[StructureId] and mem0.[MemberPath] = 'Int1' " +
                "inner join [MyClassIndexes] mem1 on mem1.[StructureId] = s.[StructureId] and mem1.[MemberPath] = 'String1' " +
                "group by s.[StructureId] " +
                "order by min(mem0.[IntegerValue]) Asc, min(mem1.[StringValue]) Desc;",
                sqlQuery.Sql);
        }

        [Test]
        public override void GenerateQuery_WithSortingOnTwoDifferentMemberOfSameType_GeneratesCorrectQuery()
        {
            var sqlQuery = On_GenerateQuery_WithSortingOnTwoDifferentMemberOfSameType_GeneratesCorrectQuery();

            Assert.AreEqual(
                "select min(s.[Json]) [Json] from [MyClassStructure] s " +
                "inner join [MyClassIndexes] si on si.[StructureId] = s.[StructureId] " +
                "inner join [MyClassIndexes] mem0 on mem0.[StructureId] = s.[StructureId] and mem0.[MemberPath] = 'Int1' " +
                "inner join [MyClassIndexes] mem1 on mem1.[StructureId] = s.[StructureId] and mem1.[MemberPath] = 'Int2' " +
                "group by s.[StructureId] " +
                "order by min(mem0.[IntegerValue]) Asc, min(mem1.[IntegerValue]) Asc;",
                sqlQuery.Sql);
        }
    }
}