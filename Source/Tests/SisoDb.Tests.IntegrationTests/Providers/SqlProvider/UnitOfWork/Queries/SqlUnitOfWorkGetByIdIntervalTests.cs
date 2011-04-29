using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SisoDb.TestUtils;

namespace SisoDb.Tests.IntegrationTests.Providers.SqlProvider.UnitOfWork.Queries
{
    [TestFixture]
    public class SqlUnitOfWorkGetByIdIntervalTests : SqlIntegrationTestBase
    {
        protected override void OnTestFinalize()
        {
            DropStructureSet<IdentityItem>();
            DropStructureSet<GuidItem>();
        }

        [Test]
        public void GetByIdInterval_ForIdentities_WhenNoItemsExists_ReturnsEmptyResult()
        {
            using (var uow = Database.CreateUnitOfWork())
            {
                var refetched = uow.GetByIdInterval<IdentityItem>(1, 3);
                
                Assert.AreEqual(0, refetched.Count());
            }
        }

        [Test]
        public void GetByIdInterval_ForGuids_WhenNoItemsExists_ReturnsEmptyResult()
        {
            var idFrom = SequentialGuid.NewSqlCompatibleGuid();
            var idTo = SequentialGuid.NewSqlCompatibleGuid();

            using (var uow = Database.CreateUnitOfWork())
            {
                var refetched = uow.GetByIdInterval<GuidItem>(idFrom, idTo);

                Assert.AreEqual(0, refetched.Count());
            }
        }

        [Test]
        public void GetByIdInterval_ForIdentities_WhenIntervalMatchesOnlyASubset_OnlySubsetIsReturned()
        {
            var items = new List<IdentityItem>
                        {
                            new IdentityItem{SortOrder = 1, Value = "A"},
                            new IdentityItem{SortOrder = 2, Value = "B"},
                            new IdentityItem{SortOrder = 3, Value = "C"},
                            new IdentityItem{SortOrder = 4, Value = "D"},
                        };

            IList<IdentityItem> refetched;

            using (var uow = Database.CreateUnitOfWork())
            {
                uow.InsertMany(items);
                uow.Commit();

                refetched = uow.GetByIdInterval<IdentityItem>(1, 3).ToList();
            }

            Assert.AreEqual(3, refetched.Count);
            CustomAssert.AreValueEqual(items[0], refetched[0]);
            CustomAssert.AreValueEqual(items[1], refetched[1]);
            CustomAssert.AreValueEqual(items[2], refetched[2]);
        }

        [Test]
        public void GetByIdInterval_ForGuids_WhenIntervalMatchesOnlyASubset_OnlySubsetIsReturned()
        {
            var guids = new[]
                            {
                                SequentialGuid.NewSqlCompatibleGuid(),
                                SequentialGuid.NewSqlCompatibleGuid(),
                                SequentialGuid.NewSqlCompatibleGuid(),
                                SequentialGuid.NewSqlCompatibleGuid()
                            };
            var items = new List<GuidItem>
                            {
                                new GuidItem{SisoId = guids[0], SortOrder = 1, Value = "A"},
                                new GuidItem{SisoId = guids[1], SortOrder = 2, Value = "B"},
                                new GuidItem{SisoId = guids[2], SortOrder = 3, Value = "C"},
                                new GuidItem{SisoId = guids[3], SortOrder = 4, Value = "D"},
                            };

            IList<GuidItem> refetched;

            using (var uow = Database.CreateUnitOfWork())
            {
                uow.InsertMany(items);
                uow.Commit();

                refetched = uow.GetByIdInterval<GuidItem>(guids[0], guids[2]).ToList();
            }

            Assert.AreEqual(3, refetched.Count);
            CustomAssert.AreValueEqual(items[0], refetched[0]);
            CustomAssert.AreValueEqual(items[1], refetched[1]);
            CustomAssert.AreValueEqual(items[2], refetched[2]);
        }
        
        private class IdentityItem
        {
            public int SisoId { get; set; }

            public int SortOrder { get; set; }

            public string Value { get; set; }
        }

        private class GuidItem
        {
            public Guid SisoId { get; set; }

            public int SortOrder { get; set; }

            public string Value { get; set; }
        }
    }
}