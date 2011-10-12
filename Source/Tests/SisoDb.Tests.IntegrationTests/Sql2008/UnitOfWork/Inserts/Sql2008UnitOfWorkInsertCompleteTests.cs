using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using NUnit.Framework;

namespace SisoDb.Tests.IntegrationTests.Sql2008.UnitOfWork.Inserts
{
    [TestFixture]
    public class Sql2008UnitOfWorkInsertCompleteTests : Sql2008IntegrationTestBase
    {
        private readonly string _rootIntValueName;
        private readonly string _rootStringValueName;
        private readonly string _nestedIntName;
        private readonly string _nestedStringName;

        private enum AssertScenario
        {
            SingleInsert,
            BatchInsert
        }

        public Sql2008UnitOfWorkInsertCompleteTests()
        {
            _rootIntValueName = "IntValue";
            _rootStringValueName = "StringValue";
            _nestedIntName = "Nested.NestedInt";
            _nestedStringName = "Nested.NestedString";
        }

        protected override void OnTestFinalize()
        {
            DropStructureSet<ItemWithIdentityForInsert>();
            DropStructureSet<ItemWithGuidForInsert>();
        }

        [Test]
        public void Insert_WhenSingleIdentityItemWithNestedValues_IsStoredProperly()
        {
            var item = new ItemWithIdentityForInsert
            {
                IntValue = 1,
                StringValue = "A.1",
                Nested = new NestedValueObject { NestedInt = 10, NestedString = "A.1.1" }
            };

            ItemWithIdentityForInsert refetched;

            using (var uow = Database.CreateUnitOfWork())
            {
                uow.Insert(item);
                uow.Commit();

                refetched = uow.GetAll<ItemWithIdentityForInsert>().First();
            }

            CollectionAssert.AreEqual(new[] { item }, new[] { refetched });

            AssertIndexesTable("ItemWithIdentityForInsertIndexes", AssertScenario.SingleInsert);
        }

        [Test]
        public void Insert_WhenSingleGuidItemWithNestedValues_IsStoredProperly()
        {
            var item = new ItemWithGuidForInsert
            {
                IntValue = 1,
                StringValue = "A.1",
                Nested = new NestedValueObject { NestedInt = 10, NestedString = "A.1.1" }
            };

            ItemWithGuidForInsert refetched;

            using (var uow = Database.CreateUnitOfWork())
            {
                uow.Insert(item);
                uow.Commit();

                refetched = uow.GetAll<ItemWithGuidForInsert>().First();
            }

            CollectionAssert.AreEqual(new[] { item }, new[] { refetched });

            AssertIndexesTable("ItemWithGuidForInsertIndexes", AssertScenario.SingleInsert);
        }

        [Test]
        public void InsertMany_WhenTwoIdentityItemsWithNestedValues_IsStoredProperly()
        {
            var items = new List<ItemWithIdentityForInsert>
            {
                new ItemWithIdentityForInsert 
                {
                    IntValue = 1, 
                    StringValue = "A.1", 
                    Nested = new NestedValueObject { NestedInt = 10, NestedString = "A.1.1" } 
                },
                new ItemWithIdentityForInsert
                {
                    IntValue = 2, 
                    StringValue = "B.1", 
                    Nested = new NestedValueObject { NestedInt = 20, NestedString = "B.1.1" }
                },
            };

            List<ItemWithIdentityForInsert> refetched;

            using (var uow = Database.CreateUnitOfWork())
            {
                uow.InsertMany(items);
                uow.Commit();

                refetched = uow.GetAll<ItemWithIdentityForInsert>().ToList();
            }

            CollectionAssert.AreEqual(items, refetched);

            AssertIndexesTable("ItemWithIdentityForInsertIndexes", AssertScenario.BatchInsert);
        }

        [Test]
        public void InsertMany_WhenTwoGuidItemsWithNestedValues_IsStoredProperly()
        {
            var items = new List<ItemWithGuidForInsert>
            {
                new ItemWithGuidForInsert
                {
                    IntValue = 1, 
                    StringValue = "A.1", 
                    Nested = new NestedValueObject { NestedInt = 10, NestedString = "A.1.1" }
                },
                new ItemWithGuidForInsert
                {
                    IntValue = 2,
                    StringValue = "B.1", 
                    Nested = new NestedValueObject { NestedInt = 20, NestedString = "B.1.1" }
                },
            };

            List<ItemWithGuidForInsert> refetched;

            using (var uow = Database.CreateUnitOfWork())
            {
                uow.InsertMany(items);
                uow.Commit();

                refetched = uow.GetAll<ItemWithGuidForInsert>().ToList();
            }

            CollectionAssert.AreEqual(items, refetched);
            AssertIndexesTable("ItemWithGuidForInsertIndexes", AssertScenario.BatchInsert);
        }

        private void AssertIndexesTable(string tableName, AssertScenario assertScenario)
        {
            var table = DbHelper.GetTableBySql("select * from dbo." + tableName);
            Assert.AreEqual(5, table.Columns.Count);
            
            var rows = table.AsEnumerable();
            switch (assertScenario)
            {
                case AssertScenario.SingleInsert:
                    Assert.AreEqual(1, rows.Count());
                    break;
                case AssertScenario.BatchInsert:
                    Assert.AreEqual(2, rows.Count());
                    break;
            }
            
            Assert.AreEqual(1, rows.Select(r => r[_rootIntValueName]).First());
            Assert.AreEqual("A.1", rows.Select(r => r[_rootStringValueName]).First());
            Assert.AreEqual(10, rows.Select(r => r[_nestedIntName]).First());
            Assert.AreEqual("A.1.1", rows.Select(r => r[_nestedStringName]).First());

            if (assertScenario == AssertScenario.BatchInsert)
            {
                Assert.AreEqual(2, rows.Select(r => r[_rootIntValueName]).Last());
                Assert.AreEqual("B.1", rows.Select(r => r[_rootStringValueName]).Last());
                Assert.AreEqual(20, rows.Select(r => r[_nestedIntName]).Last());
                Assert.AreEqual("B.1.1", rows.Select(r => r[_nestedStringName]).Last());
            }
        }

        public abstract class ItemForInsert<T> : IEquatable<ItemForInsert<T>>
        {
            public T StructureId { get; set; }

            public int IntValue { get; set; }

            public string StringValue { get; set; }

            public NestedValueObject Nested { get; set; }

            protected ItemForInsert()
            {
                Nested = new NestedValueObject();
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as ItemForInsert<T>);
            }

            public bool Equals(ItemForInsert<T> other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(other.StructureId, StructureId) && other.IntValue == IntValue && Equals(other.StringValue, StringValue) && Equals(other.Nested, Nested);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int result = StructureId.GetHashCode();
                    result = (result * 397) ^ IntValue;
                    result = (result * 397) ^ StringValue.GetHashCode();
                    result = (result * 397) ^ Nested.GetHashCode();
                    return result;
                }
            }
        }

        public class ItemWithIdentityForInsert : ItemForInsert<int>
        {
        }

        public class ItemWithGuidForInsert : ItemForInsert<Guid>
        {
        }

        public class NestedValueObject : IEquatable<NestedValueObject>
        {
            public int NestedInt { get; set; }

            public string NestedString { get; set; }

            public override bool Equals(object obj)
            {
                return Equals(obj as NestedValueObject);
            }

            public bool Equals(NestedValueObject other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return other.NestedInt == NestedInt && Equals(other.NestedString, NestedString);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (NestedInt * 397) ^ NestedString.GetHashCode();
                }
            }
        }
    }
}