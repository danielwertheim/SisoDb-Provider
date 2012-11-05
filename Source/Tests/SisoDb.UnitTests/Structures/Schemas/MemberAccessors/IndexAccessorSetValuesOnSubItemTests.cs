using NUnit.Framework;

namespace SisoDb.UnitTests.Structures.Schemas.MemberAccessors
{
    [TestFixture]
    public class IndexAccessorSetValuesOnSubItemTests : UnitTestBase
    {
        [Test]
        public void SetValue_WhenComplexType_CompexTypeIsAssigned()
        {
            const string newValue = "Test";
            var item = new Item { SingleSubItem = null };
            var subItemProp = StructurePropertyTestFactory.GetRawProperty<Item>("SingleSubItem");
            var indexAccessor = IndexAccessorTestFactory.CreateFor(subItemProp);
            
            indexAccessor.SetValue(item, new SubItem { Value = newValue });

            Assert.AreEqual(newValue, item.SingleSubItem.Value);
        }

        [Test]
        public void SetValue_WhenStringValueToNestedNullProp_ValueIsAssigned()
        {
            const string newValue = "Test";
            var item = new Item
            {
                SingleSubItem = new SubItem { Value = null }
            };
            var valueProp = StructurePropertyTestFactory.GetPropertyByPath<Item>("SingleSubItem.Value");
            var indexAccessor = IndexAccessorTestFactory.CreateFor(valueProp);

            indexAccessor.SetValue(item, newValue);

            Assert.AreEqual(newValue, item.SingleSubItem.Value);
        }

        [Test]
        public void SetValue_WhenStringValueToNestedNonNullProp_ValueIsAssigned()
        {
            const string newValue = "Test";
            var item = new Item
            {
                SingleSubItem = new SubItem { Value = "Not" + newValue }
            };
            var valueProp = StructurePropertyTestFactory.GetPropertyByPath<Item>("SingleSubItem.Value");
            var indexAccessor = IndexAccessorTestFactory.CreateFor(valueProp);
            
            indexAccessor.SetValue(item, newValue);

            Assert.AreEqual(newValue, item.SingleSubItem.Value);
        }

        private class Item
        {
            public SubItem SingleSubItem { get; set; }
        }

        private class SubItem
        {
            public string Value { get; set; }
        }
    }
}