using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SisoDb.Querying;

namespace SisoDb.Tests.IntegrationTests.Sql2008.UnitOfWork.Queries.QxExtensions
{
    [TestFixture]
    public class Sql2008UnitOfWorkQueryNestedEnumerablesWithQxAny : Sql2008IntegrationTestBase
    {
        protected override void OnTestFinalize()
        {
            DropStructureSet<Root>();
        }

        [Test]
        public void Query_WithQxAny_WhenQxAnyIsUsedWithEqOperator_ReturnsCorrectMatch()
        {
            var roots = CreateRoots();

            using (var uow = Database.CreateUnitOfWork())
            {
                uow.InsertMany(roots);
                uow.Commit();

                var refetched = uow.Where<Root>(r => r.Child.Items.QxAny(i => i.Value == 20)).ToList();

                Assert.AreEqual(1, refetched.Count);
                Assert.AreEqual("ChildTwo", refetched[0].Child.Name);
                Assert.AreEqual(20, refetched[0].Child.Items[0].Value);
                Assert.AreEqual(21, refetched[0].Child.Items[1].Value);
            }
        }

        [Test]
        public void Query_WithQxAny_WhenQxAnyOnQxAnyIsUsedWithAndNotEqOperator_ReturnsCorrectMatch()
        {
            var roots = CreateRoots();

            using (var uow = Database.CreateUnitOfWork())
            {
                uow.InsertMany(roots);
                uow.Commit();

                var refetched = uow.Where<Root>(m => m.Child.Items.QxAny(i => i.Values.QxAny(i2 => i2 != 42 && i2 != 142))).ToList();

                Assert.AreEqual(1, refetched.Count);
                Assert.AreEqual("ChildTwo", refetched[0].Child.Name);
                Assert.AreEqual(20, refetched[0].Child.Items[0].Value);
                Assert.AreEqual(21, refetched[0].Child.Items[1].Value);
            }
        }

        private static IList<Root> CreateRoots()
        {
            var roots = new[] { new Root(), new Root() };

            roots[0].Child.Name = "ChildOne";
            roots[0].Child.Items.Add(new Item { Value = 10 });
            roots[0].Child.Items.Add(new Item { Value = 11, Values = new[] { 42, 142 } });

            roots[1].Child.Name = "ChildTwo";
            roots[1].Child.Items.Add(new Item { Value = 20 });
            roots[1].Child.Items.Add(new Item { Value = 21, Values = new[] { 43, 143 } });

            return roots;
        }

        public class Root
        {
            public Guid StructureId { get; set; }

            public FirstLevelChild Child { get; set; }

            public Root()
            {
                Child = new FirstLevelChild();
            }
        }

        public class FirstLevelChild
        {
            public string Name { get; set; }

            public List<Item> Items { get; set; }

            public FirstLevelChild()
            {
                Items = new List<Item>();
            }
        }

        public class Item
        {
            public int Value { get; set; }

            public int[] Values { get; set; }
        }
    }
}