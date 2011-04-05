using System.Collections.Generic;
using NUnit.Framework;
using SisoDb.Structures.Schemas;

namespace SisoDb.Tests.UnitTests.Structures.Schemas
{
    [TestFixture]
    public class PropertyGetStringValuesTests : UnitTestBase
    {
        [Test]
        public void GetValues_WhenSingleStringMember_SingleValueIsReturned()
        {
            var propertyInfo = typeof(TestCustomer).GetProperty("CustomerNo");
            var property = new StructureProperty(propertyInfo);

            var customer = new TestCustomer { CustomerNo = "1234" };
            var customerNos = property.GetValues(customer);

            CollectionAssert.AreEqual(new[] {"1234"}, customerNos);
        }

        [Test]
        public void GetValues_WhenArrayOfInt_ReturnsAValueArray()
        {
            var propertyInfo = typeof(TestCustomer).GetProperty("Points");
            var property = new StructureProperty(propertyInfo);

            var container = new TestCustomer { Points = new[] { 5, 4, 3, 2, 1 } };
            var values = property.GetValues(container);

            CollectionAssert.AreEqual(new[] { 5, 4, 3, 2, 1 }, values);
        }

        [Test]
        public void GetValues_WhenDeepGraphWithEnumerables_CanExtractValues()
        {
            var ordersPropertyInfo = typeof(TestCustomer).GetProperty("Orders");
            var ordersProperty = new StructureProperty(ordersPropertyInfo);

            var linesPropertyInfo = typeof(TestOrder).GetProperty("Lines");
            var linesProperty = new StructureProperty(ordersProperty, linesPropertyInfo);

            var prodNoPropertyInfo = typeof(TestOrderLine).GetProperty("ProductNo");
            var prodNoProperty = new StructureProperty(linesProperty, prodNoPropertyInfo);

            var graph = new TestCustomer
                            {
                                Orders = 
                                    {
                                        new TestOrder
                                            {
                                                Lines =
                                                    {
                                                        new TestOrderLine { ProductNo = "P1", Quantity = 1 }, 
                                                        new TestOrderLine { ProductNo = "P2", Quantity = 2 }
                                                    }
                                            }
                                    }
                            };
            var values = prodNoProperty.GetValues(graph);

            CollectionAssert.AreEqual(new[] { "P1", "P2" }, values);
        }

        private class TestCustomer
        {
            public string CustomerNo { get; set; }

            public List<TestOrder> Orders { get; set; }

            public int[] Points { get; set; }

            public string[] Addresses { get; set; }

            public TestCustomer()
            {
                Orders = new List<TestOrder>();
            }
        }

        private class TestOrder
        {
            public List<TestOrderLine> Lines { get; set; }

            public TestOrder()
            {
                Lines = new List<TestOrderLine>();
            }
        }

        private class TestOrderLine
        {
            public string ProductNo { get; set; }
            public int Quantity { get; set; }
        }
    }
}