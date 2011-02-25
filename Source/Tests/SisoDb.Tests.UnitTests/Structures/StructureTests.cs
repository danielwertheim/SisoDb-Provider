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
            var fakeStructureId = new Mock<IStructureId>().Object;
            var indexes = new List<IStructureIndex>
            {
                new StructureIndex(fakeStructureId, "UniqueIndex1", "Value1", true),
                new StructureIndex(fakeStructureId, "UniqueIndex1", "Value1", true)
            };
            
            Assert.Throws<SisoDbException>(() => new Structure("Name", fakeStructureId, indexes, "{Value : \"Test\"}"));
        }
    }
}