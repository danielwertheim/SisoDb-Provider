using System;
using System.Linq;
using NUnit.Framework;
using SisoDb.Querying;

namespace SisoDb.Tests.IntegrationTests.Providers.Sql2008.UnitOfWork.Queries.QxExtensions
{
    [TestFixture]
    public class Sql2008UnitOfWorkQueryStringUsingQxEndsWithTests : Sql2008IntegrationTestBase
    {
        protected override void OnTestFinalize()
        {
            DropStructureSet<QxItemForQueries>();
        }

        private class QxItemForQueries
        {
            public Guid SisoId { get; set; }

            public string StringValue { get; set; }
        }

        [Test]
        public void QueryStringUsingQxEndsWith_NoMatch_NullIsReturned()
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
                refetched = uow.Query<QxItemForQueries>(
                    q => q.Where(i => i.StringValue.QxEndsWith("A"))).SingleOrDefault();
            }

            Assert.IsNull(refetched);
        }

        [Test]
        public void QueryStringUsingQxEndsWith_MatchingEnd_ItemIsReturned()
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
                refetched = uow.Query<QxItemForQueries>(
                    q => q.Where(i => i.StringValue.QxEndsWith("BC"))).SingleOrDefault();
            }

            Assert.AreEqual("ABC", refetched.StringValue);
        }

        [Test]
        public void QueryStringUsingQxEndsWith_CompleteMatch_ItemIsReturned()
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
                refetched = uow.Query<QxItemForQueries>(
                    q => q.Where(i => i.StringValue.QxEndsWith("ABC"))).SingleOrDefault();
            }

            Assert.AreEqual("ABC", refetched.StringValue);
        }
    }
}