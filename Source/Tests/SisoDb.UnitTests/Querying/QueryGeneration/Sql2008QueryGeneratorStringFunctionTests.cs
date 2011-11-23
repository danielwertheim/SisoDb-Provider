using NUnit.Framework;
using SisoDb.Querying;
using SisoDb.Sql2008;

namespace SisoDb.UnitTests.Querying.QueryGeneration
{
    [TestFixture]
    public class Sql2008QueryGeneratorStringFunctionTests : QueryGeneratorStringFunctionTests
    {
        protected override IDbQueryGenerator GetQueryGenerator()
        {
            return new Sql2008QueryGenerator();
        }

        [Test]
        public override void GenerateQuery_for_Where_with_String_StartsWith_GeneratesCorrectQuery()
        {
            var sqlQuery = On_GenerateQuery_for_Where_with_String_StartsWith_GeneratesCorrectQuery();

            Assert.AreEqual(
                "select min(s.[Json]) [Json] from [MyClassStructure] s " +
                "inner join [MyClassIndexes] si on si.[StructureId] = s.[StructureId] " +
                "inner join [MyClassIndexes] mem0 on mem0.[StructureId] = s.[StructureId] and mem0.[MemberPath] = 'String1' " +
                "where (mem0.[StringValue] like @p0) " +
                "group by s.[StructureId];",
                sqlQuery.Sql);

            Assert.AreEqual("@p0", sqlQuery.Parameters[0].Name);
            Assert.AreEqual("Foo%", sqlQuery.Parameters[0].Value);
        }

        [Test]
        public override void GenerateQuery_for_Where_with_String_QxStartsWith_GeneratesCorrectQuery()
        {
            var sqlQuery = On_GenerateQuery_for_Where_with_String_QxStartsWith_GeneratesCorrectQuery();

            Assert.AreEqual(
                "select min(s.[Json]) [Json] from [MyClassStructure] s " +
                "inner join [MyClassIndexes] si on si.[StructureId] = s.[StructureId] " +
                "inner join [MyClassIndexes] mem0 on mem0.[StructureId] = s.[StructureId] and mem0.[MemberPath] = 'String1' " +
                "where (mem0.[StringValue] like @p0) " +
                "group by s.[StructureId];",
                sqlQuery.Sql);

            Assert.AreEqual("@p0", sqlQuery.Parameters[0].Name);
            Assert.AreEqual("Foo%", sqlQuery.Parameters[0].Value);
        }

        [Test]
        public override void GenerateQuery_for_Where_with_String_EndsWith_GeneratesCorrectQuery()
        {
            var sqlQuery = On_GenerateQuery_for_Where_with_String_EndsWith_GeneratesCorrectQuery();

            Assert.AreEqual(
                "select min(s.[Json]) [Json] from [MyClassStructure] s " +
                "inner join [MyClassIndexes] si on si.[StructureId] = s.[StructureId] " +
                "inner join [MyClassIndexes] mem0 on mem0.[StructureId] = s.[StructureId] and mem0.[MemberPath] = 'String1' " +
                "where (mem0.[StringValue] like @p0) " +
                "group by s.[StructureId];",
                sqlQuery.Sql);

            Assert.AreEqual("@p0", sqlQuery.Parameters[0].Name);
            Assert.AreEqual("%bar", sqlQuery.Parameters[0].Value);
        }

        [Test]
        public override void GenerateQuery_for_Where_with_String_QxEndsWith_GeneratesCorrectQuery()
        {
            var sqlQuery = On_GenerateQuery_for_Where_with_String_QxEndsWith_GeneratesCorrectQuery();

            Assert.AreEqual(
                "select min(s.[Json]) [Json] from [MyClassStructure] s " +
                "inner join [MyClassIndexes] si on si.[StructureId] = s.[StructureId] " +
                "inner join [MyClassIndexes] mem0 on mem0.[StructureId] = s.[StructureId] and mem0.[MemberPath] = 'String1' " +
                "where (mem0.[StringValue] like @p0) " +
                "group by s.[StructureId];",
                sqlQuery.Sql);

            Assert.AreEqual("@p0", sqlQuery.Parameters[0].Name);
            Assert.AreEqual("%bar", sqlQuery.Parameters[0].Value);
        }

        [Test]
        public override void GenerateQuery_for_Where_with_String_QxContains_GeneratesCorrectQuery()
        {
            var sqlQuery = On_GenerateQuery_for_Where_with_String_QxContains_GeneratesCorrectQuery();

            Assert.AreEqual(
                "select min(s.[Json]) [Json] from [MyClassStructure] s " +
                "inner join [MyClassIndexes] si on si.[StructureId] = s.[StructureId] " +
                "inner join [MyClassIndexes] mem0 on mem0.[StructureId] = s.[StructureId] and mem0.[MemberPath] = 'String1' " +
                "where (mem0.[StringValue] like @p0) " +
                "group by s.[StructureId];",
                sqlQuery.Sql);

            Assert.AreEqual("@p0", sqlQuery.Parameters[0].Name);
            Assert.AreEqual("%bar%", sqlQuery.Parameters[0].Value);
        }

        [Test]
        public override void GenerateQuery_for_Where_with_String_QxLike_GeneratesCorrectQuery()
        {
            var sqlQuery = On_GenerateQuery_for_Where_with_String_QxLike_GeneratesCorrectQuery();

            Assert.AreEqual(
                "select min(s.[Json]) [Json] from [MyClassStructure] s " +
                "inner join [MyClassIndexes] si on si.[StructureId] = s.[StructureId] " +
                "inner join [MyClassIndexes] mem0 on mem0.[StructureId] = s.[StructureId] and mem0.[MemberPath] = 'String1' " +
                "where (mem0.[StringValue] like @p0) " +
                "group by s.[StructureId];",
                sqlQuery.Sql);

            Assert.AreEqual("@p0", sqlQuery.Parameters[0].Name);
            Assert.AreEqual("Foo%bar", sqlQuery.Parameters[0].Value);
        }

        [Test]
        public override void GenerateQuery_for_Where_with_String_ToLower_GeneratesCorrectQuery()
        {
            var sqlQuery = On_GenerateQuery_for_Where_with_String_ToLower_GeneratesCorrectQuery();

            Assert.AreEqual(
                "select min(s.[Json]) [Json] from [MyClassStructure] s " +
                "inner join [MyClassIndexes] si on si.[StructureId] = s.[StructureId] " +
                "inner join [MyClassIndexes] mem0 on mem0.[StructureId] = s.[StructureId] and mem0.[MemberPath] = 'String1' " +
                "where (lower(mem0.[StringValue]) = @p0) " +
                "group by s.[StructureId];",
                sqlQuery.Sql);

            Assert.AreEqual("@p0", sqlQuery.Parameters[0].Name);
            Assert.AreEqual("foo", sqlQuery.Parameters[0].Value);
        }

        [Test]
        public override void GenerateQuery_for_Where_with_String_QxToLower_GeneratesCorrectQuery()
        {
            var sqlQuery = On_GenerateQuery_for_Where_with_String_QxToLower_GeneratesCorrectQuery();

            Assert.AreEqual(
                "select min(s.[Json]) [Json] from [MyClassStructure] s " +
                "inner join [MyClassIndexes] si on si.[StructureId] = s.[StructureId] " +
                "inner join [MyClassIndexes] mem0 on mem0.[StructureId] = s.[StructureId] and mem0.[MemberPath] = 'String1' " +
                "where (lower(mem0.[StringValue]) = @p0) " +
                "group by s.[StructureId];",
                sqlQuery.Sql);

            Assert.AreEqual("@p0", sqlQuery.Parameters[0].Name);
            Assert.AreEqual("foo", sqlQuery.Parameters[0].Value);
        }

        [Test]
        public override void GenerateQuery_for_Where_with_String_ToUpper_GeneratesCorrectQuery()
        {
            var sqlQuery = On_GenerateQuery_for_Where_with_String_ToUpper_GeneratesCorrectQuery();

            Assert.AreEqual(
                "select min(s.[Json]) [Json] from [MyClassStructure] s " +
                "inner join [MyClassIndexes] si on si.[StructureId] = s.[StructureId] " +
                "inner join [MyClassIndexes] mem0 on mem0.[StructureId] = s.[StructureId] and mem0.[MemberPath] = 'String1' " +
                "where (upper(mem0.[StringValue]) = @p0) " +
                "group by s.[StructureId];",
                sqlQuery.Sql);

            Assert.AreEqual("@p0", sqlQuery.Parameters[0].Name);
            Assert.AreEqual("FOO", sqlQuery.Parameters[0].Value);
        }

        [Test]
        public override void GenerateQuery_for_Where_with_String_QxToUpper_GeneratesCorrectQuery()
        {
            var sqlQuery = On_GenerateQuery_for_Where_with_String_QxToUpper_GeneratesCorrectQuery();

            Assert.AreEqual(
                "select min(s.[Json]) [Json] from [MyClassStructure] s " +
                "inner join [MyClassIndexes] si on si.[StructureId] = s.[StructureId] " +
                "inner join [MyClassIndexes] mem0 on mem0.[StructureId] = s.[StructureId] and mem0.[MemberPath] = 'String1' " +
                "where (upper(mem0.[StringValue]) = @p0) " +
                "group by s.[StructureId];",
                sqlQuery.Sql);

            Assert.AreEqual("@p0", sqlQuery.Parameters[0].Name);
            Assert.AreEqual("FOO", sqlQuery.Parameters[0].Value);
        }
    }
}