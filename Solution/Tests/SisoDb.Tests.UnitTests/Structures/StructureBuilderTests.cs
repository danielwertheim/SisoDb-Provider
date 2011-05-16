using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SisoDb.Structures;

namespace SisoDb.Tests.UnitTests.Structures
{
    [TestFixture]
    public class StructureBuilderTests : UnitTestBase
    {
        private StructureBuilder _builder;

        protected override void OnTestInitialize()
        {
            _builder = new StructureBuilder(
                SisoEnvironment.Resources.ResolveJsonSerializer(),
                new SisoIdFactory(),
                new StructureIndexesFactory(SisoEnvironment.Formatting.StringConverter));
        }

        [Test]
        public void CalculateIdentitySeed_WhenFirstBatchAndBatchSize1000_Returns1()
        {
            var seed = StructureBuilder.CalculateIdentitySeed(1, 0, 1000);

            Assert.AreEqual(1, seed);
        }

        [Test]
        public void CalculateIdentitySeed_WhenSecondBatchAndBatchSize1000_Returns1001()
        {
            var seed = StructureBuilder.CalculateIdentitySeed(1, 1, 1000);

            Assert.AreEqual(1001, seed);
        }

        [Test]
        public void CreateBatchedIdentityStructures_WhenProcessing2900Items_ItemsAreGettingGeneratedInCorrectOrder()
        {
            var schema = StructureSchemaTestFactory.Stub<IdentityItem>(generateIdAccessor: true, indexAccessorsPaths: new[] {"Value"});
            var items = CreateIdentityItems(2900);

            var structuresBatches = _builder.CreateBatchedIdentityStructures(items, schema, 1000, 1);

            var previousTotalStructuresRead = 0;
            foreach (var structuresBatch in structuresBatches)
            {
                var structuresIds = structuresBatch.Select(s => (int)s.Id.Value).ToArray();
                var itemIds = items.Skip(previousTotalStructuresRead).Take(structuresIds.Length).Select(i => i.SisoId).ToArray();

                previousTotalStructuresRead += structuresIds.Length;

                CollectionAssert.AreEqual(itemIds, structuresIds);
            }
        }

        [Test]
        public void CreateBatchedGuidStructures_WhenProcessing2900Items_ItemsAreGettingGeneratedInCorrectOrder()
        {
            var schema = StructureSchemaTestFactory.Stub<GuidItem>(generateIdAccessor: true, indexAccessorsPaths: new[] { "Value" });
            var items = CreateGuidItems(2900);

            var structuresBatches = _builder.CreateBatchedGuidStructures(items, schema, 1000);

            var previousTotalStructuresRead = 0;
            foreach (var structuresBatch in structuresBatches)
            {
                var structuresIds = structuresBatch.Select(s => (Guid)s.Id.Value).ToArray();
                var itemIds = items.Skip(previousTotalStructuresRead).Take(structuresIds.Length).Select(i => i.SisoId).ToArray();

                previousTotalStructuresRead += structuresIds.Length;

                CollectionAssert.AreEqual(itemIds, structuresIds);
            }
        }

        private static IEnumerable<IdentityItem> CreateIdentityItems(int numOfItems)
        {
            var items = new List<IdentityItem>(numOfItems);

            for (var c = 0; c < numOfItems; c++)
                items.Add(new IdentityItem { Value = c + 1 });

            return items;
        }

        private static IEnumerable<GuidItem> CreateGuidItems(int numOfItems)
        {
            var items = new List<GuidItem>(numOfItems);

            for (var c = 0; c < numOfItems; c++)
                items.Add(new GuidItem { Value = c + 1 });

            return items;
        }

        private class IdentityItem
        {
            public int SisoId { get; set; }

            public int Value { get; set; }
        }

        private class GuidItem
        {
            public Guid SisoId { get; set; }

            public int Value { get; set; }
        }
    }
}