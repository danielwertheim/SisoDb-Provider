using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SisoDb.Querying;
using SisoDb.TestUtils;

namespace SisoDb.Tests.IntegrationTests.Providers.SqlProvider.UnitOfWork.Queries
{
    [TestFixture]
    public class SqlUnitOfWorkNamedQueryTests : SqlIntegrationTestBase
    {
        private const int QueryForMinId = 3;
        private const int QueryForMaxId = 6;
        private const string ProcedureName = "NamedQueryTest";
        private IList<ItemForNamedQueries> _items;

        protected override void OnTestInitialize()
        {
            CreateStoredProcedure();

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
            DeleteProcedure();
        }

        private void CreateStoredProcedure()
        {
            var hashForSortOrder = SisoDbEnvironment.MemberNameGenerator.Generate("SortOrder");
            var sql =
                string.Format("create procedure [dbo].[{0}] @minId int, @maxId int as begin "
                + "select Json from dbo.ItemForNamedQueriesStructure as S inner join dbo.ItemForNamedQueriesIndexes as I on I.StructureId = S.Id "
                + "where I.[StructureId] between @minId and @maxId "
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
                query.Add(new QueryParameter("minId", QueryForMinId), new QueryParameter("maxId", QueryForMaxId));
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
                query.Add(new QueryParameter("minId", minIdOutsideScope), new QueryParameter("maxId", maxIdOutsideScope));
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
                query.Add(new QueryParameter("minId", minIdOutsideScope), new QueryParameter("maxId", maxIdOutsideScope));
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
                query.Add(new QueryParameter("minId", QueryForMinId), new QueryParameter("maxId", QueryForMaxId));
                refetchedJson = uow.NamedQueryAsJson<ItemForNamedQueries>(query).ToList();
            }

            Assert.AreEqual(4, refetchedJson.Count());
            Assert.AreEqual("{\"Id\":3,\"SortOrder\":3}", refetchedJson[0]);
            Assert.AreEqual("{\"Id\":6,\"SortOrder\":6}", refetchedJson[3]);
        }

        private class ItemForNamedQueries
        {
            public int Id { get; set; }

            public int SortOrder { get; set; }
        }
    }
}