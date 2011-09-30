using System;
using System.Linq;
using NUnit.Framework;
using SisoDb.Structures;

namespace SisoDb.Tests.UnitTests.Structures
{
    [TestFixture]
    public class SisoIdValueGeneratorTests : UnitTestBase
    {
        [Test]
        public void CreateGuidIds_WhenNumOfIdsIsPositiveInteger_ReturnsCorrectNumberOfSequentialGuids()
        {
            var numOfIds = 10;

            var ids = SisoIdValueGenerator.CreateGuidIds(numOfIds);

            var orderedIds = ids.OrderBy(id => (Guid) id);

            Assert.AreEqual(numOfIds, ids.Length);
            CollectionAssert.AreEqual(orderedIds, ids);
        }

        [Test]
        public void CreateGuidIds_WhenZeroIsPassedForNumOfIds_ReturnsZeroLenghtArray()
        {
            var ids = SisoIdValueGenerator.CreateGuidIds(0);

            Assert.AreEqual(0, ids.Length);
        }

        [Test]
        public void CreateGuidIds_ReturnsGuids()
        {
            var ids = SisoIdValueGenerator.CreateGuidIds(10);

            CollectionAssert.AllItemsAreInstancesOfType(ids, typeof(Guid));
        }

        [Test]
        public void CreateIdentityIds_ReturnsInts()
        {
            var ids = SisoIdValueGenerator.CreateIdentityIds(1, 10);

            CollectionAssert.AllItemsAreInstancesOfType(ids, typeof(int));
        }

        [Test]
        public void CreateIdentityIds_WhenZeroIsPassedForNumOfIds_ReturnsZeroLenghtArray()
        {
            var ids = SisoIdValueGenerator.CreateIdentityIds(1, 0);

            Assert.AreEqual(0, ids.Length);
        }

        [Test]
        public void CreateIdentityIds_WhenStartingOn1AndTaking10Ids_Returns1To10()
        {
            var expectedIds = Enumerable.Range(1, 10).ToArray();

            var actualIds = SisoIdValueGenerator.CreateIdentityIds(1, 10);

            CollectionAssert.AreEqual(expectedIds, actualIds);
        }

        [Test]
        public void CreateIdentityIds_WhenStartingOn5AndTaking10Ids_Returns5To15()
        {
            var expectedIds = Enumerable.Range(5, 10).ToArray();

            var actualIds = SisoIdValueGenerator.CreateIdentityIds(5, 10);

            CollectionAssert.AreEqual(expectedIds, actualIds);
        }
    }
}