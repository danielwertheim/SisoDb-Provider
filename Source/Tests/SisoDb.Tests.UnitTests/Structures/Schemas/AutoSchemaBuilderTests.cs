using System;
using System.Linq;
using NUnit.Framework;
using SisoDb.Reflections;
using SisoDb.Resources;
using SisoDb.Structures;
using SisoDb.Structures.Schemas;
using SisoDb.Structures.Schemas.MemberAccessors;
using SisoDb.TestUtils;

namespace SisoDb.Tests.UnitTests.Structures.Schemas
{
    [TestFixture]
    public class AutoSchemaBuilderTests : UnitTestBase
    {
        [Test]
        public void CreateSchema_WhenNestedType_SchemaNameReflectsTypeName()
        {
            const string expectedName = "WithIdAndIndexableFirstLevelMembers";
            var builder = new AutoSchemaBuilder<WithIdAndIndexableFirstLevelMembers>();

            var schema = builder.CreateSchema();

            Assert.AreEqual(expectedName, schema.Name);
        }

        [Test]
        public void CreateSchema_WhenSecondLevelIndexablePropertiesExists_IndexAccessorsAreCreated()
        {
            var builder = new AutoSchemaBuilder<WithFirstSecondAndThirdLevelMembers>();

            var schema = builder.CreateSchema();

            var hasSecondLevelAccessors = schema.IndexAccessors.Any(iac => HasLevel(iac, 1));
            Assert.IsTrue(hasSecondLevelAccessors);
        }

        [Test]
        public void CreateSchema_WhenSecondLevelIndexablePropertiesExists_PathReflectsHierarchy()
        {
            var builder = new AutoSchemaBuilder<WithFirstSecondAndThirdLevelMembers>();

            var schema = builder.CreateSchema();

            var secondLevelItems = schema.IndexAccessors.Where(iac => HasLevel(iac, 1));
            CustomAssert.ForAll(secondLevelItems, iac => iac.Path.StartsWith("SecondLevelItem."));
        }

        [Test]
        public void CreateSchema_WhenThirdLevelIndexablePropertiesExists_IndexAccessorsAreCreated()
        {
            var builder = new AutoSchemaBuilder<WithFirstSecondAndThirdLevelMembers>();

            var schema = builder.CreateSchema();

            var hasThirdLevelAccessors = schema.IndexAccessors.Any(iac => HasLevel(iac, 2));
            Assert.IsTrue(hasThirdLevelAccessors);
        }

        [Test]
        public void CreateSchema_WhenThirdLevelIndexablePropertiesExists_PathReflectsHierarchy()
        {
            var builder = new AutoSchemaBuilder<WithFirstSecondAndThirdLevelMembers>();

            var schema = builder.CreateSchema();

            var thirdLevelItems = schema.IndexAccessors.Where(iac => HasLevel(iac, 2));
            CustomAssert.ForAll(thirdLevelItems, iac => iac.Path.StartsWith("SecondLevelItem.ThirdLevelItem."));
        }

        [Test]
        public void CreateSchema_WhenItemHasNoIdMember_ThrowsMissingKeyMemberException()
        {
            var builder = new AutoSchemaBuilder<WithNoId>();
            
            var ex = CustomAssert.Throws<SisoDbException>(() => builder.CreateSchema());

            var expectedMessage = string.Format(ExceptionMessages.AutoSchemaBuilder_MissingIdMember, "WithNoId");
            Assert.AreEqual(expectedMessage, ex.Message);
        }

        [Test]
        public void CreateSchema_WhenItemHasIdMember_CreatesSchemaWithKeyMemberAccessor()
        {
            var builder = new AutoSchemaBuilder<WithIdAndIndexableFirstLevelMembers>();

            var schema = builder.CreateSchema();

            Assert.IsNotNull(schema.IdAccessor);
        }

        [Test]
        public void CreateSchema_WhenItemHasIndexableFirstLevelProperties_IndexAccessorsAreExtracted()
        {
            var builder = new AutoSchemaBuilder<WithIdAndIndexableFirstLevelMembers>();

            var schema = builder.CreateSchema();

            CustomAssert.IsNotEmpty(schema.IndexAccessors);
        }

        [Test]
        public void CreateSchema_WhenGuidItemHasNoIndexableFirstLevelProperties_ThrowsMissingIndexableMembersException()
        {
            var builder = new AutoSchemaBuilder<WithOnlyId>();

            var ex = CustomAssert.Throws<SisoDbException>(
                () => builder.CreateSchema());

            var expectedMessage = string.Format(ExceptionMessages.AutoSchemaBuilder_MissingIndexableMembers, "WithOnlyId");
            Assert.AreEqual(expectedMessage, ex.Message);
        }

        [Test]
        public void CreateSchema_WhenIdentityItemHasNoIndexableFirstLevelProperties_ThrowsMissingIndexableMembersException()
        {
            var builder = new AutoSchemaBuilder<WithOnlyIdentity>();

            var ex = CustomAssert.Throws<SisoDbException>(
                () => builder.CreateSchema());
            
            var expectedMessage = string.Format(ExceptionMessages.AutoSchemaBuilder_MissingIndexableMembers, "WithOnlyIdentity");
            Assert.AreEqual(expectedMessage, ex.Message);
        }

        [Test]
        public void CreateSchema_WhenFirstLevelIdentity_ReturnsSchemaWithIdentityAccessor()
        {
            var builder = new AutoSchemaBuilder<WithIdentity>();
            
            var schema = builder.CreateSchema();

            Assert.AreEqual(IdTypes.Identity, schema.IdAccessor.IdType);
            Assert.IsNotNull(schema.IdAccessor);
        }

        [Test]
        public void CreateSchema_WhenFirstLevelGuid_ReturnsSchemaWithIdentityAccessor()
        {
            var builder = new AutoSchemaBuilder<WithGuid>();
            
            var schema = builder.CreateSchema();

            Assert.AreEqual(IdTypes.Guid, schema.IdAccessor.IdType);
            Assert.IsNotNull(schema.IdAccessor);
        }

        [Test]
        public void CreateSchema_WhenByteMember_IndexAccessorIsCreatedForByteMember()
        {
            var builder = new AutoSchemaBuilder<WithByte>();
            
            var schema = builder.CreateSchema();

            var byteIac = schema.IndexAccessors.SingleOrDefault(iac => iac.Path == "Byte");
            Assert.IsNotNull(byteIac);
            Assert.IsTrue(byteIac.DataType.IsByteType());
        }

        [Test]
        public void CreateSchema_WhenNullableByteMember_IndexAccessorIsCreatedForByteMember()
        {
            var builder = new AutoSchemaBuilder<WithNullableByte>();

            var schema = builder.CreateSchema();

            var byteIac = schema.IndexAccessors.SingleOrDefault(iac => iac.Path == "Byte");
            Assert.IsNotNull(byteIac);
            Assert.IsTrue(byteIac.DataType.IsNullableByteType());
        }

        [Test]
        public void CreateSchema_WhenByteArrayMember_NoIndexAccessorIsCreatedForByteArrayMember()
        {
            var builder = new AutoSchemaBuilder<WithBytes>();

            var ex = Assert.Throws<SisoDbException>(() => builder.CreateSchema());

            Assert.AreEqual(
                ExceptionMessages.AutoSchemaBuilder_MissingIndexableMembers.Inject("WithBytes"),
                ex.Message);
        }

        private static bool HasLevel(IIndexAccessor iac, int level)
        {
            var count = iac.Path.Count(ch => ch == '.');

            return count == level;
        }

        private class WithNoId
        {
        }

        private class WithOnlyId
        {
            public Guid Id { get; set; }
        }

        private class WithOnlyIdentity
        {
            public int Id { get; set; }
        }

        private class WithIdentity
        {
            public int Id { get; set; }

            public int Int1 { get; set; }
        }

        private class WithGuid
        {
            public Guid Id { get; set; }

            public int Int1 { get; set; }
        }

        private class WithByte
        {
            public Guid Id { get; set; }

            public byte Byte { get; set; }
        }

        private class WithNullableByte
        {
            public Guid Id { get; set; }

            public byte? Byte { get; set; }
        }

        private class WithBytes
        {
            public Guid Id { get; set; }

            public byte[] Bytes { get; set; }
        }

        private class WithIdAndIndexableFirstLevelMembers
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public int Age { get; set; }
        }

        private class WithFirstSecondAndThirdLevelMembers
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public int Age { get; set; }

            public SecondLevelItem SecondLevelItem { get; set; }
        }

        private class SecondLevelItem
        {
            public string Street { get; set; }
            public string Zip { get; set; }
            public string City { get; set; }

            public ThirdLevelItem ThirdLevelItem { get; set; }
        }

        private class ThirdLevelItem
        {
            public int AreaCode { get; set; }
            public int Number { get; set; }
        }
    }
}