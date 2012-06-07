﻿using System.Transactions;
using Machine.Specifications;
using SisoDb.Testing;
using SisoDb.Testing.TestModel;

namespace SisoDb.Specifications.Session
{
#if Sql2008Provider || Sql2012Provider || SqlProfilerProvider
    class TransactionScopes
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
                using (var session =TestContext.Database.BeginSession())
                {
					session.Query<IdentityItem>().Count().ShouldEqual(6);
                }
            };
        }
    }
#endif
}