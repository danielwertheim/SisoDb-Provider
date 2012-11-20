using NUnit.Framework;
using SisoDb.Querying;
using SisoDb.Querying.Sql;
using SisoDb.SqlCe4;

namespace SisoDb.UnitTests.Querying.QueryGeneration
{
    [TestFixture]
    public class SqlCe4QueryGeneratorStringFunctionTests : QueryGeneratorStringFunctionTests
    {
        protected override IDbQueryGenerator GetQueryGenerator()
        {
            return new SqlCe4QueryGenerator(new SqlCe4Statements(), new SqlExpressionBuilder(() => new SqlCe4WhereCriteriaBuilder()));
        }

        [Test]
        public override void GenerateQuery_for_Where_with_String_StartsWith_on_Nullable_ToString_GeneratesCorrectQuery()
        {
            var sqlQuery = On_GenerateQuery_for_Where_with_String_StartsWith_on_Nullable_ToString_GeneratesCorrectQuery();

            Assert.AreEqual(
                "select s.[Json] from (select s.[StructureId] from [MyClassStructure] s left join [MyClassIntegers] mem0 on mem0.[StructureId] = s.[StructureId] and mem0.[MemberPath] = 'NullableInt1' where (mem0.[StringValue] like @p0) group by s.[StructureId]) rs inner join [MyClassStructure] s on s.[StructureId] = rs.[StructureId];",
                sqlQuery.Sql);

            Assert.AreEqual("@p0", sqlQuery.Parameters[0].Name);
            Assert.AreEqual("42%", sqlQuery.Parameters[0].Value);
        }

        [Test]
        public override void GenerateQuery_for_Where_with_String_Equals_and_StartsWith_on_String_GeneratesCorrectQuery()
        {
            var sqlQuery = On_GenerateQuery_for_Where_with_String_Equals_and_StartsWith_on_String_GeneratesCorrectQuery();

            Assert.AreEqual(
                "select s.[Json] from (select s.[StructureId] from [MyClassStructure] s left join [MyClassStrings] mem0 on mem0.[StructureId] = s.[StructureId] and mem0.[MemberPath] = 'String1' where ((mem0.[Value] = @p0) or (mem0.[Value] like @p1)) group by s.[StructureId]) rs inner join [MyClassStructure] s on s.[StructureId] = rs.[StructureId];",
                sqlQuery.Sql);

            Assert.AreEqual("@p0", sqlQuery.Parameters[0].Name);
            Assert.AreEqual("Foo", sqlQuery.Parameters[0].Value);

            Assert.AreEqual("@p1", sqlQuery.Parameters[1].Name);
            Assert.AreEqual("42%", sqlQuery.Parameters[1].Value);
        }

        [Test]
        public override void GenerateQuery_for_Where_with_String_EndsWith_on_Nullable_ToString_GeneratesCorrectQuery()
        {
            var sqlQuery = On_GenerateQuery_for_Where_with_String_EndsWith_on_Nullable_ToString_GeneratesCorrectQuery();

            Assert.AreEqual(
                "select s.[Json] from (select s.[StructureId] from [MyClassStructure] s left join [MyClassIntegers] mem0 on mem0.[StructureId] = s.[StructureId] and mem0.[MemberPath] = 'NullableInt1' where (mem0.[StringValue] like @p0) group by s.[StructureId]) rs inner join [MyClassStructure] s on s.[StructureId] = rs.[StructureId];",
                sqlQuery.Sql);

            Assert.AreEqual("@p0", sqlQuery.Parameters[0].Name);
            Assert.AreEqual("%42", sqlQuery.Parameters[0].Value);
        }

        [Test]
        public override void GenerateQuery_for_Where_with_String_Equals_and_EndsWith_on_String_GeneratesCorrectQuery()
        {
            var sqlQuery = On_GenerateQuery_for_Where_with_String_Equals_and_EndsWith_on_String_GeneratesCorrectQuery();

            Assert.AreEqual(
                "select s.[Json] from (select s.[StructureId] from [MyClassStructure] s left join [MyClassStrings] mem0 on mem0.[StructureId] = s.[StructureId] and mem0.[MemberPath] = 'String1' where ((mem0.[Value] = @p0) or (mem0.[Value] like @p1)) group by s.[StructureId]) rs inner join [MyClassStructure] s on s.[StructureId] = rs.[StructureId];",
                sqlQuery.Sql);

            Assert.AreEqual("@p0", sqlQuery.Parameters[0].Name);
            Assert.AreEqual("Foo", sqlQuery.Parameters[0].Value);

            Assert.AreEqual("@p1", sqlQuery.Parameters[1].Name);
            Assert.AreEqual("%42", sqlQuery.Parameters[1].Value);
        }

        [Test]
        public override void GenerateQuery_for_Where_with_String_StartsWith_GeneratesCorrectQuery()
        {
            var sqlQuery = On_GenerateQuery_for_Where_with_String_StartsWith_GeneratesCorrectQuery();

            Assert.AreEqual(
                "select s.[Json] from (select s.[StructureId] from [MyClassStructure] s left join [MyClassStrings] mem0 on mem0.[StructureId] = s.[StructureId] and mem0.[MemberPath] = 'String1' where (mem0.[Value] like @p0) group by s.[StructureId]) rs inner join [MyClassStructure] s on s.[StructureId] = rs.[StructureId];",
                sqlQuery.Sql);

            Assert.AreEqual("@p0", sqlQuery.Parameters[0].Name);
            Assert.AreEqual("Foo%", sqlQuery.Parameters[0].Value);
        }

        [Test]
        public override void GenerateQuery_for_Where_with_String_QxStartsWith_GeneratesCorrectQuery()
        {
            var sqlQuery = On_GenerateQuery_for_Where_with_String_QxStartsWith_GeneratesCorrectQuery();

            Assert.AreEqual(
                "select s.[Json] from (select s.[StructureId] from [MyClassStructure] s left join [MyClassStrings] mem0 on mem0.[StructureId] = s.[StructureId] and mem0.[MemberPath] = 'String1' where (mem0.[Value] like @p0) group by s.[StructureId]) rs inner join [MyClassStructure] s on s.[StructureId] = rs.[StructureId];",
                sqlQuery.Sql);

            Assert.AreEqual("@p0", sqlQuery.Parameters[0].Name);
            Assert.AreEqual("Foo%", sqlQuery.Parameters[0].Value);
        }

        [Test]
        public override void GenerateQuery_for_Where_with_String_EndsWith_GeneratesCorrectQuery()
        {
            var sqlQuery = On_GenerateQuery_for_Where_with_String_EndsWith_GeneratesCorrectQuery();

            Assert.AreEqual(
                "select s.[Json] from (select s.[StructureId] from [MyClassStructure] s left join [MyClassStrings] mem0 on mem0.[StructureId] = s.[StructureId] and mem0.[MemberPath] = 'String1' where (mem0.[Value] like @p0) group by s.[StructureId]) rs inner join [MyClassStructure] s on s.[StructureId] = rs.[StructureId];",
                sqlQuery.Sql);

            Assert.AreEqual("@p0", sqlQuery.Parameters[0].Name);
            Assert.AreEqual("%bar", sqlQuery.Parameters[0].Value);
        }

        [Test]
        public override void GenerateQuery_for_Where_with_String_QxEndsWith_GeneratesCorrectQuery()
        {
            var sqlQuery = On_GenerateQuery_for_Where_with_String_QxEndsWith_GeneratesCorrectQuery();

            Assert.AreEqual(
                "select s.[Json] from (select s.[StructureId] from [MyClassStructure] s left join [MyClassStrings] mem0 on mem0.[StructureId] = s.[StructureId] and mem0.[MemberPath] = 'String1' where (mem0.[Value] like @p0) group by s.[StructureId]) rs inner join [MyClassStructure] s on s.[StructureId] = rs.[StructureId];",
                sqlQuery.Sql);

            Assert.AreEqual("@p0", sqlQuery.Parameters[0].Name);
            Assert.AreEqual("%bar", sqlQuery.Parameters[0].Value);
        }

        [Test]
        public override void GenerateQuery_for_Where_with_String_QxContains_GeneratesCorrectQuery()
        {
            var sqlQuery = On_GenerateQuery_for_Where_with_String_QxContains_GeneratesCorrectQuery();

            Assert.AreEqual(
                "select s.[Json] from (select s.[StructureId] from [MyClassStructure] s left join [MyClassStrings] mem0 on mem0.[StructureId] = s.[StructureId] and mem0.[MemberPath] = 'String1' where (mem0.[Value] like @p0) group by s.[StructureId]) rs inner join [MyClassStructure] s on s.[StructureId] = rs.[StructureId];",
                sqlQuery.Sql);

            Assert.AreEqual("@p0", sqlQuery.Parameters[0].Name);
            Assert.AreEqual("%bar%", sqlQuery.Parameters[0].Value);
        }

        [Test]
        public override void GenerateQuery_for_Where_with_String_Contains_GeneratesCorrectQuery()
        {
            var sqlQuery = On_GenerateQuery_for_Where_with_String_Contains_GeneratesCorrectQuery();

            Assert.AreEqual(
                "select s.[Json] from (select s.[StructureId] from [MyClassStructure] s left join [MyClassStrings] mem0 on mem0.[StructureId] = s.[StructureId] and mem0.[MemberPath] = 'String1' where (mem0.[Value] like @p0) group by s.[StructureId]) rs inner join [MyClassStructure] s on s.[StructureId] = rs.[StructureId];",
                sqlQuery.Sql);

            Assert.AreEqual("@p0", sqlQuery.Parameters[0].Name);
            Assert.AreEqual("%bar%", sqlQuery.Parameters[0].Value);
        }

        [Test]
        public override void GenerateQuery_for_Where_with_String_QxLike_GeneratesCorrectQuery()
        {
            var sqlQuery = On_GenerateQuery_for_Where_with_String_QxLike_GeneratesCorrectQuery();

            Assert.AreEqual(
                "select s.[Json] from (select s.[StructureId] from [MyClassStructure] s left join [MyClassStrings] mem0 on mem0.[StructureId] = s.[StructureId] and mem0.[MemberPath] = 'String1' where (mem0.[Value] like @p0) group by s.[StructureId]) rs inner join [MyClassStructure] s on s.[StructureId] = rs.[StructureId];",
                sqlQuery.Sql);

            Assert.AreEqual("@p0", sqlQuery.Parameters[0].Name);
            Assert.AreEqual("Foo%bar", sqlQuery.Parameters[0].Value);
        }

        [Test]
        public override void GenerateQuery_for_Where_with_String_ToLower_GeneratesCorrectQuery()
        {
            var sqlQuery = On_GenerateQuery_for_Where_with_String_ToLower_GeneratesCorrectQuery();

            Assert.AreEqual(
                "select s.[Json] from (select s.[StructureId] from [MyClassStructure] s left join [MyClassStrings] mem0 on mem0.[StructureId] = s.[StructureId] and mem0.[MemberPath] = 'String1' where (lower(mem0.[Value]) = @p0) group by s.[StructureId]) rs inner join [MyClassStructure] s on s.[StructureId] = rs.[StructureId];",
                sqlQuery.Sql);

            Assert.AreEqual("@p0", sqlQuery.Parameters[0].Name);
            Assert.AreEqual("foo", sqlQuery.Parameters[0].Value);
        }

        [Test]
        public override void GenerateQuery_for_Where_with_String_QxToLower_GeneratesCorrectQuery()
        {
            var sqlQuery = On_GenerateQuery_for_Where_with_String_QxToLower_GeneratesCorrectQuery();

            Assert.AreEqual(
                "select s.[Json] from (select s.[StructureId] from [MyClassStructure] s left join [MyClassStrings] mem0 on mem0.[StructureId] = s.[StructureId] and mem0.[MemberPath] = 'String1' where (lower(mem0.[Value]) = @p0) group by s.[StructureId]) rs inner join [MyClassStructure] s on s.[StructureId] = rs.[StructureId];",
                sqlQuery.Sql);

            Assert.AreEqual("@p0", sqlQuery.Parameters[0].Name);
            Assert.AreEqual("foo", sqlQuery.Parameters[0].Value);
        }

        [Test]
        public override void GenerateQuery_for_Where_with_String_ToUpper_GeneratesCorrectQuery()
        {
            var sqlQuery = On_GenerateQuery_for_Where_with_String_ToUpper_GeneratesCorrectQuery();

            Assert.AreEqual(
                "select s.[Json] from (select s.[StructureId] from [MyClassStructure] s left join [MyClassStrings] mem0 on mem0.[StructureId] = s.[StructureId] and mem0.[MemberPath] = 'String1' where (upper(mem0.[Value]) = @p0) group by s.[StructureId]) rs inner join [MyClassStructure] s on s.[StructureId] = rs.[StructureId];",
                sqlQuery.Sql);

            Assert.AreEqual("@p0", sqlQuery.Parameters[0].Name);
            Assert.AreEqual("FOO", sqlQuery.Parameters[0].Value);
        }

        [Test]
        public override void GenerateQuery_for_Where_with_String_QxToUpper_GeneratesCorrectQuery()
        {
            var sqlQuery = On_GenerateQuery_for_Where_with_String_QxToUpper_GeneratesCorrectQuery();

            Assert.AreEqual(
                "select s.[Json] from (select s.[StructureId] from [MyClassStructure] s left join [MyClassStrings] mem0 on mem0.[StructureId] = s.[StructureId] and mem0.[MemberPath] = 'String1' where (upper(mem0.[Value]) = @p0) group by s.[StructureId]) rs inner join [MyClassStructure] s on s.[StructureId] = rs.[StructureId];",
                sqlQuery.Sql);

            Assert.AreEqual("@p0", sqlQuery.Parameters[0].Name);
            Assert.AreEqual("FOO", sqlQuery.Parameters[0].Value);
        }

        [Test]
        public override void GenerateQuery_for_Where_with_String_QxIsExactly_GeneratesCorrectQuery()
        {
            var sqlQuery = On_GenerateQuery_for_Where_with_String_QxIsExactly_GeneratesCorrectQuery();

            Assert.AreEqual(
                "select s.[Json] from (select s.[StructureId] from [MyClassStructure] s left join [MyClassStrings] mem0 on mem0.[StructureId] = s.[StructureId] and mem0.[MemberPath] = 'String1' where ((mem0.[Value] = @p0) and (cast(mem0.[Value] as varbinary(300)) = cast(@p0 as varbinary(300)))) group by s.[StructureId]) rs inner join [MyClassStructure] s on s.[StructureId] = rs.[StructureId];",
                sqlQuery.Sql);

            Assert.AreEqual(1, sqlQuery.Parameters.Length);
            Assert.AreEqual("@p0", sqlQuery.Parameters[0].Name);
            Assert.AreEqual("Foo", sqlQuery.Parameters[0].Value);
        }
    }
}