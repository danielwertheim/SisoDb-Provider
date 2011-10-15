using Machine.Specifications;
using SisoDb.Sql2008;

namespace SisoDb.Specifications.Sql2008.Database.EnsureNewDatabase
{
    [Subject(typeof(Sql2008Database), "Ensure new database")]
    public class when_no_database_exists
    {
        Establish context = () =>
        {
            _testContext = TestContextFactory.CreateTemp(StorageProviders.Sql2008);
            _testContext.DbHelperForServer.DropDatabaseIfExists(_testContext.Database.Name);
        };

        Because of = () => _testContext.Database.EnsureNewDatabase();

        It should_get_created = () => _testContext.Database.Exists();

        It should_have_created_identities_table = () => _testContext.DbHelper.TableExists("SisoDbIdentities").ShouldBeTrue();

        It should_have_created_custom_ids_data_types = () => 
        {
            _testContext.DbHelper.TypeExists("SisoGuidIds").ShouldBeTrue();
            _testContext.DbHelper.TypeExists("StructureIdentityIds").ShouldBeTrue();
        };

        private static ITestContext _testContext;
    }
}