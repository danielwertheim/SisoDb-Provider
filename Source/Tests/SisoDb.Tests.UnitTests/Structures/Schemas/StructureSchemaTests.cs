using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using SisoDb.Structures.Schemas;
using SisoDb.Structures.Schemas.MemberAccessors;

namespace SisoDb.Tests.UnitTests.Structures.Schemas
{
    [TestFixture]
    public class StructureSchemaTests : UnitTestBase
    {
        [Test]
        public void Ctor_WhenNameIsNull_ThrowsArgumentNullException()
        {
            var idAccessorFake = new Mock<IIdAccessor>();

            var ex = Assert.Throws<ArgumentNullException>(() => new StructureSchema(null, idAccessorFake.Object));

            Assert.IsNotNull(ex);
        }

        [Test]
        public void Ctor_WhenIdAccessorIsNull_ThrowsArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new StructureSchema("The name", null));
            
            Assert.IsNotNull(ex);
        }

        [Test]
        public void Ctor_WhenUniqueIndexAccessorsInjected_ExistsInBothListOfIndexAccessors()
        {
            var idAccessorFake = new Mock<IIdAccessor>();
            var indexAccessorFake = new Mock<IIndexAccessor>();

            indexAccessorFake.Setup(x => x.Path).Returns("Plain");
            indexAccessorFake.Setup(x => x.IsUnique).Returns(false);
            
            var uniqueIndexAccessorFake = new Mock<IIndexAccessor>();
            uniqueIndexAccessorFake.Setup(x => x.Path).Returns("Unique");
            uniqueIndexAccessorFake.Setup(x => x.IsUnique).Returns(true);

            var schema = new StructureSchema("JustADummyName",
                idAccessorFake.Object,
                new[] {indexAccessorFake.Object, uniqueIndexAccessorFake.Object});

            Assert.IsTrue(schema.IndexAccessors.Any(iac => iac.Path == indexAccessorFake.Object.Path));
            Assert.IsFalse(schema.UniqueIndexAccessors.Any(iac => iac.Path == indexAccessorFake.Object.Path));
            Assert.IsTrue(schema.IndexAccessors.Any(iac => iac.Path == uniqueIndexAccessorFake.Object.Path));
            Assert.IsTrue(schema.UniqueIndexAccessors.Any(iac => iac.Path == uniqueIndexAccessorFake.Object.Path));
        }
    }
}