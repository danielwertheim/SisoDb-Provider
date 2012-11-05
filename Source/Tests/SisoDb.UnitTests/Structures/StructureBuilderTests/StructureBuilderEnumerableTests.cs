using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SisoDb.Structures;
using SisoDb.UnitTests.Structures.Schemas;

namespace SisoDb.UnitTests.Structures.StructureBuilderTests
{
	[TestFixture]
	public class StructureBuilderEnumerableTests : StructureBuilderBaseTests
	{
		protected override void OnTestInitialize()
		{
			Builder = new StructureBuilder();
		}

		[Test]
		public void CreateStructure_WhenEnumerableIntsOnFirstLevel_ReturnsOneIndexPerElementInCorrectOrder()
		{
			var schema = StructureSchemaTestFactory.CreateRealFrom<TestItemForFirstLevel>();
			var item = new TestItemForFirstLevel { IntArray = new[] { 5, 6, 7 } };

			var structure = Builder.CreateStructure(item, schema);

			var indices = structure.Indexes.Where(i => i.Path == "IntArray").ToList();
			Assert.AreEqual(5, indices[0].Value);
			Assert.AreEqual(6, indices[1].Value);
			Assert.AreEqual(7, indices[2].Value);
		}

		[Test]
		public void CreateStructure_WhenEnumerableIntsOnFirstLevelAreNull_ReturnsNoIndex()
		{
			var schema = StructureSchemaTestFactory.CreateRealFrom<TestItemForFirstLevel>();
			var item = new TestItemForFirstLevel { IntArray = null };

			var structure = Builder.CreateStructure(item, schema);

			var actual = structure.Indexes.SingleOrDefault(si => si.Path.StartsWith("IntArray"));
			Assert.IsNull(actual);
			Assert.AreEqual(2, structure.Indexes.Count);
		}

		[Test]
		public void CreateStructure_WhenEnumerableIntsOnSecondLevelAreNull_ReturnsNoIndex()
		{
			var schema = StructureSchemaTestFactory.CreateRealFrom<TestItemForSecondLevel>();
			var item = new TestItemForSecondLevel { Container = new Container { IntArray = null } };

			var structure = Builder.CreateStructure(item, schema);

			var actual = structure.Indexes.SingleOrDefault(si => si.Path.StartsWith("Container.IntArray"));
			Assert.IsNull(actual);
			Assert.AreEqual(2, structure.Indexes.Count);
		}

		[Test]
		public void CreateStructure_WhenEnumerableIntsOnSecondLevel_ReturnsOneIndexPerElementInCorrectOrder()
		{
			var schema = StructureSchemaTestFactory.CreateRealFrom<TestItemForSecondLevel>();
			var item = new TestItemForSecondLevel { Container = new Container { IntArray = new[] { 5, 6, 7 } } };

			var structure = Builder.CreateStructure(item, schema);

			var indices = structure.Indexes.Where(i => i.Path == "Container.IntArray").ToList();
			Assert.AreEqual(5, indices[0].Value);
			Assert.AreEqual(6, indices[1].Value);
			Assert.AreEqual(7, indices[2].Value);
		}

		[Test]
		public void CreateStructure_WhenCustomNonGenericEnumerableWithComplexElement_ReturnsIndexesForElements()
		{
			var schema = StructureSchemaTestFactory.CreateRealFrom<ModelForMyCustomNonGenericEnumerable>();
			var item = new ModelForMyCustomNonGenericEnumerable();
			item.Items.Add(new MyElement { IntValue = 1, StringValue = "A" });
			item.Items.Add(new MyElement { IntValue = 2, StringValue = "B" });

			var structure = Builder.CreateStructure(item, schema);

			var indices = structure.Indexes.Skip(1).ToList();
			Assert.AreEqual("A", indices[0].Value);
			Assert.AreEqual("B", indices[1].Value);
			Assert.AreEqual(1, indices[2].Value);
			Assert.AreEqual(2, indices[3].Value);
		}

        [Test]
        public void CreateStructure_WhenHashSetOfInts_ReturnsOneIndexPerElementInCorrectOrder()
        {
            var schema = StructureSchemaTestFactory.CreateRealFrom<TestItemWithHashSet>();
            var item = new TestItemWithHashSet { HashSetOfInts = new HashSet<int> { 5, 6, 7 } };

            var structure = Builder.CreateStructure(item, schema);

            var indices = structure.Indexes.Where(i => i.Path == "HashSetOfInts").ToList();
            Assert.AreEqual(5, indices[0].Value);
            Assert.AreEqual(6, indices[1].Value);
            Assert.AreEqual(7, indices[2].Value);
        }

        [Test]
        public void CreateStructure_WhenHashSetOfIntsIsNull_ReturnsNoIndex()
        {
            var schema = StructureSchemaTestFactory.CreateRealFrom<TestItemWithHashSet>();
            var item = new TestItemWithHashSet { HashSetOfInts = null };

            var structure = Builder.CreateStructure(item, schema);

            var actual = structure.Indexes.SingleOrDefault(si => si.Path.StartsWith("HashSetOfInts"));
            Assert.IsNull(actual);
            Assert.AreEqual(1, structure.Indexes.Count);
        }

        [Test]
        public void CreateStructure_WhenISetOfInts_ReturnsOneIndexPerElementInCorrectOrder()
        {
            var schema = StructureSchemaTestFactory.CreateRealFrom<TestItemWithISet>();
            var item = new TestItemWithISet { SetOfInts = new HashSet<int> { 5, 6, 7 } };

            var structure = Builder.CreateStructure(item, schema);

            var indices = structure.Indexes.Where(i => i.Path == "SetOfInts").ToList();
            Assert.AreEqual(5, indices[0].Value);
            Assert.AreEqual(6, indices[1].Value);
            Assert.AreEqual(7, indices[2].Value);
        }

        [Test]
        public void CreateStructure_WhenSetOfIntsIsNull_ReturnsNoIndex()
        {
            var schema = StructureSchemaTestFactory.CreateRealFrom<TestItemWithISet>();
            var item = new TestItemWithISet { SetOfInts = null };

            var structure = Builder.CreateStructure(item, schema);

            var actual = structure.Indexes.SingleOrDefault(si => si.Path.StartsWith("SetOfInts"));
            Assert.IsNull(actual);
            Assert.AreEqual(1, structure.Indexes.Count);
        }

        [Test]
        public void CreateStructure_WhenHashSetOfComplex_ReturnsOneIndexPerElementInCorrectOrder()
        {
            var schema = StructureSchemaTestFactory.CreateRealFrom<TestItemWithHashSetOfComplex>();
            var item = new TestItemWithHashSetOfComplex { HashSetOfComplex = new HashSet<Value> { new Value { Is = 5 }, new Value { Is = 6 }, new Value { Is = 7 } } };

            var structure = Builder.CreateStructure(item, schema);

            var indices = structure.Indexes.Where(i => i.Path == "HashSetOfComplex.Is").ToList();
            Assert.AreEqual(5, indices[0].Value);
            Assert.AreEqual(6, indices[1].Value);
            Assert.AreEqual(7, indices[2].Value);
        }

        [Test]
        public void CreateStructure_WhenHashSetOfComplex_HasThreeNullItems_ReturnsNoIndex()
        {
            var schema = StructureSchemaTestFactory.CreateRealFrom<TestItemWithHashSetOfComplex>();
            var item = new TestItemWithHashSetOfComplex { HashSetOfComplex = new HashSet<Value> { null, null, null } };

            var structure = Builder.CreateStructure(item, schema);

            var actual = structure.Indexes.SingleOrDefault(si => si.Path.StartsWith("HashSetOfComplex.Is"));
            Assert.IsNull(actual);
        }

        [Test]
        public void CreateStructure_WhenIDictionary_ReturnsOneIndexPerElementInCorrectOrder()
        {
            var schema = StructureSchemaTestFactory.CreateRealFrom<TestItemWithIDictionary>();
            var item = new TestItemWithIDictionary { KeyValues = new Dictionary<string, int> { { "Key1", 5 }, { "Key2", 6 }, { "Key3", 7 } } };

            var structure = Builder.CreateStructure(item, schema);

            var indices = structure.Indexes.Where(i => i.Path.StartsWith("KeyValues")).ToList();
            Assert.AreEqual(6, indices.Count);

            Assert.AreEqual("KeyValues.Key", indices[0].Path);
            Assert.AreEqual("Key1", indices[0].Value);
            Assert.AreEqual("KeyValues.Key", indices[1].Path);
            Assert.AreEqual("Key2", indices[1].Value);
            Assert.AreEqual("KeyValues.Key", indices[2].Path);
            Assert.AreEqual("Key3", indices[2].Value);

            Assert.AreEqual("KeyValues.Value", indices[3].Path);
            Assert.AreEqual(5, indices[3].Value);
            Assert.AreEqual("KeyValues.Value", indices[4].Path);
            Assert.AreEqual(6, indices[4].Value);
            Assert.AreEqual("KeyValues.Value", indices[5].Path);
            Assert.AreEqual(7, indices[5].Value);
        }

        [Test]
        public void CreateStructure_WhenDictionary_ReturnsOneIndexPerElementInCorrectOrder()
        {
            var schema = StructureSchemaTestFactory.CreateRealFrom<TestItemWithDictionary>();
            var item = new TestItemWithDictionary { KeyValues = new Dictionary<string, int> { { "Key1", 5 }, { "Key2", 6 }, { "Key3", 7 } } };

            var structure = Builder.CreateStructure(item, schema);

            var indices = structure.Indexes.Where(i => i.Path.StartsWith("KeyValues")).ToList();
            Assert.AreEqual(6, indices.Count);

            Assert.AreEqual("KeyValues.Key", indices[0].Path);
            Assert.AreEqual("Key1", indices[0].Value);
            Assert.AreEqual("KeyValues.Key", indices[1].Path);
            Assert.AreEqual("Key2", indices[1].Value);
            Assert.AreEqual("KeyValues.Key", indices[2].Path);
            Assert.AreEqual("Key3", indices[2].Value);

            Assert.AreEqual("KeyValues.Value", indices[3].Path);
            Assert.AreEqual(5, indices[3].Value);
            Assert.AreEqual("KeyValues.Value", indices[4].Path);
            Assert.AreEqual(6, indices[4].Value);
            Assert.AreEqual("KeyValues.Value", indices[5].Path);
            Assert.AreEqual(7, indices[5].Value);
        }

        [Test]
        public void CreateStructure_WhenIDictionaryWithComplex_ReturnsOneIndexPerElementInCorrectOrder()
        {
            var schema = StructureSchemaTestFactory.CreateRealFrom<TestItemWithIDictionaryOfComplex>();
            var item = new TestItemWithIDictionaryOfComplex { KeyValues = new Dictionary<string, Value> { { "Key1", new Value { Is = 5 } }, { "Key2", new Value { Is = 6 } }, { "Key3", new Value { Is = 7 } } } };

            var structure = Builder.CreateStructure(item, schema);

            var indices = structure.Indexes.Where(i => i.Path.StartsWith("KeyValues")).ToList();
            Assert.AreEqual(6, indices.Count);

            Assert.AreEqual("KeyValues.Key", indices[0].Path);
            Assert.AreEqual("Key1", indices[0].Value);
            Assert.AreEqual("KeyValues.Key", indices[1].Path);
            Assert.AreEqual("Key2", indices[1].Value);
            Assert.AreEqual("KeyValues.Key", indices[2].Path);
            Assert.AreEqual("Key3", indices[2].Value);

            Assert.AreEqual("KeyValues.Value.Is", indices[3].Path);
            Assert.AreEqual(5, indices[3].Value);
            Assert.AreEqual("KeyValues.Value.Is", indices[4].Path);
            Assert.AreEqual(6, indices[4].Value);
            Assert.AreEqual("KeyValues.Value.Is", indices[5].Path);
            Assert.AreEqual(7, indices[5].Value);
        }

        [Test]
        public void CreateStructure_WhenDictionaryWithComplex_ReturnsOneIndexPerElementInCorrectOrder()
        {
            var schema = StructureSchemaTestFactory.CreateRealFrom<TestItemWithDictionaryOfComplex>();
            var item = new TestItemWithDictionaryOfComplex { KeyValues = new Dictionary<string, Value> { { "Key1", new Value { Is = 5 } }, { "Key2", new Value { Is = 6 } }, { "Key3", new Value { Is = 7 } } } };

            var structure = Builder.CreateStructure(item, schema);

            var indexes = structure.Indexes.Where(i => i.Path.StartsWith("KeyValues")).ToList();
            Assert.AreEqual(6, indexes.Count);

            Assert.AreEqual("KeyValues.Key", indexes[0].Path);
            Assert.AreEqual("Key1", indexes[0].Value);
            Assert.AreEqual("KeyValues.Key", indexes[1].Path);
            Assert.AreEqual("Key2", indexes[1].Value);
            Assert.AreEqual("KeyValues.Key", indexes[2].Path);
            Assert.AreEqual("Key3", indexes[2].Value);

            Assert.AreEqual("KeyValues.Value.Is", indexes[3].Path);
            Assert.AreEqual(5, indexes[3].Value);
            Assert.AreEqual("KeyValues.Value.Is", indexes[4].Path);
            Assert.AreEqual(6, indexes[4].Value);
            Assert.AreEqual("KeyValues.Value.Is", indexes[5].Path);
            Assert.AreEqual(7, indexes[5].Value);
        }

		private class ModelForMyCustomNonGenericEnumerable
		{
			public Guid StructureId { get; set; }
			public MyCustomNonGenericEnumerable Items { get; set; }

			public ModelForMyCustomNonGenericEnumerable()
			{
				Items = new MyCustomNonGenericEnumerable();
			}
		}

		private class MyCustomNonGenericEnumerable : System.Collections.Generic.List<MyElement>
		{
		}

		private class MyElement
		{
			public string StringValue { get; set; }
			public int IntValue { get; set; }
		}

		private class TestItemForFirstLevel
		{
			public Guid StructureId { get; set; }

			public int IntValue { get; set; }

			public int[] IntArray { get; set; }
		}

		private class TestItemForSecondLevel
		{
			public Guid StructureId { get; set; }

			public Container Container { get; set; }
		}

		private class Container
		{
			public int IntValue { get; set; }

			public int[] IntArray { get; set; }
		}

        private class Value
        {
            public int Is { get; set; }
        }

        private class TestItemWithHashSetOfComplex
        {
            public Guid StructureId { get; set; }

            public HashSet<Value> HashSetOfComplex { get; set; }
        }

        private class TestItemWithHashSet
        {
            public Guid StructureId { get; set; }

            public HashSet<int> HashSetOfInts { get; set; }
        }

        private class TestItemWithISet
        {
            public Guid StructureId { get; set; }

            public ISet<int> SetOfInts { get; set; }
        }

        private class TestItemWithIDictionary
        {
            public Guid StructureId { get; set; }

            public IDictionary<string, int> KeyValues { get; set; }
        }

        private class TestItemWithIDictionaryOfComplex
        {
            public Guid StructureId { get; set; }

            public IDictionary<string, Value> KeyValues { get; set; }
        }

        private class TestItemWithDictionary
        {
            public Guid StructureId { get; set; }

            public Dictionary<string, int> KeyValues { get; set; }
        }

        private class TestItemWithDictionaryOfComplex
        {
            public Guid StructureId { get; set; }

            public Dictionary<string, Value> KeyValues { get; set; }
        }

        private class TestItemWithIntAsId
        {
            public int StructureId { get; set; }

            public int IntValue { get; set; }
        }

        private class TestItemWithLongAsId
        {
            public long StructureId { get; set; }

            public int IntValue { get; set; }
        }
	}
}