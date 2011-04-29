using NUnit.Framework;
using SisoDb.Providers.SqlProvider.DbSchema;

namespace SisoDb.Tests.UnitTests.Providers.SqlProvider.DbSchema
{
    [TestFixture]
    public class SqlDbColumnGeneratorTests : UnitTestBase
    {
        [Test]
        public void ToSql_WhenNameAndDbDataTypeIsPassed_ReturnsSqlWithNameAndType()
        {
            const string arbitraryColumnName = "TestName";
            const string arbitraryDbDataTypeName = "TestDbDataType";
            var expectedColumnDefinition = string.Format("[{0}] {1} sparse null", arbitraryColumnName, arbitraryDbDataTypeName);

            var columnGenerator = new SqlDbColumnGenerator();
            var columnDefinition = columnGenerator.ToSql(arbitraryColumnName, arbitraryDbDataTypeName);

            Assert.AreEqual(expectedColumnDefinition, columnDefinition);
        }
    }
}