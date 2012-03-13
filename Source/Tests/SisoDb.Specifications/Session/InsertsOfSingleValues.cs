using System;
using Machine.Specifications;
using SisoDb.Testing;
using SisoDb.Testing.Steps;

namespace SisoDb.Specifications.Session
{
    class InsertsOfSingleValues
    {
        [Subject(typeof(ISession), "Insert (when single string member")]
        public class when_inserting_item_with_single_string_member_that_is_null : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
            };

            Because of =
                () => TestContext.Database.UseOnceTo().Insert(new SingleStringMember { Value = null });

            It should_have_one_item_inserted =
                () => TestContext.Database.should_have_X_num_of_items<SingleStringMember>(1);

            It should_have_inserted_an_item_with_partial_values = () =>
            {
                var refetched = TestContext.Database.UseOnceTo().Query<SingleStringMember>().FirstOrDefault();
                refetched.ShouldNotBeNull();
                refetched.Value.ShouldEqual(default(string));
            };
        }

        [Subject(typeof(ISession), "Insert (when single datetime member")]
        public class when_inserting_item_with_single_datetime_member_that_is_null : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
            };

            Because of =
                () => TestContext.Database.UseOnceTo().Insert(new SingleDateTimeMember { Value = null });

            It should_have_one_item_inserted =
                () => TestContext.Database.should_have_X_num_of_items<SingleDateTimeMember>(1);

            It should_have_inserted_an_item_with_partial_values = () =>
            {
                var refetched = TestContext.Database.UseOnceTo().Query<SingleDateTimeMember>().FirstOrDefault();
                refetched.ShouldNotBeNull();
                refetched.Value.ShouldEqual(default(DateTime?));
            };
        }

        private class SingleDateTimeMember
        {
            public Guid StructureId { get; set; }
            public DateTime? Value { get; set; }
        }

        private class SingleStringMember
        {
            public Guid StructureId { get; set; }
            public string Value { get; set; }
        }
    }
}