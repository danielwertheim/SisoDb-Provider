using System.Collections.Generic;
using NUnit.Framework;
using SisoDb.Structures.Schemas;
using SisoDb.Structures.Schemas.MemberAccessors;

namespace SisoDb.Tests.UnitTests.Structures.Schemas.MemberAccessors
{
    [TestFixture]
    public class IndexAccessorGetValuesOnSubItemTests : UnitTestBase
    {
        [Test]
        public void GetValues_WhenSubItemsArrayHasElementsWithValues_ReturnsTheValues()
        {
            var subItemsProp = StructurePropertyTestHelper.GetProperty<Item>("SubItems");
            var valueProp = StructurePropertyTestHelper.GetProperty<SubItem>("Value", subItemsProp);

            var subItems = new[] { new SubItem { Value = "A" }, new SubItem { Value = "B" } };
            var item = new Item { SubItems = subItems };

            var indexAccessor = new IndexAccessor(valueProp);
            var values = indexAccessor.GetValues(item);

            CollectionAssert.AreEquivalent(new[] { "A", "B" }, values);
        }

        [Test]
        public void GetValues_WhenSubItemsArrayHasElementsWithNullValues_ReturnsTheNullValues()
        {
            var subItemsProp = StructurePropertyTestHelper.GetProperty<Item>("SubItems");
            var valueProp = StructurePropertyTestHelper.GetProperty<SubItem>("Value", subItemsProp);

            var subItems = new[] { new SubItem { Value = null }, new SubItem { Value = null } };
            var item = new Item { SubItems = subItems };

            var indexAccessor = new IndexAccessor(valueProp);
            var values = indexAccessor.GetValues(item);

            CollectionAssert.AreEquivalent(new string[] { null, null }, values);
        }

        [Test]
        public void GetValues_WhenSubItemsArrayIsNull_ReturnsNull()
        {
            var subItemsProp = StructurePropertyTestHelper.GetProperty<Item>("SubItems");
            var valueProp = StructurePropertyTestHelper.GetProperty<SubItem>("Value", subItemsProp);
            var item = new Item { SubItems = null };

            var indexAccessor = new IndexAccessor(valueProp);
            var value = indexAccessor.GetValues(item);

            Assert.AreEqual(new string[] { null }, value);
        }

        [Test]
        public void GetValues_WhenSubItemsArrayHasBothNullAndNonNullItems_ReturnsNonNullAndNullItems()
        {
            var subItemsProp = StructurePropertyTestHelper.GetProperty<Item>("SubItems");
            var valueProp = StructurePropertyTestHelper.GetProperty<SubItem>("Value", subItemsProp);

            var subItems = new[] { null, new SubItem { Value = "A" } };
            var item = new Item { SubItems = subItems };

            var indexAccessor = new IndexAccessor(valueProp);
            var value = indexAccessor.GetValues(item);

            Assert.AreEqual(new object[] { null, "A" }, value);
        }

        [Test]
        public void GetValues_WhenStringOnSingleSubItem_ReturnsValue()
        {
            var subItemProp = StructurePropertyTestHelper.GetProperty<Item>("SingleSubItem");
            var valueProp = StructurePropertyTestHelper.GetProperty<SubItem>("Value", subItemProp);

            var subItem = new SubItem { Value = "The value" };
            var item = new Item { SingleSubItem = subItem };

            var indexAccessor = new IndexAccessor(valueProp);
            var value = indexAccessor.GetValues(item);

            Assert.AreEqual(new object[] { "The value" }, value);
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
            var values = new IndexAccessor(prodNoProperty).GetValues(graph);

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

        private class Item
        {
            public SubItem SingleSubItem { get; set; }

            public SubItem[] SubItems { get; set; }
        }

        private class SubItem
        {
            public string Value { get; set; }
        }


    }
}