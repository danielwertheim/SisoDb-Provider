using System;
using NUnit.Framework;
using SisoDb.Resources;
using SisoDb.Structures.Schemas;
using SisoDb.Structures.Schemas.MemberAccessors;
using SisoDb.TestUtils;

namespace SisoDb.Tests.UnitTests.Structures.Schemas.MemberAccessors
{
    [TestFixture]
    public class IdAccessorTests : UnitTestBase
    {
        [Test]
        public void GetValue_FromAssignedGuidProperty_ReturnsAssignedGuid()
        {
            var id = Guid.Parse("fc47a673-5a5b-419b-9a40-a756591aa7bf");
            var item = new GuidDummy { SisoId = id };
            var property = StructurePropertyTestFactory.GetIdProperty<GuidDummy>();
            
            var idAccessor = new IdAccessor(property);
            var idViaAccessor = idAccessor.GetValue<GuidDummy, Guid>(item);

            Assert.AreEqual(id, idViaAccessor);
        }

        [Test]
        public void GetValue_FromAssignedNullableGuidProperty_ReturnsAssignedGuid()
        {
            var id = Guid.Parse("fc47a673-5a5b-419b-9a40-a756591aa7bf");
            var item = new NullableGuidDummy { SisoId = id };
            var property = StructurePropertyTestFactory.GetIdProperty<NullableGuidDummy>();
            
            var idAccessor = new IdAccessor(property);
            var idViaAccessor = idAccessor.GetValue<NullableGuidDummy, Guid>(item);

            Assert.AreEqual(id, idViaAccessor);
        }

        [Test]
        public void GetValue_FromUnAssignedNullableGuidProperty_ReturnsNull()
        {
            var item = new NullableGuidDummy();
            var property = StructurePropertyTestFactory.GetIdProperty<NullableGuidDummy>();
            
            var idAccessor = new IdAccessor(property);
            var idViaAccessor = idAccessor.GetValue<NullableGuidDummy, Guid>(item);

            Assert.IsNull(idViaAccessor);
        }

        [Test]
        public void GetValue_FromAssignedIdentityProperty_ReturnsAssignedInt()
        {
            const int id = 42;
            var item = new IdentityDummy { SisoId = id };
            var property = StructurePropertyTestFactory.GetIdProperty<IdentityDummy>();
            
            var idAccessor = new IdAccessor(property);
            var idViaAccessor = idAccessor.GetValue<IdentityDummy, int>(item);

            Assert.AreEqual(id, idViaAccessor);
        }

        [Test]
        public void GetValue_FromAssignedNullableIdentityProperty_ReturnsAssignedInt()
        {
            const int id = 42;
            var item = new NullableIdentityDummy { SisoId = id };
            var property = StructurePropertyTestFactory.GetIdProperty<NullableIdentityDummy>();
            
            var idAccessor = new IdAccessor(property);
            var idViaAccessor = idAccessor.GetValue<NullableIdentityDummy, int>(item);

            Assert.AreEqual(id, idViaAccessor);
        }

        [Test]
        public void GetValue_FromUnAssignedNullableIdentityProperty_ReturnsAssignedInt()
        {
            var item = new NullableIdentityDummy();
            var property = StructurePropertyTestFactory.GetIdProperty<NullableIdentityDummy>();

            var idAccessor = new IdAccessor(property);
            var idViaAccessor = idAccessor.GetValue<NullableIdentityDummy, int>(item);

            Assert.IsNull(idViaAccessor);
        }

        [Test]
        public void Ctor_WhenIntNotOnFirstLevel_ThrowsSisoDbException()
        {
            var itemPropertyInfo = typeof(Container).GetProperty("NestedWithIdentity");
            var itemProperty = new StructureProperty(itemPropertyInfo);

            var intPropertyInfo = typeof(IdentityDummy).GetProperty("SisoId");
            var intProperty = new StructureProperty(itemProperty, intPropertyInfo);

            var ex = Assert.Throws<SisoDbException>(() => new IdAccessor(intProperty));

            Assert.AreEqual(ExceptionMessages.IdAccessor_GetIdValue_InvalidLevel, ex.Message);
        }

        [Test]
        public void Ctor_WhenGuidNotOnFirstLevel_ThrowsSisoDbException()
        {
            var itemPropertyInfo = typeof(Container).GetProperty("NestedWithGuid");
            var itemProperty = new StructureProperty(itemPropertyInfo);

            var guidPropertyInfo = typeof(GuidDummy).GetProperty("SisoId");
            var guidProperty = new StructureProperty(itemProperty, guidPropertyInfo);

            var ex = CustomAssert.Throws<SisoDbException>(() => new IdAccessor(guidProperty));

            Assert.AreEqual(ExceptionMessages.IdAccessor_GetIdValue_InvalidLevel, ex.Message);
        }

        [Test]
        public void SetValue_ToGuidProperty_ValueIsAssigned()
        {
            var id = Guid.Parse("fc47a673-5a5b-419b-9a40-a756591aa7bf");
            var item = new GuidDummy();

            var property = StructurePropertyTestFactory.GetIdProperty<GuidDummy>();
            var idAccessor = new IdAccessor(property);
            idAccessor.SetValue(item, id);

            Assert.AreEqual(id, item.SisoId);
        }

        [Test]
        public void SetValue_ToIntProperty_ValueIsAssigned()
        {
            const int id = 42;
            var item = new IdentityDummy();

            var property = StructurePropertyTestFactory.GetIdProperty<IdentityDummy>();
            var idAccessor = new IdAccessor(property);
            idAccessor.SetValue(item, id);

            Assert.AreEqual(id, item.SisoId);
        }

        [Test]
        public void SetValue_ToNullableGuidProperty_ValueIsAssigned()
        {
            var id = Guid.Parse("fc47a673-5a5b-419b-9a40-a756591aa7bf");
            var item = new NullableGuidDummy();

            var property = StructurePropertyTestFactory.GetIdProperty<NullableGuidDummy>();
            var idAccessor = new IdAccessor(property);
            idAccessor.SetValue(item, id);

            Assert.AreEqual(id, item.SisoId);
        }

        [Test]
        public void SetValue_ToNullableIntProperty_ValueIsAssigned()
        {
            const int id = 42;
            var item = new NullableIdentityDummy();

            var property = StructurePropertyTestFactory.GetIdProperty<NullableIdentityDummy>();
            var idAccessor = new IdAccessor(property);
            idAccessor.SetValue(item, id);

            Assert.AreEqual(id, item.SisoId);
        }

        private class Container
        {
            public IdentityDummy NestedWithIdentity { get; set; }

            public GuidDummy NestedWithGuid { get; set; }
        }

        private class GuidDummy
        {
            public Guid SisoId { get; set; }
        }

        private class NullableGuidDummy
        {
            public Guid? SisoId { get; set; }
        }

        private class IdentityDummy
        {
            public int SisoId { get; set; }
        }

        private class NullableIdentityDummy
        {
            public int? SisoId { get; set; }
        }
    }
}