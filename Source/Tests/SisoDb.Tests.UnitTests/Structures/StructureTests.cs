using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using SisoDb.Structures;

namespace SisoDb.Tests.UnitTests.Structures
{
    [TestFixture]
    public class StructureTests : UnitTestBase
    {
        [Test]
        public void Ctor_WhenIndexesContainsNonUniqueUniqueIndex_ThrowsSisoDbException()
        {
            var fakeSisoId = new Mock<ISisoId>().Object;
            var indexes = new List<IStructureIndex>
            {
                new StructureIndex(fakeSisoId, "UniqueIndex1", "Value1", StructureIndexUniques.PerType),
                new StructureIndex(fakeSisoId, "UniqueIndex1", "Value1", StructureIndexUniques.PerType)
            };
            
            Assert.Throws<SisoDbException>(() => new Structure("Name", fakeSisoId, indexes, "{Value : \"Test\"}"));
        }
    }
}