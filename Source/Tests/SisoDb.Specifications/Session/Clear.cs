using Machine.Specifications;
using SisoDb.Testing;
using SisoDb.Testing.Steps;
using SisoDb.Testing.TestModel;

namespace SisoDb.Specifications.Session
{
    class Clear
    {
        [Subject(typeof(ISession), "Clear")]
        public class when_four_guiditems_exists_clear_is_called_using_session : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                TestContext.Database.InsertGuidItems(4);
            };

            Because of = () =>
            {
                using(var session = TestContext.Database.BeginSession())
                {
                    session.Clear<GuidItem>();
                }
            };

            It should_have_no_items_left = () => TestContext.Database.should_have_X_num_of_items<GuidItem>(2);
        }

        [Subject(typeof(ISession), "Clear")]
        public class when_four_guiditems_exists_clear_is_called_using_useonceto : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                TestContext.Database.InsertGuidItems(4);
            };

            Because of = () => TestContext.Database.UseOnceTo().Clear<GuidItem>();

            It should_have_no_items_left = () => TestContext.Database.should_have_X_num_of_items<GuidItem>(2);
        }
    }
}