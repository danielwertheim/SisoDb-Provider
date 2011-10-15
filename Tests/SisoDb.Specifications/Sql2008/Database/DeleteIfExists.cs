using Machine.Specifications;
using SisoDb.Sql2008;

namespace SisoDb.Specifications.Sql2008.Database.DeleteIfExists
{
    [Subject(typeof(Sql2008Database), "Delete if exists")]
    public class when_database_exists
    {
        Establish context = () =>
        {
            _testContext = TestContextFactory.CreateTemp(StorageProviders.Sql2008);
        };

        Because of = () => _testContext.Database.DeleteIfExists();

        It should_get_dropped = () => _testContext.Database.Exists().ShouldBeFalse();

        private static ITestContext _testContext;
    }
}