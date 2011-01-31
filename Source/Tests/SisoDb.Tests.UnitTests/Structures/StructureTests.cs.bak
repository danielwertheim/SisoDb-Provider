using System.Collections.Generic;
using TypeMock.ArrangeActAssert;
using NUnit.Framework;
using SisoDb.Structures;

namespace SisoDb.Tests.UnitTests.Structures
{
    [TestFixture]
    public class StructureTests
    {
        [Test, Isolated]
        public void Ctor_WhenIndexesContainsNonUniqueUniqueIndex_ThrowsSisoDbException()
        {
            var fakeStructureId = Isolate.Fake.Instance<IStructureId>();
            var indexes = new List<IStructureIndex>
            {
                new StructureIndex(fakeStructureId, "UniqueIndex1", "Value1", true),
                new StructureIndex(fakeStructureId, "UniqueIndex1", "Value1", true)
            };
            
            Assert.Throws<SisoDbException>(() => new Structure("TypeName", fakeStructureId, indexes, "{Value : \"Test\"}"));
        }
    }
}