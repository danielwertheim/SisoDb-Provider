using Machine.Specifications;
using SisoDb.Dac;
using SisoDb.Sql2008.Dac;
using SisoDb.Testing;

namespace SisoDb.IntegrationTests.Sql2008.Sql2008DbClientTests
{
    namespace CreateNew
    {
        [Subject(typeof(Sql2008DbClient), "Create new")]
        public class when_database_exists : SpecificationBase
        {
            Establish context = () =>
                TestContext = TestContextFactory.Create(StorageProviders.Sql2008);

            Because of = () => _dbClient = new Sql2008DbClient(TestContext.Database.ConnectionInfo, false);

            It should_get_correct_connection_string =
                () => _dbClient.ConnectionInfo.ConnectionString.PlainString.ShouldEqual(@"data source=.\sqlexpress;initial catalog=SisoDbTests;integrated security=SSPI;");

            It should_get_correct_provider_type =
                () => _dbClient.ConnectionInfo.ProviderType.ShouldEqual(StorageProviders.Sql2008);

            It should_have_connection_against_specified_db =
                () => _dbClient.ConnectionInfo.DbName.ShouldEqual("SisoDbTests");

            private static IDbClient _dbClient;
        }
    }
}