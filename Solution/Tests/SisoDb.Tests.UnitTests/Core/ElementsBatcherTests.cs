using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SisoDb.Core;

namespace SisoDb.Tests.UnitTests.Core
{
    [TestFixture]
    public class ElementsBatcherTests
    {
        [Test]
        public void Batch_WhenElementCountIsLowerThanMaxBatchSize_OneBatchIsReturned()
        {
            const int maxBatchSize = 500;
            const int elementsCount = 400;
            var elements = new List<string>();
            for (var c = 0; c < elementsCount; c++)
                elements.Add(c.ToString());

            var batcher = new ElementBatcher(maxBatchSize);
            var batches = batcher.Batch(elements).ToList();

            Assert.AreEqual(1, batches.Count);
            Assert.AreEqual(elementsCount, batches[0].Count());
        }

        [Test]
        public void Batch_WhenElementCountIsGreaterThanMaxBatchSize_OneBatchIsReturned()
        {
            const int maxBatchSize = 500;
            const int elementsCount = 600;
            var elements = new List<string>();
            for (var c = 0; c < elementsCount; c++)
                elements.Add(c.ToString());

            var batcher = new ElementBatcher(maxBatchSize);
            var batches = batcher.Batch(elements).ToList();

            Assert.AreEqual(2, batches.Count);
            Assert.AreEqual(maxBatchSize, batches[0].Count());
            Assert.AreEqual(100, batches[1].Count());
        }
    }
}