using Machine.Specifications;
using SisoDb.Testing;

namespace SisoDb.Specifications.Database
{
    namespace DeleteIfExists
    {
        [Subject(typeof (ISisoDatabase), "Delete if exists")]
        public class when_database_exists : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.CreateTemp();
            };

            Because of = () => TestContext.Database.DeleteIfExists();

            It should_get_dropped = () => TestContext.Database.Exists().ShouldBeFalse();
        }
    }
}