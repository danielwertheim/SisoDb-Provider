using Machine.Specifications;
using SisoDb.Dac;
using SisoDb.Sql2008.Dac;
using SisoDb.Testing;

namespace SisoDb.IntegrationTests.Sql2008.Sql2008ServerClientTests
{
    namespace CreateNew
    {
        [Subject(typeof(Sql2008ServerClient), "Create new")]
        public class when_database_exists : SpecificationBase
        {
            Establish context = () =>
                TestContext = TestContextFactory.Create(StorageProviders.Sql2008);

            Because of = () => _serverClient = new Sql2008ServerClient(TestContext.Database.ConnectionInfo);

            It should_get_connection_string_against_master =
                () => _serverClient.ConnectionString.PlainString.ShouldEqual(@"Data Source=.\sqlexpress;Initial Catalog=;Integrated Security=True");

            It should_get_correct_provider_type =
                () => _serverClient.ProviderType.ShouldEqual(StorageProviders.Sql2008);

            private static IServerClient _serverClient;
        }
    }
}