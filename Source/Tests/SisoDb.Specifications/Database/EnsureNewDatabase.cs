using Machine.Specifications;
using SisoDb.Testing;

namespace SisoDb.Specifications.Database
{
    namespace EnsureNewDatabase
    {
        [Subject(typeof (ISisoDatabase), "Ensure new database")]
        public class when_no_database_exists : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.CreateTemp();
                TestContext.DbHelperForServer.DropDatabaseIfExists(TestContext.Database.Name);
            };

            Because of = 
                () => TestContext.Database.EnsureNewDatabase();

            It should_get_created = 
                () => TestContext.Database.Exists();

            It should_have_created_identities_table =
                () => TestContext.DbHelper.TableExists("SisoDbIdentities").ShouldBeTrue();

            It should_have_created_custom_ids_data_types = () =>
            {
                TestContext.DbHelper.TypeExists("SisoGuidIds").ShouldBeTrue();
                TestContext.DbHelper.TypeExists("StructureIdentityIds").ShouldBeTrue();
            };
        }
    }
}