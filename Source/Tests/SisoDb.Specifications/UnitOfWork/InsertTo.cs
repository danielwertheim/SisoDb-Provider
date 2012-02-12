using System;
using Machine.Specifications;
using SisoDb.Testing;
using SisoDb.Testing.Steps;

namespace SisoDb.Specifications.UnitOfWork
{
    class InsertTo
    {
        [Subject(typeof(ISession), "InsertTo (when anonymous")]
        public class when_inserting_anonymous_that_partially_conforms_to_contract : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
            };

            Because of = () =>
            {
                var item = new { IntValue = 42};
                TestContext.Database.UseOnceTo().InsertTo<Model>(item);
            };

            It should_have_one_item_inserted =
                () => TestContext.Database.should_have_X_num_of_items<Model>(1);

            It should_have_inserted_an_item_with_partial_values = () =>
            {
                var refetched = TestContext.Database.UseOnceTo().Query<Model>().FirstOrDefault();
                refetched.ShouldNotBeNull();
                refetched.IntValue.ShouldEqual(42);
                refetched.StringValue.ShouldEqual(default(string));
            };
        }

        private class Model
        {
            public Guid StructureId { get; set; }

            public string StringValue { get; set; }

            public int IntValue { get; set; }
        }
    }
}