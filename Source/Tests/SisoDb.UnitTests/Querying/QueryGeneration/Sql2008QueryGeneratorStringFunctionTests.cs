using NUnit.Framework;
using SisoDb.Querying;
using SisoDb.Querying.Sql;
using SisoDb.Sql2008;

namespace SisoDb.UnitTests.Querying.QueryGeneration
{
    [TestFixture]
    public class Sql2008QueryGeneratorStringFunctionTests : QueryGeneratorStringFunctionTests
    {
        protected override IDbQueryGenerator GetQueryGenerator()
        {
            return new Sql2008QueryGenerator(new Sql2008Statements(), new SqlExpressionBuilder(() => new SqlWhereCriteriaBuilder()));
        }

        [Test]
        public override void GenerateQuery_for_Where_with_String_StartsWith_on_Nullable_ToString_GeneratesCorrectQuery()
        {
            var sqlQuery = On_GenerateQuery_for_Where_with_String_StartsWith_on_Nullable_ToString_GeneratesCorrectQuery();

            SqlApprovals.Verify(sqlQuery.Sql);
            Assert.AreEqual("@p0", sqlQuery.Parameters[0].Name);
            Assert.AreEqual("42%", sqlQuery.Parameters[0].Value);
        }

        [Test]
        public override void GenerateQuery_for_Where_with_String_Equals_and_StartsWith_on_String_GeneratesCorrectQuery()
        {
            var sqlQuery = On_GenerateQuery_for_Where_with_String_Equals_and_StartsWith_on_String_GeneratesCorrectQuery();

            SqlApprovals.Verify(sqlQuery.Sql);

            Assert.AreEqual("@p0", sqlQuery.Parameters[0].Name);
            Assert.AreEqual("Foo", sqlQuery.Parameters[0].Value);

            Assert.AreEqual("@p1", sqlQuery.Parameters[1].Name);
            Assert.AreEqual("42%", sqlQuery.Parameters[1].Value);
        }

        [Test]
        public override void GenerateQuery_for_Where_with_String_EndsWith_on_Nullable_ToString_GeneratesCorrectQuery()
        {
            var sqlQuery = On_GenerateQuery_for_Where_with_String_EndsWith_on_Nullable_ToString_GeneratesCorrectQuery();

            SqlApprovals.Verify(sqlQuery.Sql);
            Assert.AreEqual("@p0", sqlQuery.Parameters[0].Name);
            Assert.AreEqual("%42", sqlQuery.Parameters[0].Value);
        }

        [Test]
        public override void GenerateQuery_for_Where_with_String_Equals_and_EndsWith_on_String_GeneratesCorrectQuery()
        {
            var sqlQuery = On_GenerateQuery_for_Where_with_String_Equals_and_EndsWith_on_String_GeneratesCorrectQuery();

            SqlApprovals.Verify(sqlQuery.Sql);

            Assert.AreEqual("@p0", sqlQuery.Parameters[0].Name);
            Assert.AreEqual("Foo", sqlQuery.Parameters[0].Value);

            Assert.AreEqual("@p1", sqlQuery.Parameters[1].Name);
            Assert.AreEqual("%42", sqlQuery.Parameters[1].Value);
        }

        [Test]
        public override void GenerateQuery_for_Where_with_String_StartsWith_GeneratesCorrectQuery()
        {
            var sqlQuery = On_GenerateQuery_for_Where_with_String_StartsWith_GeneratesCorrectQuery();

            SqlApprovals.Verify(sqlQuery.Sql);
            Assert.AreEqual("@p0", sqlQuery.Parameters[0].Name);
            Assert.AreEqual("Foo%", sqlQuery.Parameters[0].Value);
        }

        [Test]
        public override void GenerateQuery_for_Where_with_String_QxStartsWith_GeneratesCorrectQuery()
        {
            var sqlQuery = On_GenerateQuery_for_Where_with_String_QxStartsWith_GeneratesCorrectQuery();

            SqlApprovals.Verify(sqlQuery.Sql);
            Assert.AreEqual("@p0", sqlQuery.Parameters[0].Name);
            Assert.AreEqual("Foo%", sqlQuery.Parameters[0].Value);
        }

        [Test]
        public override void GenerateQuery_for_Where_with_String_EndsWith_GeneratesCorrectQuery()
        {
            var sqlQuery = On_GenerateQuery_for_Where_with_String_EndsWith_GeneratesCorrectQuery();

            SqlApprovals.Verify(sqlQuery.Sql);
            Assert.AreEqual("@p0", sqlQuery.Parameters[0].Name);
            Assert.AreEqual("%bar", sqlQuery.Parameters[0].Value);
        }

        [Test]
        public override void GenerateQuery_for_Where_with_String_QxEndsWith_GeneratesCorrectQuery()
        {
            var sqlQuery = On_GenerateQuery_for_Where_with_String_QxEndsWith_GeneratesCorrectQuery();

            SqlApprovals.Verify(sqlQuery.Sql);
            Assert.AreEqual("@p0", sqlQuery.Parameters[0].Name);
            Assert.AreEqual("%bar", sqlQuery.Parameters[0].Value);
        }

        [Test]
        public override void GenerateQuery_for_Where_with_String_QxContains_GeneratesCorrectQuery()
        {
            var sqlQuery = On_GenerateQuery_for_Where_with_String_QxContains_GeneratesCorrectQuery();

            SqlApprovals.Verify(sqlQuery.Sql);
            Assert.AreEqual("@p0", sqlQuery.Parameters[0].Name);
            Assert.AreEqual("%bar%", sqlQuery.Parameters[0].Value);
        }

        [Test]
        public override void GenerateQuery_for_Where_with_String_Contains_GeneratesCorrectQuery()
        {
            var sqlQuery = On_GenerateQuery_for_Where_with_String_Contains_GeneratesCorrectQuery();

            SqlApprovals.Verify(sqlQuery.Sql);
            Assert.AreEqual("@p0", sqlQuery.Parameters[0].Name);
            Assert.AreEqual("%bar%", sqlQuery.Parameters[0].Value);
        }

        [Test]
        public override void GenerateQuery_for_Where_with_String_QxLike_GeneratesCorrectQuery()
        {
            var sqlQuery = On_GenerateQuery_for_Where_with_String_QxLike_GeneratesCorrectQuery();

            SqlApprovals.Verify(sqlQuery.Sql);
            Assert.AreEqual("@p0", sqlQuery.Parameters[0].Name);
            Assert.AreEqual("Foo%bar", sqlQuery.Parameters[0].Value);
        }

        [Test]
        public override void GenerateQuery_for_Where_with_String_ToLower_GeneratesCorrectQuery()
        {
            var sqlQuery = On_GenerateQuery_for_Where_with_String_ToLower_GeneratesCorrectQuery();

            SqlApprovals.Verify(sqlQuery.Sql);
            Assert.AreEqual("@p0", sqlQuery.Parameters[0].Name);
            Assert.AreEqual("foo", sqlQuery.Parameters[0].Value);
        }

        [Test]
        public override void GenerateQuery_for_Where_with_String_QxToLower_GeneratesCorrectQuery()
        {
            var sqlQuery = On_GenerateQuery_for_Where_with_String_QxToLower_GeneratesCorrectQuery();

            SqlApprovals.Verify(sqlQuery.Sql);
            Assert.AreEqual("@p0", sqlQuery.Parameters[0].Name);
            Assert.AreEqual("foo", sqlQuery.Parameters[0].Value);
        }

        [Test]
        public override void GenerateQuery_for_Where_with_String_ToUpper_GeneratesCorrectQuery()
        {
            var sqlQuery = On_GenerateQuery_for_Where_with_String_ToUpper_GeneratesCorrectQuery();

            SqlApprovals.Verify(sqlQuery.Sql);
            Assert.AreEqual("@p0", sqlQuery.Parameters[0].Name);
            Assert.AreEqual("FOO", sqlQuery.Parameters[0].Value);
        }

        [Test]
        public override void GenerateQuery_for_Where_with_String_QxToUpper_GeneratesCorrectQuery()
        {
            var sqlQuery = On_GenerateQuery_for_Where_with_String_QxToUpper_GeneratesCorrectQuery();

            SqlApprovals.Verify(sqlQuery.Sql);
            Assert.AreEqual("@p0", sqlQuery.Parameters[0].Name);
            Assert.AreEqual("FOO", sqlQuery.Parameters[0].Value);
        }

        [Test]
        public override void GenerateQuery_for_Where_with_String_QxIsExactly_GeneratesCorrectQuery()
        {
            var sqlQuery = On_GenerateQuery_for_Where_with_String_QxIsExactly_GeneratesCorrectQuery();

            SqlApprovals.Verify(sqlQuery.Sql);
            Assert.AreEqual(1, sqlQuery.Parameters.Length);
            Assert.AreEqual("@p0", sqlQuery.Parameters[0].Name);
            Assert.AreEqual("Foo", sqlQuery.Parameters[0].Value);
        }
    }
}