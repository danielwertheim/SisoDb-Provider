using System.Collections.Generic;
using System.Transactions;
using NUnit.Framework;

namespace SisoDb.Tests.IntegrationTests.Providers.Sql2008.TransactionScopes
{
    [TestFixture]
    public class Sql2008UnitOfWorkWithTransactionScopeTests : Sql2008IntegrationTestBase
    {
        protected override void OnTestFinalize()
        {
            DropStructureSet<IdentityItem>();
        }

        [Test]
        public void TransactionScope_WhenNestingMultipleUnitOfWorks_NothingIsInserted()
        {
            var items1 = new List<IdentityItem>
            {
                new IdentityItem {Value = 1}, new IdentityItem {Value = 2}, new IdentityItem {Value = 3},
            };

            var items2 = new List<IdentityItem>
            {
                new IdentityItem {Value = 4}, new IdentityItem {Value = 5}, new IdentityItem {Value = 6},
            };

            using (var ts = new TransactionScope())
            {
                using (var uow = Database.CreateUnitOfWork())
                {
                    uow.InsertMany(items1);
                    uow.Commit();
                }

                using (var uow = Database.CreateUnitOfWork())
                {
                    uow.InsertMany(items2);
                    uow.Commit();
                }

                ts.Complete();
            }

            using (var qe = Database.CreateQueryEngine())
            {
                Assert.AreEqual(6, qe.Count<IdentityItem>());
            }
        }

        [Test]
        public void TransactionScope_WhenCommittingUnitOfWorkButNotTransactionScope_NothingIsInserted()
        {
            var items = new List<IdentityItem>
                        {
                            new IdentityItem {Value = 1}, new IdentityItem {Value = 2}, new IdentityItem {Value = 3},
                        };

            using(var ts = new TransactionScope())
            {
                using(var uow = Database.CreateUnitOfWork())
                {
                    uow.InsertMany(items);
                    uow.Commit();
                }
            }

            using (var qe = Database.CreateQueryEngine())
            {
                Assert.AreEqual(0, qe.Count<IdentityItem>());
            }
        }

        [Test]
        public void TransactionScope_WhenNotCommittingUnitOfWorkAndNotTransactionScope_NothingIsInserted()
        {
            var items = new List<IdentityItem>
                        {
                            new IdentityItem {Value = 1}, new IdentityItem {Value = 2}, new IdentityItem {Value = 3},
                        };

            using (var ts = new TransactionScope())
            {
                using (var uow = Database.CreateUnitOfWork())
                {
                    uow.InsertMany(items);
                }
            }

            using (var qe = Database.CreateQueryEngine())
            {
                Assert.AreEqual(0, qe.Count<IdentityItem>());
            }
        }

        [Test]
        public void TransactionScope_WhenNotCommittingUnitOfWorkButTransactionScope_ItemsAreInserted()
        {
            var items = new List<IdentityItem>
                        {
                            new IdentityItem {Value = 1}, new IdentityItem {Value = 2}, new IdentityItem {Value = 3},
                        };

            using (var ts = new TransactionScope())
            {
                using (var uow = Database.CreateUnitOfWork())
                {
                    uow.InsertMany(items);
                }

                ts.Complete();
            }

            using (var qe = Database.CreateQueryEngine())
            {
                Assert.AreEqual(3, qe.Count<IdentityItem>());
            }
        }

        [Test]
        public void TransactionScope_WhenCommittingBothUnitOfWorkAndTransactionScope_ItemsAreInserted()
        {
            var items = new List<IdentityItem>
                        {
                            new IdentityItem {Value = 1}, new IdentityItem {Value = 2}, new IdentityItem {Value = 3},
                        };

            using (var ts = new TransactionScope())
            {
                using (var uow = Database.CreateUnitOfWork())
                {
                    uow.InsertMany(items);
                    uow.Commit();
                }

                ts.Complete();
            }

            using (var qe = Database.CreateQueryEngine())
            {
                Assert.AreEqual(3, qe.Count<IdentityItem>());
            }
        }

        private class IdentityItem
        {
            public int SisoId { get; set; }

            public int Value { get; set; }
        }
    }
}