using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;
using SisoDb.Dynamic;
using SisoDb.Resources;

namespace SisoDb.UnitTests.Dynamic
{
    [TestFixture]
    public class DynamicLambdaBuilderTests : UnitTestBase
    {
        [Test]
        public void BuildPredicate_WhenNoSpaceBeforeLambdaOperator_ThrowsArgumentException()
        {
            var builder = new DynamicLambdaBuilder();

            var ex = Assert.Throws<ArgumentException>(() => builder.BuildPredicate(typeof(Item), "i=> i.Value == \"Foo\""));
            Assert.AreEqual(ExceptionMessages.DynamicLambdaBuilder_InvalidExpressionFormat, ex.Message);
        }

        [Test]
        public void BuildPredicate_WhenStartingWithLambdaOperator_ThrowsArgumentException()
        {
            var builder = new DynamicLambdaBuilder();

            var ex = Assert.Throws<ArgumentException>(() => builder.BuildPredicate(typeof(Item), "=> i.Value == \"Foo\""));
            Assert.AreEqual(ExceptionMessages.DynamicLambdaBuilder_InvalidExpressionFormat, ex.Message);
        }

        [Test]
        public void BuildPredicate_WhenNoLambdaOperator_ThrowsArgumentException()
        {
            var builder = new DynamicLambdaBuilder();

            var ex = Assert.Throws<ArgumentException>(() => builder.BuildPredicate(typeof(Item), "i.Value == \"Foo\""));
            Assert.AreEqual(ExceptionMessages.DynamicLambdaBuilder_InvalidExpressionFormat, ex.Message);
        }

        [Test]
        public void BuildPredicate_WhenPassingValidExpressionForNonNestedItem_GeneratesValidLambda()
        {
            var builder = new DynamicLambdaBuilder();

            var expression = builder.BuildPredicate(typeof(Item), "i => i.Value == \"Alpha\" || i.Value == \"Bravo\"");

            var result = Store.QueryItems(expression);
            Assert.AreEqual(4, result.Length);
            Assert.AreEqual("Alpha", result[0].Value);
            Assert.AreEqual("Alpha", result[1].Value);
            Assert.AreEqual("Bravo", result[2].Value);
            Assert.AreEqual("Bravo", result[3].Value);
        }

        [Test]
        public void BuildPredicate_WhenPassingValidExpressionForNestedItem_GeneratesValidLambda()
        {
            var builder = new DynamicLambdaBuilder();

            var expression = builder.BuildPredicate(typeof(Store.NestedItem), "i => i.Value == \"Alpha\" || i.Value == \"Bravo\"");

            var result = Store.QueryNestedItems(expression);
            Assert.AreEqual(4, result.Length);
            Assert.AreEqual("Alpha", result[0].Value);
            Assert.AreEqual("Alpha", result[1].Value);
            Assert.AreEqual("Bravo", result[2].Value);
            Assert.AreEqual("Bravo", result[3].Value);
        }

        [Test]
        public void BuildMember_WhenPassingMemberExpression_GeneratesValidLambda()
        {
            var builder = new DynamicLambdaBuilder();

            var expression = builder.BuildMember(typeof(Item), "i => i.Value");
            Assert.IsNotNull(expression);
        }

        [Test]
        public void BuildMember_WhenPassingMemberExpressionOnNestedItem_GeneratesValidLambda()
        {
            var builder = new DynamicLambdaBuilder();

            var expression = builder.BuildMember(typeof(Store.NestedItem), "i => i.Value");
            Assert.IsNotNull(expression);
        }

        public class Store
        {
            private readonly List<Item> _items = new List<Item>();
            private readonly List<NestedItem> _nestedItems = new List<NestedItem>();

            public Store()
            {
                _items.AddRange(new[]
                {
                    new Item { Id = 1, Value = "Alpha" },
                    new Item { Id = 2, Value = "Alpha" },
                    new Item { Id = 3, Value = "Bravo" },
                    new Item { Id = 4, Value = "Bravo" },
                    new Item { Id = 5, Value = "Charlie" },
                    new Item { Id = 6, Value = "Charlie" }
                });

                _nestedItems.AddRange(_items.Select(i => new NestedItem { Id = i.Id, Value = i.Value }));
            }

            public static Item[] QueryItems(LambdaExpression q)
            {
                return new Store().QueryItems((Func<Item, bool>)q.Compile()).ToArray();
            }

            public static NestedItem[] QueryNestedItems(LambdaExpression q)
            {
                return new Store().QueryNestedItems((Func<NestedItem, bool>)q.Compile()).ToArray();
            }

            private IEnumerable<Item> QueryItems(Func<Item, bool> q)
            {
                return _items.Where(q);
            }

            private IEnumerable<NestedItem> QueryNestedItems(Func<NestedItem, bool> q)
            {
                return _nestedItems.Where(q);
            }

            public class NestedItem
            {
                public int Id { get; set; }
                public string Value { get; set; }
            }
        }

        public class Item
        {
            public int Id { get; set; }
            public string Value { get; set; }
        }
    }
}