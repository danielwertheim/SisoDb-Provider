using System.Transactions;
using Machine.Specifications;
using SisoDb.Testing;
using SisoDb.Testing.TestModel;

namespace SisoDb.Specifications.UnitOfWork
{
    namespace TransactionScopes
    {
        [Subject(typeof(IUnitOfWork), "Transaction scopes")]
        public class when_nesting_multiple_unit_of_works_in_ts_wihtout_commiting_ts : SpecificationBase
        {
            Establish context = () => TestContext = TestContextFactory.Create();

            Because of = () =>
            {
                using (var ts = new TransactionScope())
                {
                    using (var uow = TestContext.Database.CreateUnitOfWork())
                    {
                        uow.InsertMany(new[]
                        {
                            new IdentityItem { Value = 1 }, 
                            new IdentityItem { Value = 2 }, 
                            new IdentityItem { Value = 3 }
                        });
                        uow.Commit();
                    }

                    using (var uow = TestContext.Database.CreateUnitOfWork())
                    {
                        uow.InsertMany(new[]
                        {
                            new IdentityItem { Value = 4 }, 
                            new IdentityItem { Value = 5 }, 
                            new IdentityItem { Value = 6 }
                        });
                        uow.Commit();
                    }
                }
            };

            It should_not_have_inserted_anything = () =>
            {
                using (var qe = TestContext.Database.CreateQueryEngine())
                {
                    qe.Count<IdentityItem>().ShouldEqual(0);
                }
            };
        }

        [Subject(typeof(IUnitOfWork), "Transaction scopes")]
        public class when_nesting_multiple_unit_of_works_in_ts_wihtout_committing_uow_but_committing_ts : SpecificationBase
        {
            Establish context = () => TestContext = TestContextFactory.Create();

            Because of = () =>
            {
                using (var ts = new TransactionScope())
                {
                    using (var uow = TestContext.Database.CreateUnitOfWork())
                    {
                        uow.InsertMany(new[]
                        {
                            new IdentityItem { Value = 1 }, 
                            new IdentityItem { Value = 2 }, 
                            new IdentityItem { Value = 3 }
                        });
                    }

                    using (var uow = TestContext.Database.CreateUnitOfWork())
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
                using (var qe = TestContext.Database.CreateQueryEngine())
                {
                    qe.Count<IdentityItem>().ShouldEqual(6);
                }
            };
        }
    }
}