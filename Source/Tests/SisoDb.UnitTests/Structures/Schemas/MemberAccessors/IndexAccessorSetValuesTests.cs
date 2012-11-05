using NUnit.Framework;

namespace SisoDb.UnitTests.Structures.Schemas.MemberAccessors
{
    [TestFixture]
    public class IndexAccessorSetValuesTests : UnitTestBase
    {
        [Test]
        public void SetValue_WhenAssigningStringValueToNullProp_ValueIsAssigned()
        {
            const string newValue = "Test";
            var item = new Item { StringProp = null };
            var property = StructurePropertyTestFactory.GetPropertyByPath<Item>("StringProp");
            var indexAccessor = IndexAccessorTestFactory.CreateFor(property);

            indexAccessor.SetValue(item, newValue);

            Assert.AreEqual(newValue, item.StringProp);
        }

        [Test]
        public void SetValue_WhenAssigningStringValueToNonNullProp_ValueIsAssigned()
        {
            const string newValue = "Override with this";
            var item = new Item { StringProp = "Test" };
            var property = StructurePropertyTestFactory.GetPropertyByPath<Item>("StringProp");
            var indexAccessor = IndexAccessorTestFactory.CreateFor(property);

            indexAccessor.SetValue(item, newValue);

            Assert.AreEqual(newValue, item.StringProp);
        }

        [Test]
        public void SetValue_WhenAssigningPrimitiveValueToNonNullProp_ValueIsAssigned()
        {
            const int newValue = 42;
            var item = new Item { IntProp = 0 };
            var property = StructurePropertyTestFactory.GetPropertyByPath<Item>("IntProp");
            var indexAccessor = IndexAccessorTestFactory.CreateFor(property);

            indexAccessor.SetValue(item, newValue);

            Assert.AreEqual(newValue, item.IntProp);
        }

        [Test]
        public void SetValue_WhenAssigningPrimitiveValueToNullNullableProp_ValueIsAssigned()
        {
            const int newValue = 42;
            var item = new Item { NullableIntProp = null };
            var property = StructurePropertyTestFactory.GetPropertyByPath<Item>("NullableIntProp");
            var indexAccessor = IndexAccessorTestFactory.CreateFor(property);

            indexAccessor.SetValue(item, newValue);

            Assert.AreEqual(newValue, item.NullableIntProp);
        }

        [Test]
        public void SetValue_WhenAssigningPrimitiveValueToNonNullNullableProp_ValueIsAssigned()
        {
            const int newValue = 42;
            var item = new Item { NullableIntProp = 1 };
            var property = StructurePropertyTestFactory.GetPropertyByPath<Item>("NullableIntProp");
            var indexAccessor = IndexAccessorTestFactory.CreateFor(property);

            indexAccessor.SetValue(item, newValue);

            Assert.AreEqual(newValue, item.NullableIntProp);
        }

        private class Item
        {
            public string StringProp { get; set; }

            public int IntProp { get; set; }

            public int? NullableIntProp { get; set; }
        }
    }
}