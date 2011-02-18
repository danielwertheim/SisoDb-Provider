using System;
using System.Linq;
using NUnit.Framework;
using SisoDb.Querying;

namespace SisoDb.Tests.IntegrationTests.Providers.SqlProvider.UnitOfWork.Queries.QxExtensions
{
    [TestFixture]
    public class SqlUnitOfWorkWhereStringUsingQxContainsTests : IntegrationTestBase
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
        public void QueryStringUsingQxContains_MatchInMiddleOfStringExists_ItemIsReturned()
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
                refetched = uow.Where<QxItemForQueries>(i => i.StringValue.QxContains("B")).SingleOrDefault();
            }

            Assert.AreEqual("ABC", refetched.StringValue);
        }

        [Test]
        public void QueryStringUsingQxContains_MatchInStartOfStringExists_ItemIsReturned()
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
                refetched = uow.Where<QxItemForQueries>(i => i.StringValue.QxContains("A")).SingleOrDefault();
            }

            Assert.AreEqual("ABC", refetched.StringValue);
        }

        [Test]
        public void QueryStringUsingQxContains_MatchInEndOfStringExists_ItemIsReturned()
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
                refetched = uow.Where<QxItemForQueries>(i => i.StringValue.QxContains("C")).SingleOrDefault();
            }

            Assert.AreEqual("ABC", refetched.StringValue);
        }

        [Test]
        public void QueryStringUsingQxContains_NoMatch_NullIsReturned()
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
                refetched = uow.Where<QxItemForQueries>(i => i.StringValue.QxContains("D")).SingleOrDefault();
            }

            Assert.IsNull(refetched);
        }
    }
}