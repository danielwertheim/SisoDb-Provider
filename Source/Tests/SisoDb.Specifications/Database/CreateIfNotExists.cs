using Machine.Specifications;
using SisoDb.Testing;

namespace SisoDb.Specifications.Database
{
    namespace CreateIfNotExists
    {
        [Subject(typeof (ISisoDatabase), "Create if not exists")]
        public class when_no_database_exists
        {
            Establish context = () =>
            {
                _testContext = TestContextFactory.CreateTemp();
                _testContext.DbHelperForServer.DropDatabaseIfExists(_testContext.Database.Name);
            };

            Because of = () => _testContext.Database.CreateIfNotExists();

            It should_get_created = () => _testContext.Database.Exists();

            It should_have_created_identities_table =
                () => _testContext.DbHelper.TableExists("SisoDbIdentities").ShouldBeTrue();

            It should_have_created_custom_ids_data_types = () =>
            {
                _testContext.DbHelper.TypeExists("SisoGuidIds").ShouldBeTrue();
                _testContext.DbHelper.TypeExists("StructureIdentityIds").ShouldBeTrue();
            };

            private static ITestContext _testContext;
        }
    }
}