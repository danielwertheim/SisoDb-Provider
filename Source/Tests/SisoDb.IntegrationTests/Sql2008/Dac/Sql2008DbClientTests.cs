using Machine.Specifications;
using SisoDb.Dac;
using SisoDb.Sql2008.Dac;
using SisoDb.Testing;

namespace SisoDb.IntegrationTests.Sql2008.Dac
{
    namespace Sql2008DbClientTests
    {
        [Subject(typeof(Sql2008DbClient), "Create new")]
        public class when_database_exists
        {
            Establish context = () =>
                _testContext = TestContextFactory.Create(StorageProviders.Sql2008);

            Because of = () => _dbClient = new Sql2008DbClient(_testContext.Database.ConnectionInfo, false);

            It should_get_correct_connection_string =
                () => _dbClient.ConnectionString.PlainString.ShouldEqual(@"data source=.\sqlexpress;initial catalog=SisoDbTests;integrated security=SSPI;");

            It should_get_correct_provider_type =
                () => _dbClient.ProviderType.ShouldEqual(StorageProviders.Sql2008);

            It should_have_connection_against_specified_db =
                () => _dbClient.DbName.ShouldEqual("SisoDbTests");

            private static ITestContext _testContext;
            private static IDbClient _dbClient;
        }
    }
}