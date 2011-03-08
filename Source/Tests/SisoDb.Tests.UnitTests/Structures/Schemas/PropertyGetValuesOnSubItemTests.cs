using NUnit.Framework;

namespace SisoDb.Tests.UnitTests.Structures.Schemas
{
    [TestFixture]
    public class PropertyGetValuesOnSubItemTests : UnitTestBase
    {
        [Test]
        public void GetValues_WhenSubItemsArrayHasElementsWithValues_ReturnsTheValues()
        {
            var subItemsProp = PropertyTestHelper.GetProperty<Item>("SubItems");
            var valueProp = PropertyTestHelper.GetProperty<SubItem>("Value", subItemsProp, 1);

            var subItems = new[] { new SubItem { Value = "A" }, new SubItem { Value = "B" } };
            var item = new Item { SubItems = subItems };
            var values = valueProp.GetValues(item);

            CollectionAssert.AreEquivalent(new[] { "A", "B" }, values);
        }

        [Test]
        public void GetValues_WhenSubItemsArrayHasElementsWithNullValues_ReturnsTheNullValues()
        {
            var subItemsProp = PropertyTestHelper.GetProperty<Item>("SubItems");
            var valueProp = PropertyTestHelper.GetProperty<SubItem>("Value", subItemsProp, 1);

            var subItems = new[] { new SubItem { Value = null }, new SubItem { Value = null } };
            var item = new Item { SubItems = subItems };
            var values = valueProp.GetValues(item);

            CollectionAssert.AreEquivalent(new string[] { null, null }, values);
        }

        [Test]
        public void GetValues_WhenSubItemsArrayIsNull_ReturnsNull()
        {
            var subItemsProp = PropertyTestHelper.GetProperty<Item>("SubItems");
            var valueProp = PropertyTestHelper.GetProperty<SubItem>("Value", subItemsProp, 1);

            var item = new Item { SubItems = null };
            var values = valueProp.GetValues(item);

            Assert.AreEqual(new object[] { null }, values);
        }

        [Test]
        public void GetValues_WhenSubItemsArrayHasBothNullAndNonNullItems_ReturnsNonNullAndNullItems()
        {
            var subItemsProp = PropertyTestHelper.GetProperty<Item>("SubItems");
            var valueProp = PropertyTestHelper.GetProperty<SubItem>("Value", subItemsProp, 1);

            var subItems = new[] { null, new SubItem { Value = "A" } };
            var item = new Item { SubItems = subItems };
            var values = valueProp.GetValues(item);

            Assert.AreEqual(new object[] { null, "A" }, values);
        }

        [Test]
        public void GetValues_WhenStringOnSingleSubItem_ReturnsValue()
        {
            var subItemProp = PropertyTestHelper.GetProperty<Item>("SingleSubItem");
            var valueProp = PropertyTestHelper.GetProperty<SubItem>("Value", subItemProp, 1);

            var subItem = new SubItem { Value = "The value" };
            var item = new Item { SingleSubItem = subItem };
            var values = valueProp.GetValues(item);

            Assert.AreEqual(new object[]{"The value"}, values);
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