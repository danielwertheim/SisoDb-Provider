using System;
using Machine.Specifications;
using SisoDb.Testing;
using SisoDb.Testing.Steps;

namespace SisoDb.Specifications.Session
{
    class UpdatesOfSingleValues
    {
        [Subject(typeof(ISession), "Update (when single string member")]
        public class when_updating_item_with_single_string_member_to_null : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _item = new SingleStringMember { Value = "I have a value" };
                TestContext.Database.UseOnceTo().Insert(_item);
            };

            Because of = () =>
                TestContext.Database.UseOnceTo().Update<SingleStringMember>(_item.StructureId, i => i.Value = null);

            It should_have_one_item_stored =
                () => TestContext.Database.should_have_X_num_of_items<SingleStringMember>(1);

            It should_have_updated_string_member_to_null = () =>
            {
                var refetched = TestContext.Database.UseOnceTo().Query<SingleStringMember>().FirstOrDefault();
                refetched.ShouldNotBeNull();
                refetched.Value.ShouldEqual(default(string));
            };

            private static SingleStringMember _item;
        }

        [Subject(typeof(ISession), "Update (when single datetime member")]
        public class when_updating_item_with_single_datetime_member_to_null : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _item = new SingleDateTimeMember { Value = new DateTime(2012, 1, 1) };
                TestContext.Database.UseOnceTo().Insert(_item);
            };

            Because of = () =>
                TestContext.Database.UseOnceTo().Update<SingleDateTimeMember>(_item.StructureId, i => i.Value = null);

            It should_have_one_item_stored =
                () => TestContext.Database.should_have_X_num_of_items<SingleDateTimeMember>(1);

            It should_have_updated_string_member_to_null = () =>
            {
                var refetched = TestContext.Database.UseOnceTo().Query<SingleDateTimeMember>().FirstOrDefault();
                refetched.ShouldNotBeNull();
                refetched.Value.ShouldEqual(default(DateTime?));
            };

            private static SingleDateTimeMember _item;
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