using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Moq;
using NUnit.Framework;
using SisoDb.Core;
using SisoDb.Cryptography;
using SisoDb.Reflections;
using SisoDb.Resources;
using SisoDb.Structures;
using SisoDb.Structures.Schemas;
using SisoDb.Structures.Schemas.Builders;
using SisoDb.Structures.Schemas.MemberAccessors;
using SisoDb.TestUtils;

namespace SisoDb.Tests.UnitTests.Structures.Schemas.Builders
{
    [TestFixture]
    public class AutoSchemaBuilderTests : UnitTestBase
    {
        private readonly IStructureTypeFactory _structureTypeFactory = new StructureTypeFactory(new StructureTypeReflecter());
        private readonly ISchemaBuilder _schemaBuilder = new AutoSchemaBuilder(new HashService());

        private IStructureType GetStructureTypeFor<T>()
            where T : class
        {
            return _structureTypeFactory.CreateFor(TypeFor<T>.Type);
        }

        [Test]
        public void CreateSchema_WhenNestedType_SchemaNameReflectsTypeName()
        {
            const string expectedName = "WithIdAndIndexableFirstLevelMembers";
            var structureType = GetStructureTypeFor<WithIdAndIndexableFirstLevelMembers>();

            var schema = _schemaBuilder.CreateSchema(structureType);

            Assert.AreEqual(expectedName, schema.Name);
        }

        [Test]
        public void CreateSchema_WhenSecondLevelIndexablePropertiesExists_IndexAccessorsAreCreated()
        {
            var structureType = GetStructureTypeFor<WithFirstSecondAndThirdLevelMembers>();

            var schema = _schemaBuilder.CreateSchema(structureType);

            var hasSecondLevelAccessors = schema.IndexAccessors.Any(iac => HasLevel(iac, 1));
            Assert.IsTrue(hasSecondLevelAccessors);
        }

        [Test]
        public void CreateSchema_WhenSecondLevelIndexablePropertiesExists_PathReflectsHierarchy()
        {
            var structureType = GetStructureTypeFor<WithFirstSecondAndThirdLevelMembers>();

            var schema = _schemaBuilder.CreateSchema(structureType);

            var secondLevelItems = schema.IndexAccessors.Where(iac => HasLevel(iac, 1));
            CustomAssert.ForAll(secondLevelItems, iac => iac.Path.StartsWith("SecondLevelItem."));
        }

        [Test]
        public void CreateSchema_WhenThirdLevelIndexablePropertiesExists_IndexAccessorsAreCreated()
        {
            var structureType = GetStructureTypeFor<WithFirstSecondAndThirdLevelMembers>();

            var schema = _schemaBuilder.CreateSchema(structureType);

            var hasThirdLevelAccessors = schema.IndexAccessors.Any(iac => HasLevel(iac, 2));
            Assert.IsTrue(hasThirdLevelAccessors);
        }

        [Test]
        public void CreateSchema_WhenThirdLevelIndexablePropertiesExists_PathReflectsHierarchy()
        {
            var structureType = GetStructureTypeFor<WithFirstSecondAndThirdLevelMembers>();

            var schema = _schemaBuilder.CreateSchema(structureType);

            var thirdLevelItems = schema.IndexAccessors.Where(iac => HasLevel(iac, 2));
            CustomAssert.ForAll(thirdLevelItems, iac => iac.Path.StartsWith("SecondLevelItem.ThirdLevelItem."));
        }

        [Test]
        public void CreateSchema_WhenItemHasNoIdMember_ThrowsMissingKeyMemberException()
        {
            var structureTypeStub = new Mock<IStructureType>();
            structureTypeStub.Setup(s => s.Name).Returns("TempType");

            var ex = CustomAssert.Throws<SisoDbException>(() => _schemaBuilder.CreateSchema(structureTypeStub.Object));

            var expectedMessage = string.Format(ExceptionMessages.AutoSchemaBuilder_MissingIdMember, "TempType");
            Assert.AreEqual(expectedMessage, ex.Message);
        }

        [Test]
        public void CreateSchema_WhenItemHasIdMember_CreatesSchemaWithKeyMemberAccessor()
        {
            var structureType = GetStructureTypeFor<WithIdAndIndexableFirstLevelMembers>();

            var schema = _schemaBuilder.CreateSchema(structureType);

            Assert.IsNotNull(schema.IdAccessor);
        }

        [Test]
        public void CreateSchema_WhenItemHasIndexableFirstLevelProperties_IndexAccessorsAreExtracted()
        {
            var structureType = GetStructureTypeFor<WithIdAndIndexableFirstLevelMembers>();

            var schema = _schemaBuilder.CreateSchema(structureType);

            CustomAssert.IsNotEmpty(schema.IndexAccessors);
        }

        [Test]
        public void CreateSchema_WhenGuidItemHasNoIndexableFirstLevelProperties_ThrowsMissingIndexableMembersException()
        {
            var structureType = new Mock<IStructureType>();
            structureType.Setup(s => s.Name).Returns("TmpType");
            structureType.Setup(s => s.IdProperty).Returns(() =>
            {
                var idProp = new Mock<IStructureProperty>();
                idProp.Setup(i => i.Name).Returns("SisoId");
                idProp.Setup(i => i.Path).Returns("SisoId");
                idProp.Setup(i => i.IsRootMember).Returns(true);
                idProp.Setup(i => i.PropertyType).Returns(typeof(Guid));
                return idProp.Object;
            });

            var ex = CustomAssert.Throws<SisoDbException>(
                () => _schemaBuilder.CreateSchema(structureType.Object));

            var expectedMessage = string.Format(ExceptionMessages.AutoSchemaBuilder_MissingIndexableMembers, "TmpType");
            Assert.AreEqual(expectedMessage, ex.Message);
        }

        [Test]
        public void CreateSchema_WhenIdentityItemHasNoIndexableFirstLevelProperties_ThrowsMissingIndexableMembersException()
        {
            var structureType = new Mock<IStructureType>();
            structureType.Setup(s => s.Name).Returns("TmpType");
            structureType.Setup(s => s.IdProperty).Returns(() =>
            {
                var idProp = new Mock<IStructureProperty>();
                idProp.Setup(i => i.Name).Returns("SisoId");
                idProp.Setup(i => i.Path).Returns("SisoId");
                idProp.Setup(i => i.IsRootMember).Returns(true);
                idProp.Setup(i => i.PropertyType).Returns(typeof(int));
                return idProp.Object;
            });

            var ex = CustomAssert.Throws<SisoDbException>(
                () => _schemaBuilder.CreateSchema(structureType.Object));

            var expectedMessage = string.Format(ExceptionMessages.AutoSchemaBuilder_MissingIndexableMembers, "TmpType");
            Assert.AreEqual(expectedMessage, ex.Message);
        }

        [Test]
        public void CreateSchema_WhenFirstLevelIdentity_ReturnsSchemaWithIdentityAccessor()
        {
            var structureType = GetStructureTypeFor<WithIdentity>();

            var schema = _schemaBuilder.CreateSchema(structureType);

            Assert.AreEqual(IdTypes.Identity, schema.IdAccessor.IdType);
            Assert.IsNotNull(schema.IdAccessor);
        }

        [Test]
        public void CreateSchema_WhenFirstLevelGuid_ReturnsSchemaWithIdentityAccessor()
        {
            var structureType = GetStructureTypeFor<WithGuid>();

            var schema = _schemaBuilder.CreateSchema(structureType);

            Assert.AreEqual(IdTypes.Guid, schema.IdAccessor.IdType);
            Assert.IsNotNull(schema.IdAccessor);
        }

        [Test]
        public void CreateSchema_WhenByteMember_IndexAccessorIsCreatedForByteMember()
        {
            var structureType = GetStructureTypeFor<WithByte>();

            var schema = _schemaBuilder.CreateSchema(structureType);

            var byteIac = schema.IndexAccessors.SingleOrDefault(iac => iac.Path == "Byte");
            Assert.IsNotNull(byteIac);
            Assert.IsTrue(byteIac.DataType.IsByteType());
        }

        [Test]
        public void CreateSchema_WhenNullableByteMember_IndexAccessorIsCreatedForByteMember()
        {
            var structureType = GetStructureTypeFor<WithNullableByte>();

            var schema = _schemaBuilder.CreateSchema(structureType);

            var byteIac = schema.IndexAccessors.SingleOrDefault(iac => iac.Path == "Byte");
            Assert.IsNotNull(byteIac);
            Assert.IsTrue(byteIac.DataType.IsNullableByteType());
        }

        [Test]
        public void CreateSchema_WhenItemWithByteArray_NoIndexShouldBeCreatedForByteArray()
        {
            var structureType = GetStructureTypeFor<WithBytes>();

            var schema = _schemaBuilder.CreateSchema(structureType);

            Assert.AreEqual(1, schema.IndexAccessors.Count);
            Assert.IsTrue(schema.IndexAccessors[0].Name.StartsWith("DummyMember_"));
        }

        private static bool HasLevel(IIndexAccessor iac, int level)
        {
            var count = iac.Path.Count(ch => ch == '.');

            return count == level;
        }

        private class WithIdentity
        {
            public int SisoId { get; set; }

            public int Int1 { get; set; }
        }

        private class WithGuid
        {
            public Guid SisoId { get; set; }

            public int Int1 { get; set; }
        }

        private class WithByte
        {
            public Guid SisoId { get; set; }

            public byte Byte { get; set; }
        }

        private class WithNullableByte
        {
            public Guid SisoId { get; set; }

            public byte? Byte { get; set; }
        }

        private class WithIdAndIndexableFirstLevelMembers
        {
            public Guid SisoId { get; set; }
            public string Name { get; set; }
            public int Age { get; set; }
        }

        private class WithFirstSecondAndThirdLevelMembers
        {
            public Guid SisoId { get; set; }
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

        private class WithBytes
        {
            public Guid SisoId { get; set; }

            public int DummyMember { get; set; }

            public byte[] Bytes1 { get; set; }

            public IEnumerable<byte> Bytes2 { get; set; }

            public IList<byte> Bytes3 { get; set; }

            public List<byte> Bytes4 { get; set; }

            public ICollection<byte> Bytes5 { get; set; }

            public Collection<byte> Bytes6 { get; set; }
        }
    }
}