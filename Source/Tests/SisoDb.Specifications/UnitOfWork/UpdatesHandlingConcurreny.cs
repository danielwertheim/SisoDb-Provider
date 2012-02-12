using System;
using System.Threading;
using System.Threading.Tasks;
using Machine.Specifications;
using SisoDb.Testing;
using SisoDb.Testing.Steps;

namespace SisoDb.Specifications.UnitOfWork
{
    class UpdatesHandlingConcurreny
    {
        [Subject(typeof(ISession), "Update (concurrencies)")]
        public class when_item_has_enabled_concurrency_check_and_update_is_not_latest : SpecificationBase
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

            It should_not_have_changed_ids_of_the_structure_in_database =
                () => TestContext.Database.should_have_ids<Model>(_orgItem.Id);

            It should_not_have_updated_any_values =
                () => TestContext.Database.should_have_identical_structures(new[] { _orgItem });

            private static Model _orgItem;
        }

        [Subject(typeof(ISession), "Update (concurrencies)")]
        public class when_item_has_enabled_concurrency_check_and_update_is_latest : SpecificationBase
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

        [Subject(typeof(ISession), "Update (concurrencies)")]
        public class when_updating_using_inline_modifier_and_item_has_enabled_concurrency_check_and_update_is_latest : SpecificationBase
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
                    session.Update<Model>(_orgItem.Id, x => 
                    {
                        x.StringValue = "From first update";
                        x.IntValue = 142;
                    });
                    session.Update<Model>(_orgItem.Id, x => x.StringValue = "From second update");
                }
            };

            It should_only_have_one_item_stored =
                () => TestContext.Database.should_have_X_num_of_items<Model>(1);

            It should_not_have_changed_ids_of_the_structure_in_database =
                () => TestContext.Database.should_have_ids<Model>(_orgItem.Id);

            It should_have_updated_values_from_both_updates = () =>
            {
                var refetched = TestContext.Database.UseOnceTo().GetById<Model>(_orgItem.Id);
                refetched.IntValue.ShouldEqual(142);
                refetched.StringValue.ShouldEqual("From second update");
            };

            private static Model _orgItem;
        }

        [Subject(typeof(ISession), "Update (concurrencies)")]
        public class when_updating_using_inline_modifier_and_item_has_enabled_concurrency_check_and_update_is_not_latest : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _orgItem = new Model { StringValue = "Org string", IntValue = 42 };
                TestContext.Database.UseOnceTo().Insert(_orgItem);
            };

            Because of = () =>
            {
                using (var semaphore = new SemaphoreSlim(1, 1))
                {
                    Task task1 = null, task2 = null;
                    try
                    {
                        task1 = new Task(() =>
                        {
                            using (var session = TestContext.Database.BeginSession())
                            {
                                CaughtException = Catch.Exception(() =>
                                {
                                    session.Update<Model>(_orgItem.Id, x =>
                                    {
                                        semaphore.Release();
                                        Task.WaitAll(task2);

                                        x.StringValue = "From first update";
                                        x.IntValue = 142;
                                    });
                                });
                            }
                        });

                        task2 = new Task(() =>
                        {
                            semaphore.Wait();
                            TestContext.Database.UseOnceTo().Update<Model>(_orgItem.Id, x => x.StringValue = "From second update");
                            semaphore.Release();
                        });

                        semaphore.Wait();
                        task1.Start();
                        task2.Start();
                        Task.WaitAll(task1, task2);
                    }
                    finally
                    {
                        if (task1 != null && task1.Status == TaskStatus.RanToCompletion)
                            task1.Dispose();
                        if (task2 != null && task2.Status == TaskStatus.RanToCompletion)
                            task2.Dispose();
                    }
                }
            };

            It should_have_thrown_SisoDbConcurrencyException = () =>
            {
                CaughtException.ShouldNotBeNull();
                CaughtException.ShouldBeOfType<SisoDbConcurrencyException>();

                var ex = (SisoDbConcurrencyException)CaughtException;
                ex.StructureId.ShouldEqual(_orgItem.Id);
                ex.StructureName.ShouldEqual(typeof(Model).Name);
            }; 

            It should_only_have_one_item_stored =
                () => TestContext.Database.should_have_X_num_of_items<Model>(1);

            It should_not_have_changed_ids_of_the_structure_in_database =
                () => TestContext.Database.should_have_ids<Model>(_orgItem.Id);

            It should_have_updated_only_values_from_second_update = () =>
            {
                var refetched = TestContext.Database.UseOnceTo().GetById<Model>(_orgItem.Id);
                refetched.IntValue.ShouldEqual(_orgItem.IntValue);
                refetched.StringValue.ShouldEqual("From second update");
            };

            private static Model _orgItem;
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