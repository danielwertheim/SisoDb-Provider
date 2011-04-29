using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using NUnit.Framework;
using SisoDb.Annotations;
using SisoDb.Core;

namespace SisoDb.Tests.IntegrationTests.Providers.SqlProvider.UnitOfWork.Inserts
{
    [TestFixture]
    public class SqlUnitOfWorkInsertWithUniqueTests : SqlIntegrationTestBase
    {
        protected override void OnTestFinalize()
        {
            DropStructureSet<UniqueOrder>();
        }

        [Test]
        public void Insert_WhenValidUniquePerTypeByHavingUniqueOrderNo_StructureIsInserted()
        {
            var order = new UniqueOrder { SisoId = new Guid("BDAC94C3-7FB4-4781-9612-5753DD9F9330"), OrderNo = "O123" };

            using (var uow = Database.CreateUnitOfWork())
            {
                uow.Insert(order);
                uow.Commit();

                order = uow.GetById<UniqueOrder>(order.SisoId);
            }

            Assert.IsNotNull(order);
        }

        [Test]
        public void Insert_WhenValidUniquePerInstanceByHavingUniqueProductNo_StructureIsInserted()
        {
            var order = new UniqueOrder
                        {
                            SisoId = new Guid("BDAC94C3-7FB4-4781-9612-5753DD9F9330"),
                            OrderNo = "O123",
                            Lines = new List<UniqueOrderline> {new UniqueOrderline {ProductNo = "P123"}, new UniqueOrderline{ProductNo = "P321"}}
                        };

            using (var uow = Database.CreateUnitOfWork())
            {
                uow.Insert(order);
                uow.Commit();

                order = uow.GetById<UniqueOrder>(order.SisoId);
            }

            Assert.IsNotNull(order);
        }

        [Test]
        public void Insert_WhenUniquePerTypeAttributeExists_KeyValueEndsUpInUniquesTable()
        {
            var order = new UniqueOrder { SisoId = new Guid("BDAC94C3-7FB4-4781-9612-5753DD9F9330"), OrderNo = "O123" };

            using (var uow = Database.CreateUnitOfWork())
            {
                uow.Insert(order);
                uow.Commit();
            }

            var table = DbHelper.GetTableBySql(
                "select UqName, UqValue from dbo.UniqueOrderUniques where UqSisoId is null;");
            Assert.AreEqual(1, table.Rows.Count);
            Assert.IsTrue(table.AsEnumerable().First()["UqName"].ToString().StartsWith("OrderNo_"));
            Assert.AreEqual("O123", table.AsEnumerable().First()["UqValue"]);
        }

        [Test]
        public void Insert_WhenUniquePerInstanceAttributeExists_KeyValueEndsUpInUniquesTable()
        {
            var order = new UniqueOrder
            {
                SisoId = new Guid("BDAC94C3-7FB4-4781-9612-5753DD9F9330"),
                OrderNo = "O123",
                Lines = new List<UniqueOrderline> { new UniqueOrderline { ProductNo = "P123" } }
            };

            using (var uow = Database.CreateUnitOfWork())
            {
                uow.Insert(order);
                uow.Commit();
            }

            var table = DbHelper.GetTableBySql(
                "select UqName, UqValue from dbo.UniqueOrderUniques where UqSisoId = '{0}';".Inject(order.SisoId));
            Assert.AreEqual(1, table.Rows.Count);
            Assert.IsTrue(table.AsEnumerable().First()["UqName"].ToString().StartsWith("Lines.ProductNo_"));
            Assert.AreEqual("<$P123$>", table.AsEnumerable().First()["UqValue"]);
        }

        [Test]
        public void Insert_WhenViolatingUniquePerTypeByHavingDuplicateOrderNo_ThrowsException()
        {
            var order1 = new UniqueOrder { SisoId = new Guid("BDAC94C3-7FB4-4781-9612-5753DD9F9330"), OrderNo = "O123" };
            var order2 = new UniqueOrder { SisoId = new Guid("127DDD19-E351-44B4-8DB8-3412AC58A03A"), OrderNo = "O123" };

            using (var uow = Database.CreateUnitOfWork())
            {
                uow.Insert(order1);
                
                Assert.Throws<SqlException>(() => uow.Insert(order2));

                uow.Commit();
            }
        }

        [Test]
        public void Insert_WhenViolatingUniquePerInstanceByHavingDuplicateProductNoInSameGraph_ThrowsNoExceptionSinceDuplicatesAreRemoved()
        {
            var order = new UniqueOrder
            {
                SisoId = new Guid("BDAC94C3-7FB4-4781-9612-5753DD9F9330"),
                OrderNo = "O123",
                Lines = new List<UniqueOrderline> { new UniqueOrderline { ProductNo = "P123" }, new UniqueOrderline { ProductNo = "P123" } }
            };

            using (var uow = Database.CreateUnitOfWork())
            {
                uow.Insert(order);
                uow.Commit();
            }

            var table = DbHelper.GetTableBySql(
                "select UqName, UqValue from dbo.UniqueOrderUniques where UqSisoId = '{0}';".Inject(order.SisoId));
            Assert.AreEqual(1, table.Rows.Count);
            Assert.IsTrue(table.AsEnumerable().First()["UqName"].ToString().StartsWith("Lines.ProductNo_"));
            Assert.AreEqual("<$P123$>", table.AsEnumerable().First()["UqValue"]);
        }

        [Test]
        public void Insert_WhenViolatingUniquePerInstanceByHavingDuplicateProductNoInDifferentGraphs_ThrowsException()
        {
            var order1 = new UniqueOrder
            {
                SisoId = new Guid("BDAC94C3-7FB4-4781-9612-5753DD9F9330"),
                OrderNo = "O123",
                Lines = new List<UniqueOrderline> { new UniqueOrderline { ProductNo = "P123" } }
            };

            var order2 = new UniqueOrder
            {
                SisoId = new Guid("BDAC94C3-7FB4-4781-9612-5753DD9F9330"),
                OrderNo = "O123",
                Lines = new List<UniqueOrderline> { new UniqueOrderline { ProductNo = "P123" } }
            };

            using (var uow = Database.CreateUnitOfWork())
            {
                uow.Insert(order1);

                Assert.Throws<SqlException>(() => uow.Insert(order2));

                uow.Commit();
            }
        }

        private class UniqueOrder
        {
            public Guid SisoId { get; set; }

            [Unique(UniqueModes.PerType)]
            public string OrderNo { get; set; }

            public IList<UniqueOrderline> Lines { get; set; }
        }

        private class UniqueOrderline
        {
            [Unique(UniqueModes.PerInstance)]
            public string ProductNo { get; set; }
        }
    }
}