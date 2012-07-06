using Machine.Specifications;
using SisoDb.Testing;

namespace SisoDb.Specifications.Database
{
    class EnsureNewDatabase
    {
        [Subject(typeof (ISisoDatabase), "Ensure new database")]
        public class when_no_database_exists
        {
            private Establish context = () =>
            {
                _testContext = TestContextFactory.CreateTemp();
                _testContext.Database.DeleteIfExists();
            };

            private Because of =
                () => _testContext.Database.EnsureNewDatabase();

            private It should_get_created =
                () => _testContext.Database.Exists();

            private It should_have_created_identities_table =
                () => _testContext.DbHelper.TableExists("SisoDbIdentities").ShouldBeTrue();

#if Sql2008Provider || Sql2012Provider || SqlProfilerProvider
            private It should_have_created_custom_ids_data_types = () =>
            {
                _testContext.DbHelper.TypeExists("SisoGuidIds").ShouldBeTrue();
                _testContext.DbHelper.TypeExists("SisoStringIds").ShouldBeTrue();
                _testContext.DbHelper.TypeExists("SisoIdentityIds").ShouldBeTrue();
            };

            It should_have_created_custom_tableParams_for_each_typegroup = () =>
            {
                _testContext.DbHelper.TypeExists("SisoIntegers").ShouldBeTrue();
                _testContext.DbHelper.TypeExists("SisoFractals").ShouldBeTrue();
                _testContext.DbHelper.TypeExists("SisoDates").ShouldBeTrue();
                _testContext.DbHelper.TypeExists("SisoBooleans").ShouldBeTrue();
                _testContext.DbHelper.TypeExists("SisoGuids").ShouldBeTrue();
                _testContext.DbHelper.TypeExists("SisoStrings").ShouldBeTrue();
                _testContext.DbHelper.TypeExists("SisoTexts").ShouldBeTrue();
            };
#endif
#if SqlCe4Provider || Sql2005Provider
            It should_not_have_created_custom_ids_data_types = () =>
            {
                _testContext.DbHelper.TypeExists("SisoGuidIds").ShouldBeFalse();
                _testContext.DbHelper.TypeExists("SisoStringIds").ShouldBeFalse();
                _testContext.DbHelper.TypeExists("SisoIdentityIds").ShouldBeFalse();
            };

            It should_not_have_created_custom_tableParams_for_each_typegroup = () =>
            {
                _testContext.DbHelper.TypeExists("SisoIntegers").ShouldBeFalse();
                _testContext.DbHelper.TypeExists("SisoFractals").ShouldBeFalse();
                _testContext.DbHelper.TypeExists("SisoDates").ShouldBeFalse();
                _testContext.DbHelper.TypeExists("SisoBooleans").ShouldBeFalse();
                _testContext.DbHelper.TypeExists("SisoGuids").ShouldBeFalse();
                _testContext.DbHelper.TypeExists("SisoStrings").ShouldBeFalse();
                _testContext.DbHelper.TypeExists("SisoTexts").ShouldBeFalse();
            };
#endif
            private static ITestContext _testContext;
        }
    }
}