using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using SisoDb.Structures.Schemas;
using SisoDb.Structures.Schemas.MemberAccessors;

namespace SisoDb.UnitTests.Structures.Schemas
{
    [TestFixture]
    public class StructureSchemaTests : UnitTestBase
    {
        [Test]
        public void Ctor_WhenTypeIsNull_ThrowsArgumentNullException()
        {
            var idAccessorFake = new Mock<IIdAccessor>();

            var ex = Assert.Throws<ArgumentNullException>(() => new StructureSchema(null, idAccessorFake.Object));

            Assert.AreEqual("type", ex.ParamName);
        }

        [Test]
        public void Ctor_WhenIdAccessorIsNull_DoesNotThrowArgumentNullException()
        {
			var typeFake = new Mock<IStructureType>();

            Assert.DoesNotThrow(() => new StructureSchema(typeFake.Object, idAccessor: null, concurrencyTokenAccessor: null, indexAccessors: null));
        }

        [Test]
        public void Ctor_WhenConcurrencyTokenAccessorIsNull_DoesNotThrowArgumentNullException()
        {
            var typeFake = new Mock<IStructureType>();

            Assert.DoesNotThrow(() => new StructureSchema(typeFake.Object, idAccessor: null, concurrencyTokenAccessor: null, indexAccessors: null));
        }

        [Test]
        public void Ctor_WhenTimeStampAccessorIsNull_DoesNotThrowArgumentNullException()
        {
            var typeFake = new Mock<IStructureType>();

            Assert.DoesNotThrow(() => new StructureSchema(typeFake.Object, idAccessor: null, concurrencyTokenAccessor: null, indexAccessors: null));
        }

        [Test]
        public void Ctor_WhenUniqueIndexAccessorsIsInjected_ItExistsInListOfIndexAccessors()
        {
			var typeFake = new Mock<IStructureType>();

            var idAccessorFake = new Mock<IIdAccessor>();
            var conTokenFake = new Mock<IConcurrencyTokenAccessor>();
            var timeStampFake = new Mock<ITimeStampAccessor>();
            var indexAccessorFake = new Mock<IIndexAccessor>();

            indexAccessorFake.Setup(x => x.Path).Returns("Plain");
            indexAccessorFake.Setup(x => x.IsUnique).Returns(false);
            
            var uniqueIndexAccessorFake = new Mock<IIndexAccessor>();
            uniqueIndexAccessorFake.Setup(x => x.Path).Returns("Unique");
            uniqueIndexAccessorFake.Setup(x => x.IsUnique).Returns(true);

            var schema = new StructureSchema(
                typeFake.Object,
                idAccessorFake.Object,
                conTokenFake.Object,
                timeStampFake.Object,
                new[] {indexAccessorFake.Object, uniqueIndexAccessorFake.Object});

            Assert.IsTrue(schema.IndexAccessors.Any(iac => iac.Path == indexAccessorFake.Object.Path));
            Assert.IsTrue(schema.IndexAccessors.Any(iac => iac.Path == uniqueIndexAccessorFake.Object.Path));
        }
    }
}