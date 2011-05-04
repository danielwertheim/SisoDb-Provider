using NUnit.Framework;
using SisoDb.Providers.Sql2008.DbSchema;

namespace SisoDb.Tests.UnitTests.Providers.Sql2008.DbSchema
{
    [TestFixture]
    public class Sql2008DbColumnGeneratorTests : UnitTestBase
    {
        [Test]
        public void ToSql_WhenNameAndDbDataTypeIsPassed_ReturnsSqlWithNameAndType()
        {
            const string arbitraryColumnName = "TestName";
            const string arbitraryDbDataTypeName = "TestDbDataType";
            var expectedColumnDefinition = string.Format("[{0}] {1} sparse null", arbitraryColumnName, arbitraryDbDataTypeName);

            var columnGenerator = new Sql2008DbColumnGenerator();
            var columnDefinition = columnGenerator.ToSql(arbitraryColumnName, arbitraryDbDataTypeName);

            Assert.AreEqual(expectedColumnDefinition, columnDefinition);
        }
    }
}