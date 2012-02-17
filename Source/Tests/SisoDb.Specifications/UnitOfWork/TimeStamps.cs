using System;
using Machine.Specifications;
using SisoDb.Testing;
using SisoDb.Testing.Steps;

namespace SisoDb.Specifications.UnitOfWork
{
    class TimeStamps
    {
        [Subject(typeof(ISession), "Insert (timestamp)")]
        public class when_inserting_item_with_timestamp : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _beforeExecution = DateTime.Now;
                _orgItem = new Model { StringValue = "Org string" };
            };

            Because of = () => TestContext.Database.UseOnceTo().Insert(_orgItem);

            It should_only_have_one_item_stored =
                () => TestContext.Database.should_have_X_num_of_items<Model>(1);

            It should_have_assigned_value_to_timestamp =
                () => _orgItem.TimeStamp.ShouldBeGreaterThan(_beforeExecution);

            private static DateTime _beforeExecution;
            private static Model _orgItem;
        }

        [Subject(typeof(ISession), "Insert (timestamp)")]
        public class when_updating_item_with_timestamp : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _orgItem = new Model { StringValue = "Org string" };
                TestContext.Database.UseOnceTo().Insert(_orgItem);
                _beforeExecution = _orgItem.TimeStamp;
            };

            Because of = () => TestContext.Database.UseOnceTo().Update(_orgItem);

            It should_only_have_one_item_stored =
                () => TestContext.Database.should_have_X_num_of_items<Model>(1);

            It should_have_assigned_new_value_to_timestamp =
                () => _orgItem.TimeStamp.ShouldBeGreaterThan(_beforeExecution);

            private static DateTime _beforeExecution;
            private static Model _orgItem;
        }

        [Subject(typeof(ISession), "Insert (timestamp)")]
        public class when_updating_item_with_timestamp_using_inline_construct : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _orgItem = new Model { StringValue = "Org string" };
                TestContext.Database.UseOnceTo().Insert(_orgItem);
                _beforeExecution = _orgItem.TimeStamp;
            };

            Because of = () => TestContext.Database.UseOnceTo().Update<Model>(_orgItem.Id, x => x.StringValue = "Foooooo");

            It should_only_have_one_item_stored =
                () => TestContext.Database.should_have_X_num_of_items<Model>(1);

            It should_have_assigned_new_value_to_timestamp =
                () =>
                {
                    var refetched = TestContext.Database.UseOnceTo().GetById<Model>(_orgItem.Id);
                    refetched.TimeStamp.ShouldBeGreaterThan(_beforeExecution);
                };

            private static DateTime _beforeExecution;
            private static Model _orgItem;
        }

        private class Model
        {
            public Guid Id { get; set; }

            public DateTime TimeStamp { get; set; }

            public string StringValue { get; set; }
        }
    }
}