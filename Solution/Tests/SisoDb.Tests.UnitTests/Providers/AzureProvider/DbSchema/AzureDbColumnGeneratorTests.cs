using NUnit.Framework;
using SisoDb.Providers.AzureProvider.DbSchema;

namespace SisoDb.Tests.UnitTests.Providers.AzureProvider.DbSchema
{
    [TestFixture]
    public class AzureDbColumnGeneratorTests : UnitTestBase
    {
        [Test]
        public void ToSql_WhenNameAndDbDataTypeIsPassed_ReturnsSqlWithNameAndType()
        {
            const string arbitraryColumnName = "TestName";
            const string arbitraryDbDataTypeName = "TestDbDataType";
            var expectedColumnDefinition = string.Format("[{0}] {1} null", arbitraryColumnName, arbitraryDbDataTypeName);

            var columnGenerator = new AzureDbColumnGenerator();
            var columnDefinition = columnGenerator.ToSql(arbitraryColumnName, arbitraryDbDataTypeName);

            Assert.AreEqual(expectedColumnDefinition, columnDefinition);
        }
    }
}