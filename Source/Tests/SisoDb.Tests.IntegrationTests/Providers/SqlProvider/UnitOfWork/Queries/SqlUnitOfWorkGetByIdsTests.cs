using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SisoDb.TestUtils;

namespace SisoDb.Tests.IntegrationTests.Providers.SqlProvider.UnitOfWork.Queries
{
    [TestFixture]
    public class SqlUnitOfWorkGetByIdsTests : SqlIntegrationTestBase
    {
        protected override void OnTestFinalize()
        {
            DropStructureSet<IdentityItemForGetQueries>();
            DropStructureSet<GuidItemForGetQueries>();
        }

        [Test]
        public void GetByIds_WhenSeveralIdentityIds_MatchingSubsetIsReturned()
        {
            var items = new List<IdentityItemForGetQueries>
                        {
                            new IdentityItemForGetQueries {SortOrder = 1, StringValue = "A"},
                            new IdentityItemForGetQueries {SortOrder = 2, StringValue = "B"},
                            new IdentityItemForGetQueries {SortOrder = 3, StringValue = "C"}
                        };
            List<IdentityItemForGetQueries> refetched;

            using (var uow = Database.CreateUnitOfWork())
            {
                uow.InsertMany(items);
                uow.Commit();

                refetched = uow.GetByIds<IdentityItemForGetQueries>(new[] { 1, 3 }).ToList();
            }

            Assert.AreEqual(2, refetched.Count);
            CustomAssert.AreValueEqual(items[0], refetched[0]);
            CustomAssert.AreValueEqual(items[2], refetched[1]);
        }

        [Test]
        public void GetByIds_WhenSpecifyingNonExistingIdentity_MatchingSubsetIsReturned()
        {
            var nonExistingIdentity = 99;
            var items = new List<IdentityItemForGetQueries>
                        {
                            new IdentityItemForGetQueries {SortOrder = 1, StringValue = "A"},
                            new IdentityItemForGetQueries {SortOrder = 2, StringValue = "B"},
                            new IdentityItemForGetQueries {SortOrder = 3, StringValue = "C"}
                        };
            List<IdentityItemForGetQueries> refetched;

            using (var uow = Database.CreateUnitOfWork())
            {
                uow.InsertMany(items);
                uow.Commit();

                refetched = uow.GetByIds<IdentityItemForGetQueries>(new[] { 1, 3, nonExistingIdentity }).ToList();
            }

            Assert.AreEqual(2, refetched.Count);
            CustomAssert.AreValueEqual(items[0], refetched[0]);
            CustomAssert.AreValueEqual(items[2], refetched[1]);
        }

        [Test]
        public void GetByIdsAsJson_WhenSeveralIdentityIds_MatchingSubsetIsReturned()
        {
            var items = new List<IdentityItemForGetQueries>
                        {
                            new IdentityItemForGetQueries {SortOrder = 1, StringValue = "A"},
                            new IdentityItemForGetQueries {SortOrder = 2, StringValue = "B"},
                            new IdentityItemForGetQueries {SortOrder = 3, StringValue = "C"}
                        };
            List<string> refetched;

            using (var uow = Database.CreateUnitOfWork())
            {
                uow.InsertMany(items);
                uow.Commit();

                refetched = uow.GetByIdsAsJson<IdentityItemForGetQueries>(new[] { 1, 3 }).ToList();
            }

            Assert.AreEqual(2, refetched.Count);
            Assert.AreEqual("{\"SisoId\":1,\"SortOrder\":1,\"StringValue\":\"A\"}", refetched[0]);
            Assert.AreEqual("{\"SisoId\":3,\"SortOrder\":3,\"StringValue\":\"C\"}", refetched[1]);
        }

        [Test]
        public void GetByIdsAs_WhenSeveralIdentityIds_MatchingSubsetIsReturned()
        {
            var items = new List<IdentityItemForGetQueries>
                        {
                            new IdentityItemForGetQueries {SortOrder = 1, StringValue = "A"},
                            new IdentityItemForGetQueries {SortOrder = 2, StringValue = "B"},
                            new IdentityItemForGetQueries {SortOrder = 3, StringValue = "C"}
                        };
            List<View> refetched;

            using (var uow = Database.CreateUnitOfWork())
            {
                uow.InsertMany(items);
                uow.Commit();

                refetched = uow.GetByIdsAs<IdentityItemForGetQueries, View>(new[] { 1, 3 }).ToList();
            }

            Assert.AreEqual(2, refetched.Count);
            Assert.AreEqual(1, refetched[0].SortOrder);
            Assert.AreEqual("A", refetched[0].StringValue);
            Assert.AreEqual(3, refetched[1].SortOrder);
            Assert.AreEqual("C", refetched[1].StringValue);
        }

        [Test]
        public void GetByIds_WhenSeveralGuidIds_MatchingSubsetIsReturned()
        {
            var items = new List<GuidItemForGetQueries>
                        {
                            new GuidItemForGetQueries {SortOrder = 1, StringValue = "A"},
                            new GuidItemForGetQueries {SortOrder = 2, StringValue = "B"},
                            new GuidItemForGetQueries {SortOrder = 3, StringValue = "C"}
                        };
            List<GuidItemForGetQueries> refetched;

            using (var uow = Database.CreateUnitOfWork())
            {
                uow.InsertMany(items);
                uow.Commit();

                refetched = uow.GetByIds<GuidItemForGetQueries>(new[] { items[0].SisoId, items[2].SisoId }).ToList();
            }

            Assert.AreEqual(2, refetched.Count);
            CustomAssert.AreValueEqual(items[0], refetched[0]);
            CustomAssert.AreValueEqual(items[2], refetched[1]);
        }

        [Test]
        public void GetByIds_WhenSpecifyingNonExistingGuid_MatchingSubsetIsReturned()
        {
            var nonExistingGuid = Guid.Empty;
            var items = new List<GuidItemForGetQueries>
                        {
                            new GuidItemForGetQueries {SortOrder = 1, StringValue = "A"},
                            new GuidItemForGetQueries {SortOrder = 2, StringValue = "B"},
                            new GuidItemForGetQueries {SortOrder = 3, StringValue = "C"}
                        };
            List<GuidItemForGetQueries> refetched;

            using (var uow = Database.CreateUnitOfWork())
            {
                uow.InsertMany(items);
                uow.Commit();

                refetched = uow.GetByIds<GuidItemForGetQueries>(new[] { items[0].SisoId, items[2].SisoId, nonExistingGuid }).ToList();
            }

            Assert.AreEqual(2, refetched.Count);
            CustomAssert.AreValueEqual(items[0], refetched[0]);
            CustomAssert.AreValueEqual(items[2], refetched[1]);
        }

        [Test]
        public void GetByIdsAsJson_WhenSeveralGuidIds_MatchingSubsetIsReturned()
        {
            var ids = new[]
                      {
                          Guid.Parse("B5CB06F0-F853-4BF2-9BED-2B1E4D703A7A"),
                          Guid.Parse("158FD134-1A3A-462B-A11B-7853D9D5B668"),
                          Guid.Parse("5C3FFE86-21D3-4C60-9519-F6C198992F07")
                      };
            var items = new List<GuidItemForGetQueries>
                        {
                            new GuidItemForGetQueries {SisoId = ids[0], SortOrder = 1, StringValue = "A"},
                            new GuidItemForGetQueries {SisoId = ids[1], SortOrder = 2, StringValue = "B"},
                            new GuidItemForGetQueries {SisoId = ids[2], SortOrder = 3, StringValue = "C"}
                        };
            List<string> refetched;

            using (var uow = Database.CreateUnitOfWork())
            {
                uow.InsertMany(items);
                uow.Commit();

                refetched = uow.GetByIdsAsJson<GuidItemForGetQueries>(new[] { items[0].SisoId, items[2].SisoId }).ToList();
            }

            Assert.AreEqual(2, refetched.Count);
            Assert.AreEqual("{\"SisoId\":\"b5cb06f0f8534bf29bed2b1e4d703a7a\",\"SortOrder\":1,\"StringValue\":\"A\"}", refetched[0]);
            Assert.AreEqual("{\"SisoId\":\"5c3ffe8621d34c609519f6c198992f07\",\"SortOrder\":3,\"StringValue\":\"C\"}", refetched[1]);
        }

        [Test]
        public void GetByIdsAs_WhenSeveralGuidIds_MatchingSubsetIsReturned()
        {
            var ids = new[]
                      {
                          Guid.Parse("B5CB06F0-F853-4BF2-9BED-2B1E4D703A7A"),
                          Guid.Parse("158FD134-1A3A-462B-A11B-7853D9D5B668"),
                          Guid.Parse("5C3FFE86-21D3-4C60-9519-F6C198992F07")
                      };
            var items = new List<GuidItemForGetQueries>
                        {
                            new GuidItemForGetQueries {SisoId = ids[0], SortOrder = 1, StringValue = "A"},
                            new GuidItemForGetQueries {SisoId = ids[1], SortOrder = 2, StringValue = "B"},
                            new GuidItemForGetQueries {SisoId = ids[2], SortOrder = 3, StringValue = "C"}
                        };
            List<View> refetched;

            using (var uow = Database.CreateUnitOfWork())
            {
                uow.InsertMany(items);
                uow.Commit();

                refetched = uow.GetByIdsAs<GuidItemForGetQueries, View>(new[] { items[0].SisoId, items[2].SisoId }).ToList();
            }

            Assert.AreEqual(2, refetched.Count);
            Assert.AreEqual(1, refetched[0].SortOrder);
            Assert.AreEqual("A", refetched[0].StringValue);
            Assert.AreEqual(3, refetched[1].SortOrder);
            Assert.AreEqual("C", refetched[1].StringValue);
        }

        private class IdentityItemForGetQueries
        {
            public int SisoId { get; set; }

            public int SortOrder { get; set; }

            public string StringValue { get; set; }
        }

        private class GuidItemForGetQueries
        {
            public Guid SisoId { get; set; }

            public int SortOrder { get; set; }

            public string StringValue { get; set; }
        }

        private class View
        {
            public int SortOrder { get; set; }

            public string StringValue { get; set; }
        }
    }
}