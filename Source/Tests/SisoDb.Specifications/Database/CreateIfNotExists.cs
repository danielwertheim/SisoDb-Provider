using Machine.Specifications;
using SisoDb.Testing;

namespace SisoDb.Specifications.Database
{
    namespace CreateIfNotExists
    {
        [Subject(typeof (ISisoDatabase), "Create if not exists")]
        public class when_no_database_exists : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.CreateTemp();
                TestContext.Database.DeleteIfExists();
            };

            Because of = 
                () => TestContext.Database.CreateIfNotExists();

            It should_get_created = 
                () => TestContext.Database.Exists();

            It should_have_created_identities_table =
                () => TestContext.DbHelper.TableExists("SisoDbIdentities").ShouldBeTrue();

#if Sql2008Provider
            It should_have_created_custom_ids_data_types = () =>
            {
                TestContext.DbHelper.TypeExists("SisoGuidIds").ShouldBeTrue();
                TestContext.DbHelper.TypeExists("StructureIdentityIds").ShouldBeTrue();
            };
#endif
        }
    }
}