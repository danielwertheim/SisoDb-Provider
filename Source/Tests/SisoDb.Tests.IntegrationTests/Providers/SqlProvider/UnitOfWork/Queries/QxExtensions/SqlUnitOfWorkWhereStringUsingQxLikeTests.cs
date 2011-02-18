using System;
using System.Linq;
using NUnit.Framework;
using SisoDb.Querying;

namespace SisoDb.Tests.IntegrationTests.Providers.SqlProvider.UnitOfWork.Queries.QxExtensions
{
    [TestFixture]
    public class SqlUnitOfWorkWhereStringUsingQxLikeTests : IntegrationTestBase
    {
        protected override void OnTestFinalize()
        {
            DropStructureSet<QxItemForQueries>();
        }

        private class QxItemForQueries
        {
            public Guid Id { get; set; }

            public string StringValue { get; set; }
        }

        [Test]
        public void QueryStringUsingQxLike_NoMatch_NullIsReturned()
        {
            var item = new QxItemForQueries { StringValue = "ABC" };
            using (var uow = Database.CreateUnitOfWork())
            {
                uow.Insert(item);
                uow.Commit();
            }

            QxItemForQueries refetched;
            using (var uow = Database.CreateUnitOfWork())
            {
                refetched = uow.Where<QxItemForQueries>(i => i.StringValue.QxLike("%A")).SingleOrDefault();
            }

            Assert.IsNull(refetched);
        }

        [Test]
        public void QueryStringUsingQxLike_MatchStart_ItemIsReturned()
        {
            var item = new QxItemForQueries { StringValue = "ABC" };
            using (var uow = Database.CreateUnitOfWork())
            {
                uow.Insert(item);
                uow.Commit();
            }

            QxItemForQueries refetched;
            using (var uow = Database.CreateUnitOfWork())
            {
                refetched = uow.Where<QxItemForQueries>(i => i.StringValue.QxLike("A%")).SingleOrDefault();
            }

            Assert.AreEqual("ABC", refetched.StringValue);
        }

        [Test]
        public void QueryStringUsingQxLike_MatchEnd_ItemIsReturned()
        {
            var item = new QxItemForQueries { StringValue = "ABC" };
            using (var uow = Database.CreateUnitOfWork())
            {
                uow.Insert(item);
                uow.Commit();
            }

            QxItemForQueries refetched;
            using (var uow = Database.CreateUnitOfWork())
            {
                refetched = uow.Where<QxItemForQueries>(i => i.StringValue.QxLike("%C")).SingleOrDefault();
            }

            Assert.AreEqual("ABC", refetched.StringValue);
        }

        [Test]
        public void QueryStringUsingQxLike_MatchMiddle_ItemIsReturned()
        {
            var item = new QxItemForQueries { StringValue = "ABC" };
            using (var uow = Database.CreateUnitOfWork())
            {
                uow.Insert(item);
                uow.Commit();
            }

            QxItemForQueries refetched;
            using (var uow = Database.CreateUnitOfWork())
            {
                refetched = uow.Where<QxItemForQueries>(i => i.StringValue.QxLike("%B%")).SingleOrDefault();
            }

            Assert.AreEqual("ABC", refetched.StringValue);
        }
    }
}