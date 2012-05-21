using System;
using Moq;
using NCore;
using NUnit.Framework;
using PineCone.Structures.Schemas;
using PineCone.Structures.Schemas.MemberAccessors;
using SisoDb.DbSchema;
using SisoDb.Resources;

namespace SisoDb.UnitTests.DbSchema
{
    [TestFixture]
    public class DbDataTypeTranslatorTests : UnitTestBase
    {
        private readonly DataTypeConverter _dataTypeConverter = new DataTypeConverter();
        private readonly DbDataTypeTranslator _translator = new DbDataTypeTranslator();
        
        [Test]
        public void ToSql_ForByteArray_ThrowsException()
        {
            var propertyFake = new Mock<IStructureProperty>();
            propertyFake.Setup(x => x.Path).Returns("Bytes");
            propertyFake.Setup(x => x.IsEnumerable).Returns(true);
            propertyFake.Setup(x => x.ElementDataType).Returns(typeof(byte));
            propertyFake.Setup(x => x.DataType).Returns(typeof(byte[]));
            propertyFake.Setup(x => x.IsRootMember).Returns(true);

            var iac = new IndexAccessor(propertyFake.Object, _dataTypeConverter.Convert(propertyFake.Object));

            var ex = Assert.Throws<SisoDbException>(() => _translator.ToDbType(iac.DataType));

            Assert.AreEqual(
                ExceptionMessages.DbDataTypeTranslator_UnsupportedType.Inject("Byte[]"),
                ex.Message);
        }

        [Test]
        public void ToSql_ForInt_ReturnsSqlColumnStringWithInt()
        {
            AssertSqlTranslationForType<int>("[int]");
        }

        [Test]
        public void ToSql_ForByte_ReturnsSqlColumnStringWithInt()
        {
            AssertSqlTranslationForType<byte>("[tinyint]");
        }

        [Test]
        public void ToSql_ForDateTime_ReturnsSqlColumnStringWithDateTime()
        {
            AssertSqlTranslationForType<DateTime>("[datetime]");
        }

        [Test]
        public void ToSql_ForLong_ReturnsSqlColumnStringWithBigInt()
        {
            AssertSqlTranslationForType<long>("[bigint]");
        }

        [Test]
        public void ToSql_ForShort_ReturnsSqlColumnStringWithSmallInt()
        {
            AssertSqlTranslationForType<short>("[smallint]");
        }

        [Test]
        public void ToSql_ForBool_ReturnsSqlColumnStringWithBit()
        {
            AssertSqlTranslationForType<bool>("[bit]");
        }

        [Test]
        public void ToSql_ForGuid_ReturnsSqlColumnStringWithUniqueIdentifier()
        {
            AssertSqlTranslationForType<Guid>("[uniqueidentifier]");
        }

        [Test]
        public void ToSql_ForDecimal_ReturnsSqlColumnStringWithDecimal_18_5()
        {
            AssertSqlTranslationForType<decimal>("[decimal](18,5)");
        }

        [Test]
        public void ToSql_ForDouble_ReturnsSqlColumnStringWithFloat()
        {
            AssertSqlTranslationForType<double>("[float]");
        }

        [Test]
        public void ToSql_ForSingle_ReturnsSqlColumnStringWithFloat()
        {
            AssertSqlTranslationForType<Single>("[float]");
        }

        [Test]
        public void ToSql_ForFloat_ReturnsSqlColumnStringWithFloat()
        {
            AssertSqlTranslationForType<float>("[float]");
        }

        [Test]
        public void ToSql_ForChar_ReturnsSqlColumnStringWithNChar_1()
        {
            AssertSqlTranslationForType<char>("[nchar](1)");
        }

        [Test]
        public void ToSql_ForMyEnumDummy_ReturnsSqlColumnStringWithInt()
        {
            AssertSqlTranslationForType<MyEnumDummy>("[int]");
        }

        private void AssertSqlTranslationForType<T>(string expectedDbType)
        {
            var indexAccessor = GetFakeIndexAccessorForType<T>(isUnique: false);
            var dbType = _translator.ToDbType(indexAccessor.DataType);

            Assert.AreEqual(expectedDbType, dbType, indexAccessor.DataType.Name);
        }

        private static IIndexAccessor GetFakeIndexAccessorForType<T>(bool isUnique)
        {
            var iacFake = new Mock<IIndexAccessor>();
            iacFake.Setup(x => x.DataType).Returns(typeof(T));
            iacFake.Setup(x => x.IsUnique).Returns(isUnique);

            return iacFake.Object;
        }

        private enum MyEnumDummy
        {
            A
        }
    }
}