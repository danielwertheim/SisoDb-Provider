using Moq;
using NUnit.Framework;
using PineCone.Structures.Schemas;
using PineCone.Structures.Schemas.MemberAccessors;

namespace SisoDb.UnitTests.DbSchema
{
    [TestFixture]
    public abstract class StorageSchemaTestBase : UnitTestBase
    {
        protected static IStructureSchema CreateStructureSchemaFakeWithPlainAndUniques()
        {
            var structureTypeFake = new Mock<IStructureType>();
            var idAccessorFake = new Mock<IIdAccessor>();

            var indexAccessorFake1 = new Mock<IIndexAccessor>();
            indexAccessorFake1.Setup(x => x.Path).Returns("Plain1");
            indexAccessorFake1.Setup(x => x.IsUnique).Returns(false);

            var indexAccessorFake2 = new Mock<IIndexAccessor>();
            indexAccessorFake2.Setup(x => x.Path).Returns("Plain2");
            indexAccessorFake2.Setup(x => x.IsUnique).Returns(false);

            var uniqueIndexAccessorFake1 = new Mock<IIndexAccessor>();
            uniqueIndexAccessorFake1.Setup(x => x.Path).Returns("Unique1");
            uniqueIndexAccessorFake1.Setup(x => x.IsUnique).Returns(true);

            var uniqueIndexAccessorFake2 = new Mock<IIndexAccessor>();
            uniqueIndexAccessorFake2.Setup(x => x.Path).Returns("Unique2");
            uniqueIndexAccessorFake2.Setup(x => x.IsUnique).Returns(true);

            return new StructureSchema(structureTypeFake.Object, "FakeHash", idAccessorFake.Object, null,
                new[]
                {
                    indexAccessorFake1.Object,
                    indexAccessorFake2.Object,
                    uniqueIndexAccessorFake1.Object,
                    uniqueIndexAccessorFake2.Object
                });
        }
    }
}