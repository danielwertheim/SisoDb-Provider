using System.Transactions;
using Machine.Specifications;
using SisoDb.Testing;
using SisoDb.Testing.TestModel;

namespace SisoDb.Specifications.UnitOfWork
{
#if Sql2008Provider
	class TransactionScopes
    {
        [Subject(typeof(IWriteSession), "Transaction scopes")]
        public class when_nesting_multiple_unit_of_works_in_ts_wihtout_commiting_ts : SpecificationBase
        {
            Establish context = () => TestContext = TestContextFactory.Create();

            Because of = () =>
            {
                using (var ts = new TransactionScope())
                {
                    using (var uow = TestContext.Database.BeginWriteSession())
                    {
                        uow.InsertMany(new[]
                        {
                            new IdentityItem { Value = 1 }, 
                            new IdentityItem { Value = 2 }, 
                            new IdentityItem { Value = 3 }
                        });
                    }

                    using (var uow = TestContext.Database.BeginWriteSession())
                    {
                        uow.InsertMany(new[]
                        {
                            new IdentityItem { Value = 4 }, 
                            new IdentityItem { Value = 5 }, 
                            new IdentityItem { Value = 6 }
                        });
                    }
                }
            };

            It should_not_have_inserted_anything = () =>
            {
                using (var qe = TestContext.Database.BeginReadSession())
                {
					qe.Query<IdentityItem>().Count().ShouldEqual(0);
                }
            };
        }

        [Subject(typeof(IWriteSession), "Transaction scopes")]
        public class when_nesting_multiple_unit_of_works_in_ts_wihtout_committing_uow_but_committing_ts : SpecificationBase
        {
            Establish context = () => TestContext = TestContextFactory.Create();

            Because of = () =>
            {
                using (var ts = new TransactionScope())
                {
                    using (var uow = TestContext.Database.BeginWriteSession())
                    {
                        uow.InsertMany(new[]
                        {
                            new IdentityItem { Value = 1 }, 
                            new IdentityItem { Value = 2 }, 
                            new IdentityItem { Value = 3 }
                        });
                    }

                    using (var uow = TestContext.Database.BeginWriteSession())
                    {
                        uow.InsertMany(new[]
                        {
                            new IdentityItem { Value = 4 }, 
                            new IdentityItem { Value = 5 }, 
                            new IdentityItem { Value = 6 }
                        });
                    }

                    ts.Complete();
                }
            };

            It should_not_have_inserted_anything = () =>
            {
                using (var qe = TestContext.Database.BeginReadSession())
                {
					qe.Query<IdentityItem>().Count().ShouldEqual(6);
                }
            };
        }
    }
#endif
}