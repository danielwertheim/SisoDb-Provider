using System;
using NUnit.Framework;
using SisoDb.Structures.Schemas;
using SisoDb.Structures.Schemas.MemberAccessors;

namespace SisoDb.Tests.UnitTests.Structures.Schemas.MemberAccessors
{
    [TestFixture]
    public class IdAccessorTests : UnitTestBase
    {
        [Test]
        public void GetValue_FromAssignedGuidProperty_ReturnsAssignedGuid()
        {
            var id = Guid.Parse("fc47a673-5a5b-419b-9a40-a756591aa7bf");
            var item = new GuidDummy { Id = id };
            var property = StructureTypeInfo<GuidDummy>.IdProperty;
            
            var idAccessor = new IdAccessor(property);
            var idViaAccessor = idAccessor.GetValue<Guid>(item);

            Assert.AreEqual(id, idViaAccessor);
        }

        [Test]
        public void GetValue_FromAssignedNullableGuidProperty_ReturnsAssignedGuid()
        {
            var id = Guid.Parse("fc47a673-5a5b-419b-9a40-a756591aa7bf");
            var item = new NullableGuidDummy { Id = id };
            var property = StructureTypeInfo<NullableGuidDummy>.IdProperty;
            
            var idAccessor = new IdAccessor(property);
            var idViaAccessor = idAccessor.GetValue<Guid>(item);

            Assert.AreEqual(id, idViaAccessor);
        }

        [Test]
        public void GetValue_FromUnAssignedNullableGuidProperty_ReturnsNull()
        {
            var item = new NullableGuidDummy();
            var property = StructureTypeInfo<NullableGuidDummy>.IdProperty;
            
            var idAccessor = new IdAccessor(property);
            var idViaAccessor = idAccessor.GetValue<Guid>(item);

            Assert.IsNull(idViaAccessor);
        }

        [Test]
        public void GetValue_FromAssignedIdentityProperty_ReturnsAssignedInt()
        {
            const int id = 42;
            var item = new IdentityDummy { Id = id };
            var property = StructureTypeInfo<IdentityDummy>.IdProperty;

            var idAccessor = new IdAccessor(property);
            var idViaAccessor = idAccessor.GetValue<int>(item);

            Assert.AreEqual(id, idViaAccessor);
        }

        [Test]
        public void GetValue_FromAssignedNullableIdentityProperty_ReturnsAssignedInt()
        {
            const int id = 42;
            var item = new NullableIdentityDummy { Id = id };
            var property = StructureTypeInfo<NullableIdentityDummy>.IdProperty;

            var idAccessor = new IdAccessor(property);
            var idViaAccessor = idAccessor.GetValue<int>(item);

            Assert.AreEqual(id, idViaAccessor);
        }

        [Test]
        public void GetValue_FromUnAssignedNullableIdentityProperty_ReturnsAssignedInt()
        {
            var item = new NullableIdentityDummy();
            var property = StructureTypeInfo<NullableIdentityDummy>.IdProperty;

            var idAccessor = new IdAccessor(property);
            var idViaAccessor = idAccessor.GetValue<int>(item);

            Assert.IsNull(idViaAccessor);
        }

        [Test]
        public void SetValue_ToGuidProperty_ValueIsAssigned()
        {
            var id = Guid.Parse("fc47a673-5a5b-419b-9a40-a756591aa7bf");
            var item = new GuidDummy();

            var property = StructureTypeInfo<GuidDummy>.IdProperty;
            var idAccessor = new IdAccessor(property);
            idAccessor.SetValue(item, id);

            Assert.AreEqual(id, item.Id);
        }

        [Test]
        public void SetValue_ToIntProperty_ValueIsAssigned()
        {
            const int id = 42;
            var item = new IdentityDummy();

            var property = StructureTypeInfo<IdentityDummy>.IdProperty;
            var idAccessor = new IdAccessor(property);
            idAccessor.SetValue(item, id);

            Assert.AreEqual(id, item.Id);
        }

        [Test]
        public void SetValue_ToNullableGuidProperty_ValueIsAssigned()
        {
            var id = Guid.Parse("fc47a673-5a5b-419b-9a40-a756591aa7bf");
            var item = new NullableGuidDummy();

            var property = StructureTypeInfo<NullableGuidDummy>.IdProperty;
            var idAccessor = new IdAccessor(property);
            idAccessor.SetValue(item, id);

            Assert.AreEqual(id, item.Id);
        }

        [Test]
        public void SetValue_ToNullableIntProperty_ValueIsAssigned()
        {
            const int id = 42;
            var item = new NullableIdentityDummy();

            var property = StructureTypeInfo<NullableIdentityDummy>.IdProperty;
            var idAccessor = new IdAccessor(property);
            idAccessor.SetValue(item, id);

            Assert.AreEqual(id, item.Id);
        }

        private class GuidDummy
        {
            public Guid Id { get; set; }
        }

        private class NullableGuidDummy
        {
            public Guid? Id { get; set; }
        }

        private class IdentityDummy
        {
            public int Id { get; set; }
        }

        private class NullableIdentityDummy
        {
            public int? Id { get; set; }
        }
    }
}