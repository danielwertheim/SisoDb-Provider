using System;
using NUnit.Framework;
using SisoDb.NCore;
using SisoDb.Resources;
using SisoDb.Structures.Schemas.MemberAccessors;

namespace SisoDb.UnitTests.Structures.Schemas.MemberAccessors
{
    [TestFixture]
    public class ConcurrencyTokenAccessorTests : UnitTestBase
    {
        [Test]
        public void Ctor_WhenMemberIsNotOnRootLevel_ThrowsException()
        {
            var concTokenProperty = StructurePropertyTestFactory.GetPropertyByPath<ModelWithMemberNotInRoot>("NestedModelItem.ConcurrencyToken");

            var ex = Assert.Throws<SisoDbException>(() => new ConcurrencyTokenAccessor(concTokenProperty));

            Assert.AreEqual(ExceptionMessages.ConcurrencyTokenAccessor_InvalidLevel.Inject(concTokenProperty.Name), ex.Message);
        }

        [Test]
        public void Ctor_WhenMemberIsNotGuidIntOrLong_ThrowsException()
        {
            var concTokenProperty = StructurePropertyTestFactory.GetPropertyByPath<ModelWithStringMember>("ConcurrencyToken");

            var ex = Assert.Throws<SisoDbException>(() => new ConcurrencyTokenAccessor(concTokenProperty));

            Assert.AreEqual(ExceptionMessages.ConcurrencyTokenAccessor_Invalid_Type.Inject(concTokenProperty.Name), ex.Message);
        }

        [Test]
        public void GetValue_WhenAssignedGuidExistsInModel_ReturnsGuid()
        {
            var initialToken = Guid.Parse("de7d7fcb-ccd0-46d2-b3e2-cd4a357c697f");
            var concTokenProperty = StructurePropertyTestFactory.GetPropertyByPath<ModelWithGuidMember>("ConcurrencyToken");
            var accessor = new ConcurrencyTokenAccessor(concTokenProperty);
            var model = new ModelWithGuidMember {ConcurrencyToken = initialToken};

            var token = accessor.GetValue(model);

            Assert.AreEqual(initialToken, token);
        }

        [Test]
        public void GetValue_WhenAssignedIntExistsInModel_ReturnsGuid()
        {
            const int initialToken = 42;
            var concTokenProperty = StructurePropertyTestFactory.GetPropertyByPath<ModelWithIntMember>("ConcurrencyToken");
            var accessor = new ConcurrencyTokenAccessor(concTokenProperty);
            var model = new ModelWithIntMember { ConcurrencyToken = initialToken };

            var token = accessor.GetValue(model);

            Assert.AreEqual(initialToken, token);
        }

        [Test]
        public void GetValue_WhenAssignedLongExistsInModel_ReturnsGuid()
        {
            const long initialToken = (long)42;
            var concTokenProperty = StructurePropertyTestFactory.GetPropertyByPath<ModelWithBigIntMember>("ConcurrencyToken");
            var accessor = new ConcurrencyTokenAccessor(concTokenProperty);
            var model = new ModelWithBigIntMember { ConcurrencyToken = initialToken };

            var token = accessor.GetValue(model);

            Assert.AreEqual(initialToken, token);
        }

        [Test]
        public void SetValue_WhenAssigningNewGuidOnModel_UpdatesGuidOnModel()
        {
            var initialToken = Guid.Parse("de7d7fcb-ccd0-46d2-b3e2-cd4a357c697f");
            var assignedToken = Guid.Parse("f13185dd-1145-4e63-a53f-a0e22dda3e03");
            var concTokenProperty = StructurePropertyTestFactory.GetPropertyByPath<ModelWithGuidMember>("ConcurrencyToken");
            var accessor = new ConcurrencyTokenAccessor(concTokenProperty);
            var model = new ModelWithGuidMember { ConcurrencyToken = initialToken };

            accessor.SetValue(model, assignedToken);

            Assert.AreEqual(assignedToken, model.ConcurrencyToken);
        }

        [Test]
        public void SetValue_WhenAssigningNewIntOnModel_UpdatesGuidOnModel()
        {
            const int initialToken = 42;
            const int assignedToken = 43;
            var concTokenProperty = StructurePropertyTestFactory.GetPropertyByPath<ModelWithIntMember>("ConcurrencyToken");
            var accessor = new ConcurrencyTokenAccessor(concTokenProperty);
            var model = new ModelWithIntMember { ConcurrencyToken = initialToken };

            accessor.SetValue(model, assignedToken);

            Assert.AreEqual(assignedToken, model.ConcurrencyToken);
        }

        [Test]
        public void SetValue_WhenAssigningNewLongOnModel_UpdatesGuidOnModel()
        {
            const long initialToken = (long)42;
            const long assignedToken = (long)43;
            var concTokenProperty = StructurePropertyTestFactory.GetPropertyByPath<ModelWithBigIntMember>("ConcurrencyToken");
            var accessor = new ConcurrencyTokenAccessor(concTokenProperty);
            var model = new ModelWithBigIntMember { ConcurrencyToken = initialToken };

            accessor.SetValue(model, assignedToken);

            Assert.AreEqual(assignedToken, model.ConcurrencyToken);
        }

        private class ModelWithGuidMember
        {
            public Guid ConcurrencyToken { get; set; }
        }

        private class ModelWithIntMember
        {
            public int ConcurrencyToken { get; set; }
        }

        private class ModelWithBigIntMember
        {
            public long ConcurrencyToken { get; set; }
        }

        private class ModelWithStringMember
        {
            public string ConcurrencyToken { get; set; }
        }

        private class ModelWithMemberNotInRoot
        {
            public ModelWithGuidMember NestedModelItem { get; set; }
        }
    }
}