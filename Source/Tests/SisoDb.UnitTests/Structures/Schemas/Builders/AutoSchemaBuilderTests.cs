using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Moq;
using NUnit.Framework;
using SisoDb.NCore.Reflections;
using SisoDb.Resources;
using SisoDb.Structures.Schemas;
using SisoDb.Structures.Schemas.Builders;
using SisoDb.Structures.Schemas.MemberAccessors;

namespace SisoDb.UnitTests.Structures.Schemas.Builders
{
    [TestFixture]
    public class AutoSchemaBuilderTests : UnitTestBase
    {
        private readonly IStructureTypeFactory _structureTypeFactory = new StructureTypeFactory();
        private readonly IStructureSchemaBuilder _structureSchemaBuilder = new AutoStructureSchemaBuilder();

        private IStructureType GetStructureTypeFor<T>()
            where T : class
        {
            return _structureTypeFactory.CreateFor(typeof(T));
        }

        [Test]
        public void CreateSchema_WhenNestedType_SchemaNameReflectsTypeName()
        {
            const string expectedName = "WithIdAndIndexableFirstLevelMembers";
            var structureType = GetStructureTypeFor<WithIdAndIndexableFirstLevelMembers>();

            var schema = _structureSchemaBuilder.CreateSchema(structureType);

            Assert.AreEqual(expectedName, schema.Name);
        }

        [Test]
        public void CreateSchema_WhenSecondLevelIndexablePropertiesExists_IndexAccessorsAreCreated()
        {
            var structureType = GetStructureTypeFor<WithFirstSecondAndThirdLevelMembers>();

            var schema = _structureSchemaBuilder.CreateSchema(structureType);

            var hasSecondLevelAccessors = schema.IndexAccessors.Any(iac => HasLevel(iac, 1));
            Assert.IsTrue(hasSecondLevelAccessors);
        }

        [Test]
        public void CreateSchema_WhenSecondLevelIndexablePropertiesExists_PathReflectsHierarchy()
        {
            var structureType = GetStructureTypeFor<WithFirstSecondAndThirdLevelMembers>();

            var schema = _structureSchemaBuilder.CreateSchema(structureType);

            var secondLevelItems = schema.IndexAccessors.Where(iac => HasLevel(iac, 1));
            CustomAssert.ForAll(secondLevelItems, iac => iac.Path.StartsWith("SecondLevelItem."));
        }

        [Test]
        public void CreateSchema_WhenThirdLevelIndexablePropertiesExists_IndexAccessorsAreCreated()
        {
            var structureType = GetStructureTypeFor<WithFirstSecondAndThirdLevelMembers>();

            var schema = _structureSchemaBuilder.CreateSchema(structureType);

            var hasThirdLevelAccessors = schema.IndexAccessors.Any(iac => HasLevel(iac, 2));
            Assert.IsTrue(hasThirdLevelAccessors);
        }

        [Test]
        public void CreateSchema_WhenThirdLevelIndexablePropertiesExists_PathReflectsHierarchy()
        {
            var structureType = GetStructureTypeFor<WithFirstSecondAndThirdLevelMembers>();

            var schema = _structureSchemaBuilder.CreateSchema(structureType);

            var thirdLevelItems = schema.IndexAccessors.Where(iac => HasLevel(iac, 2));
            CustomAssert.ForAll(thirdLevelItems, iac => iac.Path.StartsWith("SecondLevelItem.ThirdLevelItem."));
        }

        [Test]
        public void CreateSchema_WhenThirdLevelIndexableEnumerablePropertiesExists_IndexAccessorsAreCreated()
        {
            var structureType = GetStructureTypeFor<WithFirstSecondAndThirdLevelMembers>();

            var schema = _structureSchemaBuilder.CreateSchema(structureType);

            var hasThirdLevelAccessors = schema.IndexAccessors.Any(iac => HasLevel(iac, 2) && iac.Path == "SecondLevelItem.ThirdLevelItem.Numbers");
            Assert.IsTrue(hasThirdLevelAccessors);
        }

        [Test]
        public void CreateSchema_WhenItemHasNoIdMember_AndAllowMissingIdMemberIsFalse_ThrowsMissingIdMemberException()
        {
			var structureType = GetStructureTypeFor<WithNoId>();

            var ex = Assert.Throws<SisoDbException>(() => _structureSchemaBuilder.CreateSchema(structureType));

            var expectedMessage = string.Format(ExceptionMessages.AutoSchemaBuilder_MissingIdMember, "WithNoId");
            Assert.AreEqual(expectedMessage, ex.Message);
        }

        [Test]
        public void CreateSchema_WhenItemHasIdMember_CreatesSchemaWithIdMemberAccessor()
        {
            var structureType = GetStructureTypeFor<WithGuidId>();

            var schema = _structureSchemaBuilder.CreateSchema(structureType);

            Assert.IsNotNull(schema.IdAccessor);
        }

		[Test]
		public void CreateSchema_WhenItemHasIdMemberOfTypeName_CreatesSchemaWithIdMemberAccessor()
		{
			var structureType = GetStructureTypeFor<WithCustomIdOfTypeName>();

			var schema = _structureSchemaBuilder.CreateSchema(structureType);

			Assert.IsNotNull(schema.IdAccessor);
		}

		[Test]
		public void CreateSchema_WhenItemHasIdMemberWithNameId_CreatesSchemaWithIdMemberAccessor()
		{
			var structureType = GetStructureTypeFor<WithId>();

			var schema = _structureSchemaBuilder.CreateSchema(structureType);

			Assert.IsNotNull(schema.IdAccessor);
		}

        [Test]
        public void CreateSchema_WhenItemHasIndexableFirstLevelProperties_IndexAccessorsAreExtracted()
        {
            var structureType = GetStructureTypeFor<WithIdAndIndexableFirstLevelMembers>();

            var schema = _structureSchemaBuilder.CreateSchema(structureType);

            CollectionAssert.IsNotEmpty(schema.IndexAccessors);
        }

        [Test]
        public void CreateSchema_WhenGuidItemHasNoIndexableFirstLevelProperties_ThrowsMissingIndexableMembersException()
        {
            var structureType = new Mock<IStructureType>();
            structureType.Setup(s => s.Name).Returns("TmpType");
            structureType.Setup(s => s.IdProperty).Returns(() =>
            {
                var idProp = new Mock<IStructureProperty>();
                idProp.Setup(i => i.Name).Returns("StructureId");
                idProp.Setup(i => i.Path).Returns("StructureId");
                idProp.Setup(i => i.IsRootMember).Returns(true);
                idProp.Setup(i => i.DataType).Returns(typeof(Guid));
                return idProp.Object;
            });

            var ex = Assert.Throws<SisoDbException>(
                () => _structureSchemaBuilder.CreateSchema(structureType.Object));

            var expectedMessage = string.Format(ExceptionMessages.AutoSchemaBuilder_MissingIndexableMembers, "TmpType");
            Assert.AreEqual(expectedMessage, ex.Message);
        }

        [Test]
        public void CreateSchema_WhenItemHasNoIndexableFirstLevelProperties_ThrowsMissingIndexableMembersException()
        {
            var structureType = new Mock<IStructureType>();
            structureType.Setup(s => s.Name).Returns("TmpType");
            structureType.Setup(s => s.IdProperty).Returns(() =>
            {
                var idProp = new Mock<IStructureProperty>();
                idProp.Setup(i => i.Name).Returns("StructureId");
                idProp.Setup(i => i.Path).Returns("StructureId");
                idProp.Setup(i => i.IsRootMember).Returns(true);
                idProp.Setup(i => i.DataType).Returns(typeof(Guid));
                return idProp.Object;
            });

            var ex = Assert.Throws<SisoDbException>(
                () => _structureSchemaBuilder.CreateSchema(structureType.Object));

            var expectedMessage = string.Format(ExceptionMessages.AutoSchemaBuilder_MissingIndexableMembers, "TmpType");
            Assert.AreEqual(expectedMessage, ex.Message);
        }

        [Test]
        public void CreateSchema_WhenFirstLevelGuid_ReturnsSchemaWithIdentityAccessor()
        {
            var structureType = GetStructureTypeFor<WithGuidId>();

            var schema = _structureSchemaBuilder.CreateSchema(structureType);

            Assert.AreEqual(typeof(Guid), schema.IdAccessor.DataType);
            Assert.IsNotNull(schema.IdAccessor);
        }

        [Test]
        public void CreateSchema_WhenByteMember_IndexAccessorIsCreatedForByteMember()
        {
            var structureType = GetStructureTypeFor<WithByte>();

            var schema = _structureSchemaBuilder.CreateSchema(structureType);

            var byteIac = schema.IndexAccessors.SingleOrDefault(iac => iac.Path == "Byte");
            Assert.IsNotNull(byteIac);
            Assert.IsTrue(byteIac.DataType.IsByteType());
        }

        [Test]
        public void CreateSchema_WhenNullableByteMember_IndexAccessorIsCreatedForByteMember()
        {
            var structureType = GetStructureTypeFor<WithNullableByte>();

            var schema = _structureSchemaBuilder.CreateSchema(structureType);

            var byteIac = schema.IndexAccessors.SingleOrDefault(iac => iac.Path == "Byte");
            Assert.IsNotNull(byteIac);
            Assert.IsTrue(byteIac.DataType.IsNullableByteType());
        }

        [Test]
        public void CreateSchema_WhenItemWithByteArray_NoIndexShouldBeCreatedForByteArray()
        {
            var structureType = GetStructureTypeFor<WithBytes>();

            var schema = _structureSchemaBuilder.CreateSchema(structureType);

            Assert.AreEqual(1, schema.IndexAccessors.Count(iac => iac.Path != StructureIdPropertyNames.Default));
            Assert.IsTrue(schema.IndexAccessors[1].Path.StartsWith("DummyMember"));
        }

		[Test]
		public void CreateSchema_WhenClassContainsStructMember_StructMemberIsRepresentedInSchema()
		{
			var structureType = GetStructureTypeFor<WithStruct>();

			var schema = _structureSchemaBuilder.CreateSchema(structureType);

			Assert.AreEqual(2, schema.IndexAccessors.Count);
			Assert.AreEqual("Content", schema.IndexAccessors[1].Path);
			Assert.AreEqual(typeof(MyText), schema.IndexAccessors[1].DataType);
		}

        [Test]
        public void CreateSchema_WhenClassContainsTimeStamp_TimeStampMemberIsRepresentedInSchema()
        {
            var structureType = GetStructureTypeFor<WithTimeStamp>();

            var schema = _structureSchemaBuilder.CreateSchema(structureType);

            Assert.AreEqual(2, schema.IndexAccessors.Count);
            Assert.IsTrue(schema.HasTimeStamp);
            Assert.AreEqual(typeof(DateTime), schema.TimeStampAccessor.DataType);
            Assert.AreEqual("TimeStamp", schema.TimeStampAccessor.Path);

            Assert.AreEqual("TimeStamp", schema.IndexAccessors[1].Path);
            Assert.AreEqual(typeof(DateTime), schema.IndexAccessors[1].DataType);
        }

        [Test]
        public void CreateSchema_WhenClassContainsNullableTimeStamp_TimeStampMemberIsRepresentedInSchema()
        {
            var structureType = GetStructureTypeFor<WithNullableTimeStamp>();

            var schema = _structureSchemaBuilder.CreateSchema(structureType);

            Assert.AreEqual(2, schema.IndexAccessors.Count);
            Assert.IsTrue(schema.HasTimeStamp);
            Assert.AreEqual(typeof(DateTime?), schema.TimeStampAccessor.DataType);
            Assert.AreEqual("TimeStamp", schema.TimeStampAccessor.Path);
            
            Assert.AreEqual("TimeStamp", schema.IndexAccessors[1].Path);
            Assert.AreEqual(typeof(DateTime?), schema.IndexAccessors[1].DataType);
        }

        [Test]
        public void CreateSchema_WhenClassContainsConcurrencyToken_IsRepresentedAsSpecificMemberAsWellAsIndexAccessor()
        {
            var structureType = GetStructureTypeFor<WithConcurrencyToken>();

            var schema = _structureSchemaBuilder.CreateSchema(structureType);

            Assert.AreEqual(2, schema.IndexAccessors.Count);
            Assert.AreEqual("StructureId", schema.IndexAccessors[0].Path);
            Assert.AreEqual("ConcurrencyToken", schema.IndexAccessors[1].Path);

            Assert.IsTrue(schema.HasConcurrencyToken);
            Assert.AreEqual(typeof(Guid), schema.ConcurrencyTokenAccessor.DataType);
            Assert.AreEqual("ConcurrencyToken", schema.ConcurrencyTokenAccessor.Path);
        }

        private static bool HasLevel(IIndexAccessor iac, int level)
        {
            var count = iac.Path.Count(ch => ch == '.');

            return count == level;
        }

		private class WithNoId
		{
			public int Int1 { get; set; } 
		}

        private class WithGuidId
        {
            public Guid StructureId { get; set; }

            public int Int1 { get; set; }
        }

		private class WithCustomIdOfTypeName
		{
			public Guid WithCustomIdOfTypeNameId { get; set; }

			public int Int1 { get; set; }
		}

		private class WithId
		{
			public Guid Id { get; set; }

			public int Int1 { get; set; }
		}

        private class WithByte
        {
            public Guid StructureId { get; set; }

            public byte Byte { get; set; }
        }

        private class WithNullableByte
        {
            public Guid StructureId { get; set; }

            public byte? Byte { get; set; }
        }

        private class WithIdAndIndexableFirstLevelMembers
        {
            public Guid StructureId { get; set; }
            public string Name { get; set; }
            public int Age { get; set; }
        }

        private class WithFirstSecondAndThirdLevelMembers
        {
            public Guid StructureId { get; set; }
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
            public int[] Numbers { get; set; }
        }

        private class WithBytes
        {
            public Guid StructureId { get; set; }

            public int DummyMember { get; set; }

            public byte[] Bytes1 { get; set; }

            public IEnumerable<byte> Bytes2 { get; set; }

            public IList<byte> Bytes3 { get; set; }

            public List<byte> Bytes4 { get; set; }

            public ICollection<byte> Bytes5 { get; set; }

            public Collection<byte> Bytes6 { get; set; }
        }

        private class WithTimeStamp
        {
            public Guid StructureId { get; set; }
            public DateTime TimeStamp { get; set; }
        }

        private class WithNullableTimeStamp
        {
            public Guid StructureId { get; set; }
            public DateTime? TimeStamp { get; set; }
        }

        private class WithConcurrencyToken
        {
            public Guid StructureId { get; set; }
            public Guid ConcurrencyToken { get; set; }
        }

		private class WithStruct
		{
			public Guid StructureId { get; set; }

			public MyText Content { get; set; }
		}

		[Serializable]
		private struct MyText
		{
			private readonly string _value;

			public MyText(string value)
			{
				_value = value;
			}

			public static MyText Parse(string value)
			{
				return value == null ? null : new MyText(value);
			}

			public static implicit operator MyText(string value)
			{
				return new MyText(value);
			}

			public static implicit operator string(MyText item)
			{
				return item._value;
			}

			public override string ToString()
			{
				return _value;
			}
		}
    }
}