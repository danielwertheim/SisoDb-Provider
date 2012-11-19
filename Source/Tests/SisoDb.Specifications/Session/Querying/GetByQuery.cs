using Machine.Specifications;
using SisoDb.Specifications.Model;
using SisoDb.Testing;

namespace SisoDb.Specifications.Session.Querying
{
    class GetByQuery
    {
        [Subject(typeof(ISession), "GetByQuery")]
        public class when_no_structures_are_present : SpecificationBase
        {
            Establish context = 
                () => TestContext = TestContextFactory.Create();

            Because of = 
                () => _fetchedStructure = TestContext.Database.UseOnceTo().GetByQuery<QueryGuidItem>(i => i.IntegerValue == 42);

            It should_return_a_null_structure =
                () => _fetchedStructure.ShouldBeNull();

            private static QueryGuidItem _fetchedStructure;
        }
    }
}