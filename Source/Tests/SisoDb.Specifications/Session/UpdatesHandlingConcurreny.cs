using System;
using System.Threading;
using System.Threading.Tasks;
using Machine.Specifications;
using SisoDb.Structures.Schemas;
using SisoDb.Testing;

namespace SisoDb.Specifications.Session
{
    class UpdatesHandlingConcurreny
    {
        [Subject(typeof(ISession), "Update (concurrencies)")]
        public class when_item_has_enabled_concurrency_check_and_update_is_not_latest : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _orgItem = new ModelWithGuidToken { StringValue = "Org string", IntValue = 42 };
                TestContext.Database.UseOnceTo().Insert(_orgItem);
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var copy1 = session.GetById<ModelWithGuidToken>(_orgItem.Id);
                    var copy2 = session.GetById<ModelWithGuidToken>(_orgItem.Id);

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

                var ex = (SisoDbConcurrencyException)CaughtException;
                ex.StructureId.ShouldEqual(_orgItem.Id);
                ex.StructureName.ShouldEqual(typeof(ModelWithGuidToken).Name);
            };

            It should_not_have_changed_ids_of_the_structure_in_database =
                () => TestContext.Database.should_have_ids<ModelWithGuidToken>(_orgItem.Id);

            It should_not_have_updated_any_values =
                () => TestContext.Database.should_have_identical_structures(new[] { _orgItem });

            private static ModelWithGuidToken _orgItem;
        }

        [Subject(typeof(ISession), "Update (concurrencies)")]
        public class when_item_has_enabled_int_concurrency_check_and_update_is_not_latest : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _orgItem = new ModelWithIntToken { StringValue = "Org string", IntValue = 42 };
                TestContext.Database.UseOnceTo().Insert(_orgItem);
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var copy1 = session.GetById<ModelWithIntToken>(_orgItem.Id);
                    var copy2 = session.GetById<ModelWithIntToken>(_orgItem.Id);

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

                var ex = (SisoDbConcurrencyException)CaughtException;
                ex.StructureId.ShouldEqual(_orgItem.Id);
                ex.StructureName.ShouldEqual(typeof(ModelWithIntToken).Name);
            };

            It should_not_have_changed_ids_of_the_structure_in_database =
                () => TestContext.Database.should_have_ids<ModelWithIntToken>(_orgItem.Id);

            It should_not_have_updated_any_values =
                () => TestContext.Database.should_have_identical_structures(new[] { _orgItem });

            private static ModelWithIntToken _orgItem;
        }

        [Subject(typeof(ISession), "Update (concurrencies)")]
        public class when_item_has_enabled_long_concurrency_check_and_update_is_not_latest : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _orgItem = new ModelWithLongToken { StringValue = "Org string", IntValue = 42 };
                TestContext.Database.UseOnceTo().Insert(_orgItem);
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var copy1 = session.GetById<ModelWithLongToken>(_orgItem.Id);
                    var copy2 = session.GetById<ModelWithLongToken>(_orgItem.Id);

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

                var ex = (SisoDbConcurrencyException)CaughtException;
                ex.StructureId.ShouldEqual(_orgItem.Id);
                ex.StructureName.ShouldEqual(typeof(ModelWithLongToken).Name);
            };

            It should_not_have_changed_ids_of_the_structure_in_database =
                () => TestContext.Database.should_have_ids<ModelWithLongToken>(_orgItem.Id);

            It should_not_have_updated_any_values =
                () => TestContext.Database.should_have_identical_structures(new[] { _orgItem });

            private static ModelWithLongToken _orgItem;
        }

        [Subject(typeof(ISession), "Update (concurrencies)")]
        public class when_item_has_enabled_concurrency_check_and_update_is_latest : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<ModelWithGuidToken>();
                _orgItem = new ModelWithGuidToken { StringValue = "Org string", IntValue = 42 };
                TestContext.Database.UseOnceTo().Insert(_orgItem);
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    _copy1 = session.GetById<ModelWithGuidToken>(_orgItem.Id);
                    _copy1.StringValue = "From copy 1";
                    session.Update(_copy1);

                    _copy2 = session.GetById<ModelWithGuidToken>(_orgItem.Id);
                    _copy2.StringValue = "From copy 2";
                    session.Update(_copy2);
                }
            };

            It should_only_have_one_item_stored =
                () => TestContext.Database.should_have_X_num_of_items<ModelWithGuidToken>(1);

            It should_not_have_changed_ids_of_the_structure_in_database =
                () => TestContext.Database.should_have_ids<ModelWithGuidToken>(_orgItem.Id);

            It should_have_updated_values_in_database =
                () => TestContext.Database.should_have_identical_structures(new[] { _copy2 });

            It should_have_stored_the_concurrency_token_in_the_index_table =
                () => TestContext.DbHelper.should_have_stored_member_in_indexes_table<ModelWithGuidToken>(_structureSchema, _orgItem.Id, m => m.ConcurrencyToken, typeof(Guid));

            private static IStructureSchema _structureSchema;
            private static ModelWithGuidToken _orgItem;
            private static ModelWithGuidToken _copy1, _copy2;
        }

        [Subject(typeof(ISession), "Update (concurrencies)")]
        public class when_item_has_enabled_int_concurrency_check_and_update_is_latest : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<ModelWithIntToken>();
                _orgItem = new ModelWithIntToken { StringValue = "Org string", IntValue = 42 };
                TestContext.Database.UseOnceTo().Insert(_orgItem);
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    _copy1 = session.GetById<ModelWithIntToken>(_orgItem.Id);
                    _copy1.StringValue = "From copy 1";
                    session.Update(_copy1);

                    _copy2 = session.GetById<ModelWithIntToken>(_orgItem.Id);
                    _copy2.StringValue = "From copy 2";
                    session.Update(_copy2);
                }
            };

            It should_only_have_one_item_stored =
                () => TestContext.Database.should_have_X_num_of_items<ModelWithIntToken>(1);

            It should_not_have_changed_ids_of_the_structure_in_database =
                () => TestContext.Database.should_have_ids<ModelWithIntToken>(_orgItem.Id);

            It should_have_updated_values_in_database =
                () => TestContext.Database.should_have_identical_structures(new[] { _copy2 });

            It should_have_updated_the_concurrency_token = () =>
            {
                _copy1.ConcurrencyToken = 1;
                _copy2.ConcurrencyToken = 2;
            };

            It should_have_stored_the_concurrency_token_in_the_index_table =
                () => TestContext.DbHelper.should_have_stored_member_in_indexes_table<ModelWithIntToken>(_structureSchema, _orgItem.Id, m => m.ConcurrencyToken, typeof(int));

            private static IStructureSchema _structureSchema;
            private static ModelWithIntToken _orgItem;
            private static ModelWithIntToken _copy1, _copy2;
        }

        [Subject(typeof(ISession), "Update (concurrencies)")]
        public class when_item_has_enabled_long_concurrency_check_and_update_is_latest : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<ModelWithLongToken>();
                _orgItem = new ModelWithLongToken { StringValue = "Org string", IntValue = 42 };
                TestContext.Database.UseOnceTo().Insert(_orgItem);
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    _copy1 = session.GetById<ModelWithLongToken>(_orgItem.Id);
                    _copy1.StringValue = "From copy 1";
                    session.Update(_copy1);

                    _copy2 = session.GetById<ModelWithLongToken>(_orgItem.Id);
                    _copy2.StringValue = "From copy 2";
                    session.Update(_copy2);
                }
            };

            It should_only_have_one_item_stored =
                () => TestContext.Database.should_have_X_num_of_items<ModelWithLongToken>(1);

            It should_not_have_changed_ids_of_the_structure_in_database =
                () => TestContext.Database.should_have_ids<ModelWithLongToken>(_orgItem.Id);

            It should_have_updated_values_in_database =
                () => TestContext.Database.should_have_identical_structures(new[] { _copy2 });

            It should_have_updated_the_concurrency_token = () =>
            {
                _copy1.ConcurrencyToken = 1;
                _copy2.ConcurrencyToken = 2;
            };

            It should_have_stored_the_concurrency_token_in_the_index_table =
                () => TestContext.DbHelper.should_have_stored_member_in_indexes_table<ModelWithLongToken>(_structureSchema, _orgItem.Id, m => m.ConcurrencyToken, typeof(long));

            private static IStructureSchema _structureSchema;
            private static ModelWithLongToken _orgItem;
            private static ModelWithLongToken _copy1, _copy2;
        }

        [Subject(typeof(ISession), "Update inline (concurrencies)")]
        public class when_updating_using_inline_modifier_and_item_has_enabled_concurrency_check_and_update_is_latest : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _orgItem = new ModelWithGuidToken { StringValue = "Org string", IntValue = 42 };
                TestContext.Database.UseOnceTo().Insert(_orgItem);
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    session.Update<ModelWithGuidToken>(_orgItem.Id, x =>
                    {
                        x.StringValue = "From first update";
                        x.IntValue = 142;
                    });
                    session.Update<ModelWithGuidToken>(_orgItem.Id, x => x.StringValue = "From second update");
                }
            };

            It should_only_have_one_item_stored =
                () => TestContext.Database.should_have_X_num_of_items<ModelWithGuidToken>(1);

            It should_not_have_changed_ids_of_the_structure_in_database =
                () => TestContext.Database.should_have_ids<ModelWithGuidToken>(_orgItem.Id);

            It should_have_updated_values_from_both_updates = () =>
            {
                var refetched = TestContext.Database.UseOnceTo().GetById<ModelWithGuidToken>(_orgItem.Id);
                refetched.IntValue.ShouldEqual(142);
                refetched.StringValue.ShouldEqual("From second update");
            };

            private static ModelWithGuidToken _orgItem;
        }

        [Subject(typeof(ISession), "Update inline (concurrencies)")]
        public class when_updating_interface_using_inline_modifier_and_item_has_enabled_concurrency_check_and_update_is_latest : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _orgItem = new ModelWithGuidToken { StringValue = "Org string", IntValue = 42 };
                TestContext.Database.UseOnceTo().Insert<IModelWithGuidToken>(_orgItem);
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    session.Update<IModelWithGuidToken, ModelWithGuidToken>(_orgItem.Id, x =>
                    {
                        x.StringValue = "From first update";
                        x.IntValue = 142;
                    });
                    session.Update<IModelWithGuidToken, ModelWithGuidToken>(_orgItem.Id, x => x.StringValue = "From second update");
                }
            };

            It should_only_have_one_item_stored =
                () => TestContext.Database.should_have_X_num_of_items<IModelWithGuidToken>(1);

            It should_not_have_changed_ids_of_the_structure_in_database =
                () => TestContext.Database.should_have_ids<IModelWithGuidToken, ModelWithGuidToken>(_orgItem.Id);

            It should_have_updated_values_from_both_updates = () =>
            {
                var refetched = TestContext.Database.UseOnceTo().GetByIdAs<IModelWithGuidToken, ModelWithGuidToken>(_orgItem.Id);
                refetched.IntValue.ShouldEqual(142);
                refetched.StringValue.ShouldEqual("From second update");
            };

            private static ModelWithGuidToken _orgItem;
        }

        [Subject(typeof(ISession), "Update inline (concurrencies)")]
        public class when_updating_using_inline_modifier_and_item_has_enabled_concurrency_check_and_update_is_latest_but_process_is_manually_aborted : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _orgItem = new ModelWithGuidToken { StringValue = "Org string", IntValue = 42 };
                TestContext.Database.UseOnceTo().Insert(_orgItem);
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    session.Update<ModelWithGuidToken>(_orgItem.Id, x =>
                    {
                        x.StringValue = "From first update";
                        x.IntValue = 142;
                    });
                    session.Update<ModelWithGuidToken>(_orgItem.Id, x => x.StringValue = "From second update", x => false);
                }
            };

            It should_only_have_one_item_stored =
                () => TestContext.Database.should_have_X_num_of_items<ModelWithGuidToken>(1);

            It should_not_have_changed_ids_of_the_structure_in_database =
                () => TestContext.Database.should_have_ids<ModelWithGuidToken>(_orgItem.Id);

            It should_only_have_updated_values_from_first_update = () =>
            {
                var refetched = TestContext.Database.UseOnceTo().GetById<ModelWithGuidToken>(_orgItem.Id);
                refetched.IntValue.ShouldEqual(142);
                refetched.StringValue.ShouldEqual("From first update");
            };

            private static ModelWithGuidToken _orgItem;
        }

        [Subject(typeof(ISession), "Update inline (concurrencies)")]
        public class when_updating_using_inline_modifier_and_other_session_tries_to_read_the_same_item : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _orgItem = new ModelWithGuidToken { StringValue = "Org string", IntValue = 42 };
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
                                session.Update<ModelWithGuidToken>(_orgItem.Id, x =>
                                {
                                    semaphore.Release();
                                    Task.WaitAll(task2);

                                    x.StringValue = "I am updated";
                                    x.IntValue = 142;
                                });
                            }
                        });

                        task2 = new Task(() =>
                        {
                            semaphore.Wait();
                            CaughtException = Catch.Exception(() => _getByIdResult = TestContext.Database.UseOnceTo().GetById<ModelWithGuidToken>(_orgItem.Id));
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

#if Sql2005Provider || Sql2008Provider || Sql2012Provider || SqlProfilerProvider
            It should_have_thrown_a_timeout_exception = () =>
            {
                CaughtException.ShouldNotBeNull();
                CaughtException.Message.ShouldContain("Timeout expired");
            };
#endif

#if SqlCe4Provider
            It should_not_have_thrown_a_timeout_exception = () => CaughtException.ShouldBeNull();
#endif

            It should_only_have_one_item_stored =
                () => TestContext.Database.should_have_X_num_of_items<ModelWithGuidToken>(1);

            It should_not_have_changed_ids_of_the_structure_in_database =
                () => TestContext.Database.should_have_ids<ModelWithGuidToken>(_orgItem.Id);

#if Sql2005Provider || Sql2008Provider || Sql2012Provider || SqlProfilerProvider
            It should_not_have_returned_item_for_concurrent_get_by_id = () => _getByIdResult.ShouldBeNull();
#endif

#if SqlCe4Provider
            It should_have_returned_item_for_concurrent_get_by_id = () => _getByIdResult.ShouldNotBeNull();
#endif

            It should_have_updated_values_from_update = () =>
            {
                var refetched = TestContext.Database.UseOnceTo().GetById<ModelWithGuidToken>(_orgItem.Id);
                refetched.IntValue.ShouldEqual(142);
                refetched.StringValue.ShouldEqual("I am updated");
            };

            private static ModelWithGuidToken _orgItem;
            private static ModelWithGuidToken _getByIdResult;
        }

        [Subject(typeof(ISession), "Update inline (concurrencies)")]
        public class when_updating_using_inline_modifier_and_other_session_tries_to_do_inline_update_of_the_same_item : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _orgItem = new ModelWithGuidToken { StringValue = "Org string", IntValue = 42 };
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
                                session.Update<ModelWithGuidToken>(_orgItem.Id, x =>
                                {
                                    semaphore.Release();
                                    Task.WaitAll(task2);

                                    x.StringValue = "I am updated";
                                    x.IntValue = 142;
                                });
                            }
                        });

                        task2 = new Task(() =>
                        {
                            semaphore.Wait();
                            CaughtException = Catch.Exception(() => TestContext.Database.UseOnceTo().Update<ModelWithGuidToken>(_orgItem.Id, x => x.StringValue = "I am a nasty concurrent update."));
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

#if Sql2005Provider || Sql2008Provider || Sql2012Provider || SqlProfilerProvider
            It should_have_thrown_a_timeout_exception = () =>
            {
                CaughtException.ShouldNotBeNull();
                CaughtException.Message.ShouldContain("Timeout expired");
            };
#endif

#if SqlCe4Provider
            It should_have_thrown_a_timeout_exception = () =>
            {
                CaughtException.ShouldNotBeNull();
                CaughtException.Message.ShouldContain("SQL Server Compact timed out waiting for a lock");
            };
#endif

            It should_only_have_one_item_stored =
                () => TestContext.Database.should_have_X_num_of_items<ModelWithGuidToken>(1);

            It should_not_have_changed_ids_of_the_structure_in_database =
                () => TestContext.Database.should_have_ids<ModelWithGuidToken>(_orgItem.Id);

            It should_have_updated_values_from_update = () =>
            {
                var refetched = TestContext.Database.UseOnceTo().GetById<ModelWithGuidToken>(_orgItem.Id);
                refetched.IntValue.ShouldEqual(142);
                refetched.StringValue.ShouldEqual("I am updated");
            };

            private static ModelWithGuidToken _orgItem;
        }

        [Subject(typeof(ISession), "Update (concurrencies)")]
        public class when_item_is_interface_and_has_enabled_concurrency_check_and_update_is_latest : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<IModelWithGuidToken>();
                _orgItem = new ModelWithGuidToken { StringValue = "Org string", IntValue = 42 };
                TestContext.Database.UseOnceTo().Insert<IModelWithGuidToken>(_orgItem);
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    _copy1 = session.GetByIdAs<IModelWithGuidToken, ModelWithGuidToken>(_orgItem.Id);
                    _copy1.StringValue = "From copy 1";
                    session.Update<IModelWithGuidToken>(_copy1);

                    _copy2 = session.GetByIdAs<IModelWithGuidToken, ModelWithGuidToken>(_orgItem.Id);
                    _copy2.StringValue = "From copy 2";
                    session.Update<IModelWithGuidToken>(_copy2);
                }
            };

            It should_only_have_one_item_stored =
                () => TestContext.Database.should_have_X_num_of_items<IModelWithGuidToken>(1);

            It should_not_have_changed_ids_of_the_structure_in_database =
                () => TestContext.Database.should_have_ids<IModelWithGuidToken, ModelWithGuidToken>(_orgItem.Id);

            It should_have_updated_values_in_database =
                () => TestContext.Database.should_have_identical_structures<IModelWithGuidToken, ModelWithGuidToken>(new[] { _copy2 });

            It should_have_stored_the_concurrency_token_in_the_index_table =
                () => TestContext.DbHelper.should_have_stored_member_in_indexes_table<IModelWithGuidToken>(_structureSchema, _orgItem.Id, m => m.ConcurrencyToken, typeof(Guid));

            private static IStructureSchema _structureSchema;
            private static ModelWithGuidToken _orgItem;
            private static ModelWithGuidToken _copy1, _copy2;
        }

        private interface IModelWithGuidToken
        {
            Guid Id { get; set; }
            Guid ConcurrencyToken { get; set; }
            string StringValue { get; set; }
            int IntValue { get; set; }
        }

        private class ModelWithGuidToken : IModelWithGuidToken
        {
            public Guid Id { get; set; }
            public Guid ConcurrencyToken { get; set; }
            public string StringValue { get; set; }
            public int IntValue { get; set; }
        }

        private class ModelWithIntToken
        {
            public Guid Id { get; set; }
            public int ConcurrencyToken { get; set; }
            public string StringValue { get; set; }
            public int IntValue { get; set; }
        }

        private class ModelWithLongToken
        {
            public Guid Id { get; set; }
            public long ConcurrencyToken { get; set; }
            public string StringValue { get; set; }
            public int IntValue { get; set; }
        }
    }
}