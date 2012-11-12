using System.Collections.Generic;
using NUnit.Framework;

namespace SisoDb.UnitTests.Structures.Schemas.StructurePropertyTests
{
    [TestFixture]
    public class StructurePropertyGetStringValueTests : UnitTestBase
    {
        [Test]
        public void GetValue_WhenSingleStringMember_SingleValueIsReturned()
        {
            var property = StructurePropertyTestFactory.GetPropertyByPath<TestCustomer>("CustomerNo");

            var customer = new TestCustomer { CustomerNo = "1234" };
            var customerNos = (string)property.GetValue(customer);

            CollectionAssert.AreEqual("1234", customerNos);
        }

        [Test]
        public void GetValue_WhenArrayOfInt_ReturnsAValueArray()
        {
            var property = StructurePropertyTestFactory.GetPropertyByPath<TestCustomer>("Points");

            var container = new TestCustomer { Points = new[] { 5, 4, 3, 2, 1 } };
            var values = (IEnumerable<int>)property.GetValue(container);

            CollectionAssert.AreEqual(new[] { 5, 4, 3, 2, 1 }, values);
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