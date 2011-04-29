using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SisoDb.Structures.Schemas;
using SisoDb.Structures.Schemas.MemberAccessors;
using TypeMock.ArrangeActAssert;

namespace SisoDb.Tests.UnitTests.Structures.Schemas
{
    [TestFixture]
    public class StructureSchemaTests
    {
        [Test, Isolated]
        public void Ctor_WhenNameIsNull_ThrowsArgumentNullException()
        {
            var idAccessor = Isolate.Fake.Instance<IIdAccessor>();

            var ex = Assert.Throws<ArgumentNullException>(() => new StructureSchema(null, idAccessor));

            Assert.IsNotNull(ex);
        }

        [Test]
        public void Ctor_WhenIdAccessorIsNull_ThrowsArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new StructureSchema("The name", null));
            
            Assert.IsNotNull(ex);
        }

        [Test, Isolated]
        public void Ctor_WhenUniqueIndexAccessorsInjected_ExistsInBothListOfIndexAccessors()
        {
            var fakeIIdAccessor = Isolate.Fake.Instance<IIdAccessor>();
            var plainIndexAccessor = Isolate.Fake.Instance<IIndexAccessor>();
            Isolate.WhenCalled(() => plainIndexAccessor.Path).WillReturn("Plain");
            Isolate.WhenCalled(() => plainIndexAccessor.IsUnique).WillReturn(false);
            
            var uniqueIndexAccessor = Isolate.Fake.Instance<IIndexAccessor>();
            Isolate.WhenCalled(() => uniqueIndexAccessor.Path).WillReturn("Unique");
            Isolate.WhenCalled(() => uniqueIndexAccessor.IsUnique).WillReturn(true);

            var schema = new StructureSchema("JustADummyName",
                fakeIIdAccessor,
                new[] {plainIndexAccessor, uniqueIndexAccessor});

            Assert.IsTrue(schema.IndexAccessors.Any(iac => iac.Path == plainIndexAccessor.Path));
            Assert.IsFalse(schema.UniqueIndexAccessors.Any(iac => iac.Path == plainIndexAccessor.Path));
            Assert.IsTrue(schema.IndexAccessors.Any(iac => iac.Path == uniqueIndexAccessor.Path));
            Assert.IsTrue(schema.UniqueIndexAccessors.Any(iac => iac.Path == uniqueIndexAccessor.Path));
        }
    }
}