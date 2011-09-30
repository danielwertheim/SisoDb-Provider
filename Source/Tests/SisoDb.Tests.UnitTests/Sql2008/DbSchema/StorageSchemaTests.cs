using Moq;
using NUnit.Framework;
using SisoDb.Structures.Schemas;
using SisoDb.Structures.Schemas.MemberAccessors;

namespace SisoDb.Tests.UnitTests.Sql2008.DbSchema
{
    [TestFixture]
    public abstract class StorageSchemaTests : UnitTestBase
    {
        protected static IStructureSchema CreateStructureSchemaFakeWithPlainAndUniques()
        {
            var idAccessorFake = new Mock<IIdAccessor>();

            var indexAccessorFake1 = new Mock<IIndexAccessor>();
            indexAccessorFake1.Setup(x => x.Name).Returns("Plain1");
            indexAccessorFake1.Setup(x => x.IsUnique).Returns(false);

            var indexAccessorFake2 = new Mock<IIndexAccessor>();
            indexAccessorFake2.Setup(x => x.Name).Returns("Plain2");
            indexAccessorFake2.Setup(x => x.IsUnique).Returns(false);

            var uniqueIndexAccessorFake1 = new Mock<IIndexAccessor>();
            uniqueIndexAccessorFake1.Setup(x => x.Name).Returns("Unique1");
            uniqueIndexAccessorFake1.Setup(x => x.IsUnique).Returns(true);

            var uniqueIndexAccessorFake2 = new Mock<IIndexAccessor>();
            uniqueIndexAccessorFake2.Setup(x => x.Name).Returns("Unique2");
            uniqueIndexAccessorFake2.Setup(x => x.IsUnique).Returns(true);

            return new StructureSchema(
                "FakeName",
                "FakeHash",
                idAccessorFake.Object,
                new[]
                    {
                        indexAccessorFake1.Object, indexAccessorFake2.Object,
                        uniqueIndexAccessorFake1.Object, uniqueIndexAccessorFake2.Object
                    });
        }
    }
}