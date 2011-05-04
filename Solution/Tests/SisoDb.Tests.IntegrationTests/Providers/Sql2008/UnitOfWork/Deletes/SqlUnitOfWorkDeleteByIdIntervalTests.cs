using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SisoDb.TestUtils;

namespace SisoDb.Tests.IntegrationTests.Providers.Sql2008.UnitOfWork.Deletes
{
    [TestFixture]
    public class SqlUnitOfWorkDeleteByIdIntervalTests : SqlIntegrationTestBase
    {
        protected override void OnTestFinalize()
        {
            DropStructureSet<IdentityItem>();
            DropStructureSet<GuidItem>();
        }

        [Test]
        public void DeleteByIdInterval_ForIdentities_WhenNoItemsExists_NoExceptionIsThrown()
        {
            using (var uow = Database.CreateUnitOfWork())
            {
                uow.DeleteByIdInterval<IdentityItem>(1, 3);
                uow.Commit();

                Assert.AreEqual(0, uow.Count<IdentityItem>());
            }
        }

        [Test]
        public void DeleteByIdInterval_ForGuids_WhenNoItemsExists_NoExceptionIsThrown()
        {
            var idFrom = Guid.Parse("063EC0E2-30FC-4940-9A8B-7AB34B29350A");
            var idTo = Guid.Parse("A5B78C0E-256B-4DF1-8362-64C167D642C1");

            using (var uow = Database.CreateUnitOfWork())
            {
                uow.DeleteByIdInterval<GuidItem>(idFrom, idTo);
                uow.Commit();

                Assert.AreEqual(0, uow.Count<IdentityItem>());
            }
        }

        [Test]
        public void DeleteByIdInterval_ForIdentities_WhenIntervalMatchesOnlyASubset_OnlySubsetIsDeleted()
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

                uow.DeleteByIdInterval<IdentityItem>(1, 3);
                uow.Commit();

                refetched = uow.GetAll<IdentityItem>().ToList();
            }

            Assert.AreEqual(1, refetched.Count);
            CustomAssert.AreValueEqual(items[3], refetched[0]);
        }

        [Test]
        public void DeleteByIdInterval_ForGuids_WhenIntervalMatchesOnlyASubset_OnlySubsetIsDeleted()
        {
            var items = new List<GuidItem>
                        {
                            new GuidItem{SortOrder = 1, Value = "A"},
                            new GuidItem{SortOrder = 2, Value = "B"},
                            new GuidItem{SortOrder = 3, Value = "C"},
                            new GuidItem{SortOrder = 4, Value = "D"},
                        };

            IList<GuidItem> refetched;

            using (var uow = Database.CreateUnitOfWork())
            {
                uow.InsertMany(items);
                uow.Commit();

                uow.DeleteByIdInterval<GuidItem>(items[0].SisoId, items[2].SisoId);
                uow.Commit();

                refetched = uow.GetAll<GuidItem>().ToList();
            }

            Assert.AreEqual(1, refetched.Count);
            CustomAssert.AreValueEqual(items[3], refetched[0]);
        }

        [Test]
        public void DeleteByIdInterval_ForIdentities_WhenNoMatch_NothingIsDeleted()
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

                uow.DeleteByIdInterval<IdentityItem>(100, 200);
                uow.Commit();

                refetched = uow.GetAll<IdentityItem>().ToList();
            }

            Assert.AreEqual(4, refetched.Count);
        }

        [Test]
        public void DeleteByIdInterval_ForGuids_WhenNoMatch_NothingIsDeleted()
        {
            var guids = new[]
                        {
                            SequentialGuid.NewSqlCompatibleGuid(),
                            SequentialGuid.NewSqlCompatibleGuid(),
                            SequentialGuid.NewSqlCompatibleGuid(),
                            SequentialGuid.NewSqlCompatibleGuid(),
                            SequentialGuid.NewSqlCompatibleGuid(),//Extra #1 --> idFrom when deleting
                            SequentialGuid.NewSqlCompatibleGuid(),//Extra #2 --> idTo when deleting
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

                uow.DeleteByIdInterval<GuidItem>(guids[4], guids[5]);
                uow.Commit();

                refetched = uow.GetAll<GuidItem>().ToList();
            }

            Assert.AreEqual(4, refetched.Count);
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