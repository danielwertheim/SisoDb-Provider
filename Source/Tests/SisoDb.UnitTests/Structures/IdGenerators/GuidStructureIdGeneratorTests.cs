using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using SisoDb.Structures;
using SisoDb.Structures.IdGenerators;
using SisoDb.Structures.Schemas;

namespace SisoDb.UnitTests.Structures.IdGenerators
{
    [TestFixture]
    public class GuidStructureIdGeneratorTests : UnitTestBase
    {
        private IStructureIdGenerator _structureIdGenerator;

        protected override void OnFixtureInitialize()
        {
            _structureIdGenerator = new GuidStructureIdGenerator();
        }

        private IEnumerable<IStructureId> GenerateIds(int numOfIds)
        {
            for (var c = 0; c < numOfIds; c++)
                yield return StructureId.Create(SequentialGuid.New());
        }

        [Test]
        public void CreateIds_WhenNumOfIdsIsPositiveInteger_ReturnsCorrectNumberOfSequentialGuids()
        {
            var numOfIds = 10;

            var idsCount = _structureIdGenerator.Generate(Mock.Of<IStructureSchema>(), numOfIds)
                .Where(id => id.HasValue)
                .Select(id => id.Value)
                .Cast<Guid>()
                .Count();
            
            Assert.AreEqual(numOfIds, idsCount);
        }

        [Test]
        public void CreateIds_WhenZeroIsPassedForNumOfIds_ReturnsZeroLenghtArray()
        {
            var ids = _structureIdGenerator.Generate(Mock.Of<IStructureSchema>(), 0).ToArray();

            Assert.AreEqual(0, ids.Length);
        }

        [Test]
        public void CreateIds_ReturnsGuids()
        {
            var ids = _structureIdGenerator.Generate(Mock.Of<IStructureSchema>(), 10).Select(id => id.Value);

            CollectionAssert.AllItemsAreInstancesOfType(ids, typeof(Guid));
        }
    }
}