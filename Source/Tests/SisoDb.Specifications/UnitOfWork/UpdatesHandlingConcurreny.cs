using System;
using Machine.Specifications;
using SisoDb.Testing;
using SisoDb.Testing.Steps;

namespace SisoDb.Specifications.UnitOfWork
{
    class UpdatesHandlingConcurreny
    {
        [Subject(typeof(ISession), "Update (concurrencies)")]
        public class when_item_has_enabled_concurrency_check_and_is_not_latest : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _orgItem = new Model { StringValue = "Org string", IntValue = 42 };
                TestContext.Database.UseOnceTo().Insert(_orgItem);
            };

            Because of = () =>
            {
                using(var session = TestContext.Database.BeginSession())
                {
                    var copy1 = session.GetById<Model>(_orgItem.Id);
                    var copy2 = session.GetById<Model>(_orgItem.Id);

                    copy1.StringValue = "From copy 1";
                    session.Update(copy1);

                    copy2.StringValue = "From copy 2";

                    CaughtException = Catch.Exception(() => session.Update(copy2));
                }
            };

            It should_have_thrown_SisoDbConcurrencyException = () =>
            {
                CaughtException.ShouldNotBeNull();
                CaughtException.ShouldBeOfType<SisoDbConcurrencyException>();
                
                var ex = (SisoDbConcurrencyException) CaughtException;
                ex.StructureId.ShouldEqual(_orgItem.Id);
                ex.StructureName.ShouldEqual(typeof (Model).Name);
            };

            private static Model _orgItem;
        }

        [Subject(typeof(ISession), "Update (concurrencies)")]
        public class when_item_has_enabled_concurrency_check_and_is_latest : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _orgItem = new Model { StringValue = "Org string", IntValue = 42 };
                TestContext.Database.UseOnceTo().Insert(_orgItem);
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    _copy1 = session.GetById<Model>(_orgItem.Id);
                    _copy1.StringValue = "From copy 1";
                    session.Update(_copy1);

                    _copy2 = session.GetById<Model>(_orgItem.Id);
                    _copy2.StringValue = "From copy 2";
                    session.Update(_copy2);
                }
            };

            It should_only_have_one_item_stored =
                () => TestContext.Database.should_have_X_num_of_items<Model>(1);

            It should_not_have_changed_ids_of_the_structure_in_database =
                () => TestContext.Database.should_have_ids<Model>(_orgItem.Id);

            It should_have_updated_values_in_database =
                () => TestContext.Database.should_have_identical_structures(new []{_copy2});

            private static Model _orgItem;
            private static Model _copy1, _copy2;
        }

        private class Model
        {
            public Guid Id { get; set; }

            public Guid ConcurrencyToken { get; set; }

            public string StringValue { get; set; }

            public int IntValue { get; set; }
        }
    }
}