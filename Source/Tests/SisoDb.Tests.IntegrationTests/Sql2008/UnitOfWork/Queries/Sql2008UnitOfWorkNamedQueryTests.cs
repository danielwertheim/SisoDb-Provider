using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SisoDb.Core;
using SisoDb.Dac;
using SisoDb.Querying;
using SisoDb.TestUtils;

namespace SisoDb.Tests.IntegrationTests.Sql2008.UnitOfWork.Queries
{
    [TestFixture]
    public class Sql2008UnitOfWorkNamedQueryTests : Sql2008IntegrationTestBase
    {
        private const int QueryForMinId = 3;
        private const int QueryForMaxId = 6;
        private const string ProcedureName = "NamedQueryTest";
        private IList<ItemForNamedQueries> _items;

        protected override void OnFixtureInitialize()
        {
            CreateStoredProcedure();
        }

        protected override void OnFixtureFinalize()
        {
            DeleteProcedure();
        }

        protected override void OnTestInitialize()
        {
            _items = new List<ItemForNamedQueries>();

            for (var c = 0; c < 10; c++)
                _items.Add(new ItemForNamedQueries {SortOrder = c + 1});

            using (var uow = Database.CreateUnitOfWork())
            {
                uow.InsertMany(_items);
                uow.Commit();
            }
        }

        protected override void OnTestFinalize()
        {
            DropStructureSet<ItemForNamedQueries>();
        }

        private void CreateStoredProcedure()
        {
            var hashForSortOrder = SisoEnvironment.Resources.ResolveMemberNameGenerator().Generate("SortOrder");
            var sql =
                string.Format("create procedure [dbo].[{0}] @minId int, @maxId int as begin "
                + "select Json from dbo.ItemForNamedQueriesStructure as S inner join dbo.ItemForNamedQueriesIndexes as I on I.SisoId = S.SisoId "
                + "where I.[SisoId] between @minId and @maxId "
                + "order by I.[{1}];"
                + "end", ProcedureName, hashForSortOrder);

            DbHelper.CreateProcedure(sql);
        }

        private void DeleteProcedure()
        {
            DbHelper.DropProcedure("[dbo].[{0}]".Inject(ProcedureName));
        }

        [Test]
        public void NamedQuery_WhenStoredProcedureWithArgsHasResult_ReturnsItems()
        {
            IList<ItemForNamedQueries> refetched;
            using (var uow = Database.CreateUnitOfWork())
            {
                var query = new NamedQuery(ProcedureName);
                query.Add(new DacParameter("minId", QueryForMinId), new DacParameter("maxId", QueryForMaxId));
                refetched = uow.NamedQuery<ItemForNamedQueries>(query).ToList();
            }

            Assert.AreEqual(4, refetched.Count());
            CustomAssert.AreValueEqual(_items[2], refetched[0]);
            CustomAssert.AreValueEqual(_items[5], refetched[3]);
        }

        [Test]
        public void NamedQuery_WhenStoredProcedureHasNoResult_NonNullEmptyResult()
        {
            var minIdOutsideScope = _items.Count + 1;
            var maxIdOutsideScope = minIdOutsideScope;

            IList<ItemForNamedQueries> refetched;
            using (var uow = Database.CreateUnitOfWork())
            {
                var query = new NamedQuery(ProcedureName);
                query.Add(new DacParameter("minId", minIdOutsideScope), new DacParameter("maxId", maxIdOutsideScope));
                refetched = uow.NamedQuery<ItemForNamedQueries>(query).ToList();
            }

            Assert.IsNotNull(refetched);
            Assert.AreEqual(0, refetched.Count());
        }

        [Test]
        public void NamedQueryAsJson_WhenStoredProcedureHasNoResult_NonNullEmptyResult()
        {
            var minIdOutsideScope = _items.Count + 1;
            var maxIdOutsideScope = minIdOutsideScope;

            IList<string> refetched;
            using (var uow = Database.CreateUnitOfWork())
            {
                var query = new NamedQuery(ProcedureName);
                query.Add(new DacParameter("minId", minIdOutsideScope), new DacParameter("maxId", maxIdOutsideScope));
                refetched = uow.NamedQueryAsJson<ItemForNamedQueries>(query).ToList();
            }

            Assert.IsNotNull(refetched);
            Assert.AreEqual(0, refetched.Count());
        }

        [Test]
        public void NamedQueryAsJson_WhenStoredProcedureWithArgsHasResult_ReturnsItems()
        {
            IList<string > refetchedJson;
            using (var uow = Database.CreateUnitOfWork())
            {
                var query = new NamedQuery(ProcedureName);
                query.Add(new DacParameter("minId", QueryForMinId), new DacParameter("maxId", QueryForMaxId));
                refetchedJson = uow.NamedQueryAsJson<ItemForNamedQueries>(query).ToList();
            }

            Assert.AreEqual(4, refetchedJson.Count());
            Assert.AreEqual("{\"SisoId\":3,\"SortOrder\":3}", refetchedJson[0]);
            Assert.AreEqual("{\"SisoId\":6,\"SortOrder\":6}", refetchedJson[3]);
        }

        private class ItemForNamedQueries
        {
            public int SisoId { get; set; }

            public int SortOrder { get; set; }
        }
    }
}