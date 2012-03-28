using Machine.Specifications;
using SisoDb.Testing;

namespace SisoDb.Specifications.Database
{
    class InitializeExisting
    {
        [Subject(typeof (ISisoDatabase), "Initialize existing")]
        public class when_blank_database_exists : SpecificationBase
        {
            Establish context = () =>
            {
                _testContext = TestContextFactory.CreateTemp();
                _testContext.Database.EnsureNewDatabase();
            };

            Because of = 
                () => _testContext.Database.InitializeExisting();

            It should_get_created = 
                () => _testContext.Database.Exists();

            It should_have_created_identities_table =
                () => _testContext.DbHelper.TableExists("SisoDbIdentities").ShouldBeTrue();

#if Sql2008Provider || Sql2012Provider || SqlProfilerProvider
            It should_have_created_custom_ids_data_types = () =>
            {
                _testContext.DbHelper.TypeExists("SisoGuidIds").ShouldBeTrue();
                _testContext.DbHelper.TypeExists("StructureIdentityIds").ShouldBeTrue();
            };
#endif
#if SqlCe4Provider
            It should_not_have_created_custom_ids_data_types = () =>
            {
                _testContext.DbHelper.TypeExists("SisoGuidIds").ShouldBeFalse();
                _testContext.DbHelper.TypeExists("StructureIdentityIds").ShouldBeFalse();
            };
#endif
            private static ITestContext _testContext;
        }

        [Subject(typeof (ISisoDatabase), "Initialize existing")]
        public class when_no_database_exists : SpecificationBase
        {
            Establish context = () =>
            {
                _testContext = TestContextFactory.CreateTemp();
                _testContext.Database.DeleteIfExists();
            };

            Because of =
                () => CaughtException = Catch.Exception(() => _testContext.Database.InitializeExisting());

            It should_fail =
                () => CaughtException.ShouldBeOfType<SisoDbException>();

            private static ITestContext _testContext;
        }
    }
}