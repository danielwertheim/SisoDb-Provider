using System;
using Machine.Specifications;
using SisoDb.Sql2008;
using SisoDb.Testing;

namespace SisoDb.Specifications.Sql2008.Database
{
    namespace InitializeExisting
    {
        [Subject(typeof (Sql2008Database), "Initialize existing")]
        public class when_blank_database_exists : SpecificationBase
        {
            Establish context = () =>
            {
                _testContext = TestContextFactory.CreateTemp(StorageProviders.Sql2008);
                _testContext.DbHelperForServer.DropDatabaseIfExists(_testContext.Database.Name);
                _testContext.DbHelperForServer.EnsureDbExists(_testContext.Database.Name);
            };

            Because of = () => _testContext.Database.InitializeExisting();

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

        [Subject(typeof (Sql2008Database), "Initialize existing")]
        public class when_no_database_exists : SpecificationBase
        {
            Establish context = () =>
            {
                _testContext = TestContextFactory.CreateTemp(StorageProviders.Sql2008);
                _testContext.DbHelperForServer.DropDatabaseIfExists(_testContext.Database.Name);
            };

            Because of = 
                () => CaughtException = Catch.Exception(() => _testContext.Database.InitializeExisting());

            It should_fail = 
                () => CaughtException.ShouldBeOfType<SisoDbException>();

            private static ITestContext _testContext;
        }
    }
}