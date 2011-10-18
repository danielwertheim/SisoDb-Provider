using System.Collections.Generic;
using System.Transactions;
using Machine.Specifications;
using SisoDb.Sql2008;
using SisoDb.Testing;

namespace SisoDb.Specifications.Sql2008.UnitOfWork
{
    namespace TransactionScopes
    {
        [Subject(typeof(Sql2008UnitOfWork), "Transaction scopes")]
        public class when_nesting_multiple_unit_of_works_in_ts_wihtout_commiting_ts : SpecificationBase
        {
            Establish context = () => TestContext = TestContextFactory.Create(StorageProviders.Sql2008);

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

        [Subject(typeof(Sql2008UnitOfWork), "Transaction scopes")]
        public class when_nesting_multiple_unit_of_works_in_ts_wihtout_committing_uow_but_committing_ts : SpecificationBase
        {
            Establish context = () => TestContext = TestContextFactory.Create(StorageProviders.Sql2008);

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

        public class IdentityItem
        {
            public int StructureId { get; set; }

            public int Value { get; set; }
        }
    }
}