using System;
using System.Linq;
using Machine.Specifications;
using SisoDb.Testing;
using SisoDb.Testing.Steps;

namespace SisoDb.Specifications.Session
{
    class Inserts
    {
        [Subject(typeof(ISession), "Insert")]
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

            It should_have_inserted_an_item_with_null_values = () =>
            {
                var refetched = TestContext.Database.UseOnceTo().Query<SingleStringMember>().FirstOrDefault();
                refetched.Value.ShouldBeNull();
            };
        }

        [Subject(typeof(ISession), "InsertMany")]
        public class when_inserting_three_items_with_single_string_member_that_is_null : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
            };

            Because of = () =>
                TestContext.Database.UseOnceTo().InsertMany(new[]
                {
                    new SingleStringMember { Value = null },
                    new SingleStringMember { Value = null },
                    new SingleStringMember { Value = null }
                });

            It should_have_three_items_inserted =
                () => TestContext.Database.should_have_X_num_of_items<SingleStringMember>(3);

            It should_have_inserted_item_with_null_values = () =>
            {
                var refetched = TestContext.Database.UseOnceTo().Query<SingleStringMember>().ToArray();
                refetched.All(s => s.Value == null).ShouldBeTrue();
            };
        }

        [Subject(typeof(ISession), "Insert")]
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

            It should_have_inserted_an_item_with_null_values = () =>
            {
                var refetched = TestContext.Database.UseOnceTo().Query<SingleDateTimeMember>().FirstOrDefault();
                refetched.Value.ShouldBeNull();
            };
        }
#if Sql2005Provider || Sql2008Provider || Sql2012Provider || SqlProfilerProvider
        [Subject(typeof(ISession), "Insert")]
        public class when_inserting_item_with_single_datetime_member_having_min_value : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
            };

            Because of =
                () => TestContext.Database.UseOnceTo().Insert(new SingleDateTimeMember { Value = DateTime.MinValue });

            It should_have_one_item_inserted =
                () => TestContext.Database.should_have_X_num_of_items<SingleDateTimeMember>(1);

            It should_have_inserted_item_with_min_value = () =>
            {
                var refetched = TestContext.Database.UseOnceTo().Query<SingleDateTimeMember>().FirstOrDefault();
                refetched.Value.ShouldEqual(DateTime.MinValue);
            };
        }

        [Subject(typeof(ISession), "InsertMany")]
        public class when_inserting_items_with_datetimes_having_min_value : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
            };

            Because of = () => TestContext.Database.UseOnceTo().InsertMany(new[]
            {
                new MultiDateTimeMember { Dates = new[] { DateTime.MinValue, DateTime.MinValue } },
                new MultiDateTimeMember { Dates = new[] { DateTime.MinValue, DateTime.MinValue } },
                new MultiDateTimeMember { Dates = new[] { DateTime.MinValue, DateTime.MinValue } }
            });

            It should_have_one_item_inserted =
                () => TestContext.Database.should_have_X_num_of_items<MultiDateTimeMember>(3);

            It should_have_inserted_item_with_min_value = () =>
            {
                var refetched = TestContext.Database.UseOnceTo().Query<MultiDateTimeMember>().ToArray();
                refetched.SelectMany(r => r.Dates).All(d => d.Equals(DateTime.MinValue)).ShouldBeTrue();
            };
        }
#endif
        [Subject(typeof(ISession), "Insert")]
        public class when_inserting_item_with_single_text_member_that_is_null : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
            };

            Because of =
                () => TestContext.Database.UseOnceTo().Insert(new SingleTextMember { Text = null });

            It should_have_one_item_inserted =
                () => TestContext.Database.should_have_X_num_of_items<SingleTextMember>(1);

            It should_have_inserted_an_item_with_null_values = () =>
            {
                var refetched = TestContext.Database.UseOnceTo().Query<SingleTextMember>().FirstOrDefault();
                refetched.Text.ShouldBeNull();
            };
        }

        [Subject(typeof(ISession), "InsertMany")]
        public class when_inserting_three_items_with_single_text_member_that_is_null : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
            };

            Because of = () => 
                TestContext.Database.UseOnceTo().InsertMany(new []
                {
                    new SingleTextMember { Text = null },
                    new SingleTextMember { Text = null },
                    new SingleTextMember { Text = null }
                });

            It should_have_three_items_inserted =
                () => TestContext.Database.should_have_X_num_of_items<SingleTextMember>(3);

            It should_have_inserted_item_with_null_values = () =>
            {
                var refetched = TestContext.Database.UseOnceTo().Query<SingleTextMember>().ToArray();
                refetched.All(s => s.Text == null).ShouldBeTrue();
            };
        }

        private class SingleStringMember
        {
            public Guid StructureId { get; set; }
            public string Value { get; set; }
        }

        private class SingleTextMember
        {
            public Guid StructureId { get; set; }
            public string Text { get; set; }
        }

        private class SingleDateTimeMember
        {
            public Guid StructureId { get; set; }
            public DateTime? Value { get; set; }
        }

        private class MultiDateTimeMember
        {
            public Guid StructureId { get; set; }
            public DateTime[] Dates { get; set; }
        }
    }
}