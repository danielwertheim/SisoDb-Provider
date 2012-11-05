using System;
using Moq;
using NUnit.Framework;
using SisoDb.Structures;
using SisoDb.Structures.Schemas;

namespace SisoDb.UnitTests.Structures.Schemas
{
	[TestFixture]
	public class DataTypeConverterTests : UnitTestBase
	{
	    private IDataTypeConverter _converter;

        protected override void OnFixtureInitialize()
        {
            _converter = new DataTypeConverter();
        }

        private IStructureProperty CreateProperty(Type type, string name = null)
        {
            var property = new Mock<IStructureProperty>();
            property.SetupGet(f => f.Name).Returns(name ?? "Foo");
            property.SetupGet(f => f.DataType).Returns(type);

            return property.Object;
        }

        [Test]
        [TestCase(typeof(ushort))]
        [TestCase(typeof(ushort?))]
        [TestCase(typeof(uint))]
        [TestCase(typeof(uint?))]
        [TestCase(typeof(ulong))]
        [TestCase(typeof(ulong?))]
        public void Convert_TypeIsUnsignedIntegerFamily_ReturnsUnsignedIntegerNumber(Type type)
        {
            Assert.AreEqual(DataTypeCode.UnsignedIntegerNumber, _converter.Convert(CreateProperty(type)));
        }

		[Test]
        [TestCase(typeof(short))]
        [TestCase(typeof(short?))]
		[TestCase(typeof(int))]
		[TestCase(typeof(int?))]
        [TestCase(typeof(long))]
        [TestCase(typeof(long?))]
		public void Convert_TypeIsIntegerFamily_ReturnsIntegerNumber(Type type)
		{
			Assert.AreEqual(DataTypeCode.IntegerNumber, _converter.Convert(CreateProperty(type)));
		}

		[Test]
		[TestCase(typeof(Single))]
		[TestCase(typeof(Single?))]
		[TestCase(typeof(double))]
		[TestCase(typeof(double?))]
		[TestCase(typeof(decimal))]
		[TestCase(typeof(decimal?))]
		[TestCase(typeof(float))]
		[TestCase(typeof(float?))]
		public void Convert_TypeIsFractalFamily_ReturnsFractalNumber(Type type)
		{
			Assert.AreEqual(DataTypeCode.FractalNumber, _converter.Convert(CreateProperty(type)));
		}

		[Test]
		[TestCase(typeof(bool))]
		[TestCase(typeof(bool?))]
		public void Convert_TypeIsBool_ReturnsBool(Type type)
		{
			Assert.AreEqual(DataTypeCode.Bool, _converter.Convert(CreateProperty(type)));
		}

		[Test]
		[TestCase(typeof(DateTime))]
		[TestCase(typeof(DateTime?))]
		public void Convert_TypeIsDateTime_ReturnsDateTime(Type type)
		{
			Assert.AreEqual(DataTypeCode.DateTime, _converter.Convert(CreateProperty(type)));
		}

		[Test]
		[TestCase(typeof(Guid))]
		[TestCase(typeof(Guid?))]
		public void Convert_TypeIsGuid_ReturnsGuid(Type type)
		{
			Assert.AreEqual(DataTypeCode.Guid, _converter.Convert(CreateProperty(type)));
		}

		[Test]
		[TestCase(typeof(string))]
		public void Convert_TypeIsString_ReturnsString(Type type)
		{
			Assert.AreEqual(DataTypeCode.String, _converter.Convert(CreateProperty(type)));
		}

        [Test]
        [TestCase(typeof(string), "MyText")]
        [TestCase(typeof(string), "MyDescription")]
        [TestCase(typeof(string), "MyContent")]
        public void Convert_When_typeIsString_but_name_should_by_convention_be_converted_to_Text_Then_it_should_return_Text(Type type, string name)
        {
            Assert.AreEqual(DataTypeCode.Text, _converter.Convert(CreateProperty(type, name)));
        }

		[Test]
		[TestCase(typeof(DataTypeCode))]
		[TestCase(typeof(DataTypeCode?))]
		public void Convert_TypeIsEnum_ReturnsEnum(Type type)
		{
			Assert.AreEqual(DataTypeCode.Enum, _converter.Convert(CreateProperty(type)));
		}
	}
}