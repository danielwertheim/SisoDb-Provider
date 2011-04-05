using System;
using Moq;
using NUnit.Framework;
using SisoDb.Core;
using SisoDb.Providers.SqlProvider.DbSchema;
using SisoDb.Resources;
using SisoDb.Structures.Schemas;
using SisoDb.Structures.Schemas.MemberAccessors;

namespace SisoDb.Tests.UnitTests.Providers.SqlProvider.DbSchema
{
    [TestFixture]
    public class SqlDbDataTypeTranslatorTests : UnitTestBase
    {
        private SqlDbDataTypeTranslator _translator;

        protected override void OnFixtureInitialize()
        {
            _translator = new SqlDbDataTypeTranslator();
        }

        [Test]
        public void ToSql_ForUnsupportedTypeOnIndexAccessor_ThrowsSisoDbException()
        {
            var iac = GetFakeIndexAccessorForType<TimeSpan>(isUnique: false);

            var ex = Assert.Throws<SisoDbException>(() => _translator.ToDbType(iac));

            Assert.AreEqual("The datatype 'TimeSpan' is not supported!", ex.Message);
        }

        [Test]
        public void ToSql_ForByteArray_ThrowsException()
        {
            var propertyFake = new Mock<IStructureProperty>();
            propertyFake.Setup(x => x.Path).Returns("Bytes");
            propertyFake.Setup(x => x.IsEnumerable).Returns(true);
            propertyFake.Setup(x => x.ElementType).Returns(typeof (byte));
            propertyFake.Setup(x => x.PropertyType).Returns(typeof (byte[]));

            var iac = new IndexAccessor(propertyFake.Object);

            var ex = Assert.Throws<SisoDbException>(() => _translator.ToDbType(iac));

            Assert.AreEqual(
                ExceptionMessages.SqlDbDataTypeTranslator_ByteArraysAreNotSupported.Inject("Bytes"),
                ex.Message);
        }

        [Test]
        public void ToSql_ForNonUniqueStringIndexAccessor_ReturnsSqlColumnStringWithNVarChar_Max()
        {
            var iac = GetFakeIndexAccessorForType<string>(isUnique: false);
            AssertSqlTranslation(iac, "[nvarchar](max)");
        }

        [Test]
        public void ToSql_ForUniqueStringIndexAccessor_ReturnsSqlColumnStringWithNVarChar_Max()
        {
            var iac = GetFakeIndexAccessorForType<string>(isUnique: true);
            AssertSqlTranslation(iac, "[nvarchar](max)");
        }

        [Test]
        public void ToSql_ForArrayOfInt_ReturnsSqlColumnStringWithNVarcar_Max()
        {
            AssertSqlTranslationForType<int[]>("[nvarchar](max)");
        }

        [Test]
        public void ToSql_ForArrayOfString_ReturnsSqlColumnStringWithNVarcar_Max()
        {
            AssertSqlTranslationForType<string[]>("[nvarchar](max)");
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
            var dbType = _translator.ToDbType(indexAccessor);

            Assert.AreEqual(expectedDbType, dbType, indexAccessor.DataType.Name);
        }

        private void AssertSqlTranslation(IIndexAccessor indexAccessor, string expectedDbType)
        {
            var dbType = _translator.ToDbType(indexAccessor);

            Assert.AreEqual(expectedDbType, dbType, indexAccessor.DataType.Name);
        }

        private static IIndexAccessor GetFakeIndexAccessorForType<T>(bool isUnique)
        {
            var iacFake = new Mock<IIndexAccessor>();
            iacFake.Setup(x => x.DataType).Returns(typeof (T));
            iacFake.Setup(x => x.IsUnique).Returns(isUnique);

            return iacFake.Object;
        }

        private enum MyEnumDummy
        {
            A
        }
    }
}