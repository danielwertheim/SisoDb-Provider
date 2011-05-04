using System;
using System.Linq;
using NUnit.Framework;
using SisoDb.TestUtils;

namespace SisoDb.Tests.IntegrationTests.Providers.Sql2008.UnitOfWork.Inserts
{
    [TestFixture]
    public class SqlUnitOfWorkInsertCompleteTests : SqlIntegrationTestBase
    {
        protected override void OnTestFinalize()
        {
            DropStructureSet<RootItemForMappingTests>();
        }

        [Test]
        public void Insert_WhenCompleteItem_ValuesAreCorrect()
        {
            RootItemForMappingTests item = CreateGraph();
            RootItemForMappingTests refetched;

            using (var uow = Database.CreateUnitOfWork())
            {
                uow.Insert(item);
                uow.Commit();

                refetched = uow.GetAll<RootItemForMappingTests>().SingleOrDefault();
            }

            Assert.IsNotNull(refetched);
            CustomAssert.AreValueEqual(item, refetched);
        }

        private RootItemForMappingTests CreateGraph()
        {
            var item = new RootItemForMappingTests
                       {
                           String1 = "Root-TheString1",
                           Int1 = 142,
                           Bool1 = true,
                           DateTime1 = new DateTime(2000, 1, 1),
                           Bytes = BitConverter.GetBytes(12345),
                           Integers = new[] { 11, 12, 13, 14, 15 },
                           Complex = new ItemForMappingTests
                                     {
                                         String1 = "Complex-TheString1",
                                         Int1 = 242,
                                         Bool1 = false,
                                         DateTime1 = new DateTime(2000, 1, 1)
                                     },
                           ComplexArray = new[]
                                          {
                                              new ItemForMappingTests
                                              {
                                                  String1 = "Complex-TheString1",
                                                  Int1 = 242,
                                                  Bool1 = false,
                                                  DateTime1 = new DateTime(2000, 1, 1)
                                              }
                                          }
                       };

            return item;
        }

        private class RootItemForMappingTests
        {
            public int SisoId { get; set; }

            public ItemForMappingTests Complex { get; set; }

            public string String1 { get; set; }

            public int Int1 { get; set; }

            public bool Bool1 { get; set; }

            public DateTime DateTime1 { get; set; }

            public byte[] Bytes { get; set; }

            public int[] Integers { get; set; }

            public ItemForMappingTests[] ComplexArray { get; set; }
        }

        private class ItemForMappingTests
        {
            public string String1 { get; set; }

            public int Int1 { get; set; }

            public bool Bool1 { get; set; }

            public DateTime DateTime1 { get; set; }
        }
    }
}