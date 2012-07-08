using System.Transactions;
using Machine.Specifications;
using SisoDb.Testing;
using SisoDb.Testing.TestModel;
using SisoDb.Testing.Steps;

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

        [Subject(typeof(ITransactionalSession), "Mark as failed")]
        public class when_manually_marking_a_transactional_session_as_failed_after_inserts : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _session = TestContext.Database.BeginSession();
                _session.InsertMany(new[]
                {
                    new GuidItem {Value = 1},
                    new GuidItem {Value = 2},
                    new GuidItem {Value = 3}
                });
            };

            Because of = () =>
            {
                ((ITransactionalSession)_session).MarkAsFailed();
                _session.Dispose();
            };

            It should_not_have_inserted_the_structures = () =>
                TestContext.Database.should_have_none_items_left<GuidItem>();

            It should_have_a_failed_session = () => 
                ((ITransactionalSession) _session).Failed.ShouldBeTrue();

            It should_have_a_session_with_failed_status = () =>
                _session.Status.IsFailed().ShouldBeTrue();

            Cleanup after = () =>
            {
                if(_session == null)
                    return;

                if(!_session.Status.IsDisposed())
                    _session.Dispose();

                _session = null;
            };

            private static ISession _session;
        }
    }
#endif
}