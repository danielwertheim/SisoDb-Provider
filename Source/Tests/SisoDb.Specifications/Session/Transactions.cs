using System.Transactions;
using Machine.Specifications;
using SisoDb.NCore;
using SisoDb.Testing;
using SisoDb.Testing.TestModel;
using SisoDb.Testing.Steps;
using SisoDb.Resources;

namespace SisoDb.Specifications.Session
{
#if Sql2005Provider || Sql2008Provider || Sql2012Provider || SqlProfilerProvider
    class Transactions
    {
        [Subject(typeof(ISession), "Transaction scopes")]
        public class when_nesting_multiple_unit_of_works_in_ts_wihtout_commiting_ts : SpecificationBase
        {
            Establish context = () => TestContext = TestContextFactory.Create();

            private Because of = () =>
            {
                using (var t = new TransactionScope(TransactionScopeOption.Required))
                {
                    using (var session = TestContext.Database.BeginSession())
                    {
                        session.InsertMany(new[]
                        {
                            new IdentityItem {Value = 1},
                            new IdentityItem {Value = 2},
                            new IdentityItem {Value = 3}
                        });
                    }

                    using (var session = TestContext.Database.BeginSession())
                    {
                        session.InsertMany(new[]
                        {
                            new IdentityItem {Value = 4},
                            new IdentityItem {Value = 5},
                            new IdentityItem {Value = 6}
                        });
                    }
                }
            };

            It should_not_have_inserted_anything = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    session.Query<IdentityItem>().Count().ShouldEqual(0);
                }
            };
        }

        [Subject(typeof(ISession), "Transaction scopes")]
        public class when_nesting_multiple_unit_of_works_in_ts_wihtout_committing_uow_but_committing_ts : SpecificationBase
        {
            Establish context = () => TestContext = TestContextFactory.Create();

            private Because of = () =>
            {
                using (var t = new TransactionScope(TransactionScopeOption.Required))
                {
                    using (var session = TestContext.Database.BeginSession())
                    {
                        session.InsertMany(new[]
                        {
                            new IdentityItem {Value = 1},
                            new IdentityItem {Value = 2},
                            new IdentityItem {Value = 3}
                        });
                    }

                    using (var session = TestContext.Database.BeginSession())
                    {
                        session.InsertMany(new[]
                        {
                            new IdentityItem {Value = 4},
                            new IdentityItem {Value = 5},
                            new IdentityItem {Value = 6}
                        });
                    }
                    t.Complete();
                }
            };

            It should_not_have_inserted_anything = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    session.Query<IdentityItem>().Count().ShouldEqual(6);
                }
            };
        }
#endif
        [Subject(typeof(ISession), "Mark as failed")]
        public class when_manually_marking_a_transactional_session_as_failed_after_inserts : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _session = (ISession)TestContext.Database.BeginSession();
                _session.InsertMany(new[]
                {
                    new GuidItem {Value = 1},
                    new GuidItem {Value = 2},
                    new GuidItem {Value = 3}
                });
            };

            Because of = () =>
            {
                _session.MarkAsFailed();
                _session.Dispose();
            };

            It should_not_have_inserted_the_structures = () =>
                TestContext.Database.should_have_none_items_left<GuidItem>();

            It should_have_a_failed_session = () =>
                _session.HasFailed.ShouldBeTrue();

            It should_have_a_session_with_failed_status = () =>
                _session.Status.IsFailed().ShouldBeTrue();

            Cleanup after = () =>
            {
                if (_session == null)
                    return;

                if (!_session.Status.IsDisposed())
                    _session.Dispose();

                _session = null;
            };

            private static ISession _session;
        }

        [Subject(typeof(ISession), "Being used after failed")]
        public class when_transactional_session_allready_has_been_marked_as_failed : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _session = (ISession)TestContext.Database.BeginSession();
                _session.InsertMany(new[]
                {
                    new GuidItem {Value = 1},
                    new GuidItem {Value = 2},
                    new GuidItem {Value = 3}
                });
                _session.MarkAsFailed();
            };

            Because of = () =>
            {
                CaughtException = Catch.Exception(() => _session.Insert(new GuidItem { Value = 42 }));
                _session.Dispose();
            };

            It should_have_thrown_an_exception = () => 
            {
                CaughtException.ShouldNotBeNull();
                CaughtException.Message.ShouldEqual(ExceptionMessages.Session_AlreadyFailed.Inject(_session.Id, _session.Db.Name));
            };

            It should_not_have_inserted_the_structures = () =>
                TestContext.Database.should_have_none_items_left<GuidItem>();

            It should_have_a_failed_session = () =>
                _session.HasFailed.ShouldBeTrue();

            It should_have_a_session_with_failed_status = () =>
                _session.Status.IsFailed().ShouldBeTrue();

            Cleanup after = () =>
            {
                if (_session == null)
                    return;

                if (!_session.Status.IsDisposed())
                    _session.Dispose();

                _session = null;
            };

            private static ISession _session;
        }

        [Subject(typeof(ISession), "Abort")]
        public class when_manually_marking_a_transactional_session_as_aborted_after_inserts : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _session = (ISession)TestContext.Database.BeginSession();
                _session.InsertMany(new[]
                {
                    new GuidItem {Value = 1},
                    new GuidItem {Value = 2},
                    new GuidItem {Value = 3}
                });
            };

            Because of = () =>
            {
                _session.Abort();
                _session.Dispose();
            };

            It should_not_have_inserted_the_structures = () =>
                TestContext.Database.should_have_none_items_left<GuidItem>();

            It should_have_a_aborted_the_session = () =>
                _session.IsAborted.ShouldBeTrue();

            It should_have_a_session_with_aborted_status = () =>
                _session.Status.IsAborted().ShouldBeTrue();

            Cleanup after = () =>
            {
                if (_session == null)
                    return;

                if (!_session.Status.IsDisposed())
                    _session.Dispose();

                _session = null;
            };

            private static ISession _session;
        }

        [Subject(typeof(ISession), "Being used after aborted")]
        public class when_transactional_session_allready_has_been_aborted : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _session = (ISession)TestContext.Database.BeginSession();
                _session.InsertMany(new[]
                {
                    new GuidItem {Value = 1},
                    new GuidItem {Value = 2},
                    new GuidItem {Value = 3}
                });
                _session.Abort();
            };

            Because of = () =>
            {
                CaughtException = Catch.Exception(() => _session.Insert(new GuidItem { Value = 42 }));
                _session.Dispose();
            };

            It should_not_have_thrown_an_exception = () => 
                CaughtException.ShouldBeNull();

            It should_not_have_inserted_the_structures = () =>
                TestContext.Database.should_have_none_items_left<GuidItem>();

            It should_have_an_aborted_session = () =>
                _session.IsAborted.ShouldBeTrue();

            It should_have_a_session_with_aborted_status = () =>
                _session.Status.IsAborted().ShouldBeTrue();

            Cleanup after = () =>
            {
                if (_session == null)
                    return;

                if (!_session.Status.IsDisposed())
                    _session.Dispose();

                _session = null;
            };

            private static ISession _session;
        }
    }
}