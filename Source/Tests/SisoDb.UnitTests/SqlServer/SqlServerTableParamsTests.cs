using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.SqlServer.Server;
using NUnit.Framework;
using PineCone.Structures;
using SisoDb.NCore;
using SisoDb.Resources;
using SisoDb.SqlServer;

namespace SisoDb.UnitTests.SqlServer
{
    [TestFixture]
    public class SqlServerTableParamsTests : UnitTestBase
    {
        [Test]
        public void Create_WhenIntegers_ReturnsTableParamForIntegerNumbers()
        {
            var values = new object[] { 1, 2, 3 };
            var param = SqlServerTableParams.Create("myParam", typeof(int), DataTypeCode.IntegerNumber, values);

            AssertIsCorrectTableParam(param, "myParam", "SisoIntegers", values);
        }

        [Test]
        public void Create_WhenIntegersWhereOneIsNull_ReturnsTableParamForIntegerNumbers()
        {
            var values = new object[] { 1, null, 3 };
            var param = SqlServerTableParams.Create("myParam", typeof(int?), DataTypeCode.IntegerNumber, values);

            AssertIsCorrectTableParam(param, "myParam", "SisoIntegers", values);
        }

        [Test]
        public void Create_WhenLongs_ReturnsTableParamForIntegerNumbers()
        {
            var values = new object[] { 1L, 2L, 3L };
            var param = SqlServerTableParams.Create("myParam", typeof(long), DataTypeCode.IntegerNumber, values);

            AssertIsCorrectTableParam(param, "myParam", "SisoIntegers", values);
        }

        [Test]
        public void Create_WhenLongsWhereOneIsNull_ReturnsTableParamForIntegerNumbers()
        {
            var values = new object[] { 1L, null, 3L };
            var param = SqlServerTableParams.Create("myParam", typeof(long?), DataTypeCode.IntegerNumber, values);

            AssertIsCorrectTableParam(param, "myParam", "SisoIntegers", values);
        }

        [Test]
        public void Create_WhenBooleans_ReturnsTableParamForBooleans()
        {
            var values = new object[] { true, false, true };
            var param = SqlServerTableParams.Create("myParam", typeof(bool), DataTypeCode.Bool, values);

            AssertIsCorrectTableParam(param, "myParam", "SisoBooleans", values);
        }

        [Test]
        public void Create_WhenBooleansWhereOneIsNull_ReturnsTableParamForBooleans()
        {
            var values = new object[] { true, null, false };
            var param = SqlServerTableParams.Create("myParam", typeof(bool?), DataTypeCode.Bool, values);

            AssertIsCorrectTableParam(param, "myParam", "SisoBooleans", values);
        }

        [Test]
        public void Create_WhenDecimals_ReturnsTableParamForFractalNumbers()
        {
            var values = new object[] { 1.1M, 2.2M, 3.3M };
            var param = SqlServerTableParams.Create("myParam", typeof(decimal), DataTypeCode.FractalNumber, values);

            AssertIsCorrectTableParam(param, "myParam", "SisoFractals", values);
        }

        [Test]
        public void Create_WhenDecimalsWhereOneIsNull_ReturnsTableParamForFractalNumbers()
        {
            var values = new object[] { 1.1M, null, 3.3M };
            var param = SqlServerTableParams.Create("myParam", typeof(decimal?), DataTypeCode.FractalNumber, values);

            AssertIsCorrectTableParam(param, "myParam", "SisoFractals", values);
        }

        [Test]
        public void Create_WhenDoubles_ReturnsTableParamForFractalNumbers()
        {
            var values = new object[] { 1.1d, 2.2d, 3.3d };
            var param = SqlServerTableParams.Create("myParam", typeof(double), DataTypeCode.FractalNumber, values);

            AssertIsCorrectTableParam(param, "myParam", "SisoFractals", values);
        }

        [Test]
        public void Create_WhenDoublesWhereOneIsNull_ReturnsTableParamForFractalNumbers()
        {
            var values = new object[] { 1.1d, null, 3.3d };
            var param = SqlServerTableParams.Create("myParam", typeof(double?), DataTypeCode.FractalNumber, values);

            AssertIsCorrectTableParam(param, "myParam", "SisoFractals", values);
        }

        [Test]
        public void Create_WhenFloats_ReturnsTableParamForFractalNumbers()
        {
            var values = new object[] { 1.1f, 2.2f, 3.3f };
            var param = SqlServerTableParams.Create("myParam", typeof(float), DataTypeCode.FractalNumber, values);

            AssertIsCorrectTableParam(param, "myParam", "SisoFractals", values);
        }

        [Test]
        public void Create_WhenFloatsWhereOneIsNull_ReturnsTableParamForFractalNumbers()
        {
            var values = new object[] { 1.1f, null, 3.3f };
            var param = SqlServerTableParams.Create("myParam", typeof(float?), DataTypeCode.FractalNumber, values);

            AssertIsCorrectTableParam(param, "myParam", "SisoFractals", values);
        }

        [Test]
        public void Create_WhenSingles_ReturnsTableParamForFractalNumbers()
        {
            var values = new object[] { 1.1f, 2.2f, 3.3f };
            var param = SqlServerTableParams.Create("myParam", typeof(Single), DataTypeCode.FractalNumber, values);

            AssertIsCorrectTableParam(param, "myParam", "SisoFractals", values);
        }

        [Test]
        public void Create_WhenSinglesWhereOneIsNull_ReturnsTableParamForFractalNumbers()
        {
            var values = new object[] { 1.1f, null, 3.3f };
            var param = SqlServerTableParams.Create("myParam", typeof(Single?), DataTypeCode.FractalNumber, values);

            AssertIsCorrectTableParam(param, "myParam", "SisoFractals", values);
        }

        [Test]
        public void Create_WhenDates_ReturnsTableParamForDates()
        {
            var origo = new DateTime(2012, 1, 2, 3, 4, 5);
            var values = new object[] { origo.AddDays(-1), origo, origo.AddDays(1) };
            var param = SqlServerTableParams.Create("myParam", typeof(DateTime), DataTypeCode.DateTime, values);

            AssertIsCorrectTableParam(param, "myParam", "SisoDates", values);
        }

        [Test]
        public void Create_WhenDatesWhereOneIsNull_ReturnsTableParamForDates()
        {
            var origo = new DateTime(2012, 1, 2, 3, 4, 5);
            var values = new object[] { null, origo, origo.AddDays(1) };
            var param = SqlServerTableParams.Create("myParam", typeof(DateTime?), DataTypeCode.DateTime, values);

            AssertIsCorrectTableParam(param, "myParam", "SisoDates", values);
        }

        [Test]
        public void Create_WhenGuids_ReturnsTableParamForGuids()
        {
            var values = new object[] { Guid.Parse("46a56b21-6b2d-4d05-8187-ad2200402219"), Guid.Parse("b6748652-43d5-48a4-b474-23f74ca0fd6e"), Guid.Parse("644b1369-d03b-41e1-9205-73273c3d790d") };
            var param = SqlServerTableParams.Create("myParam", typeof(Guid), DataTypeCode.Guid, values);

            AssertIsCorrectTableParam(param, "myParam", "SisoGuids", values);
        }

        [Test]
        public void Create_WhenGuidsWhereOneIsNull_ReturnsTableParamForGuids()
        {
            var values = new object[] { Guid.Parse("46a56b21-6b2d-4d05-8187-ad2200402219"), Guid.Parse("b6748652-43d5-48a4-b474-23f74ca0fd6e"), Guid.Parse("644b1369-d03b-41e1-9205-73273c3d790d") };
            var param = SqlServerTableParams.Create("myParam", typeof(Guid?), DataTypeCode.Guid, values);

            AssertIsCorrectTableParam(param, "myParam", "SisoGuids", values);
        }

        [Test]
        public void Create_WhenEnums_ReturnsTableParamForStrings()
        {
            var values = new object[] { ConsoleColor.Black, ConsoleColor.White, ConsoleColor.Red };
            var param = SqlServerTableParams.Create("myParam", typeof(ConsoleColor), DataTypeCode.Enum, values);

            AssertIsCorrectTableParam(param, "myParam", "SisoStrings", values.Select(v => v.ToString()).ToArray());
        }

        [Test]
        public void Create_WhenEnumsWhereOneIsNull_ReturnsTableParamForStrings()
        {
            var values = new object[] { ConsoleColor.Black, null, ConsoleColor.Red };
            var param = SqlServerTableParams.Create("myParam", typeof(ConsoleColor?), DataTypeCode.Enum, values);

            AssertIsCorrectTableParam(param, "myParam", "SisoStrings", values.Select(v => v == null ? null : v.ToString()).ToArray());
        }

        [Test]
        public void Create_WhenStrings_ReturnsTableParamForStrings()
        {
            var values = new object[] { "Aplha", "Bravo", "Charlie" };
            var param = SqlServerTableParams.Create("myParam", typeof(string), DataTypeCode.String, values);

            AssertIsCorrectTableParam(param, "myParam", "SisoStrings", values);
        }

        [Test]
        public void Create_WhenStringsWhereOneIsNull_ReturnsTableParamForStrings()
        {
            var values = new object[] { "Aplha", null, "Charlie" };
            var param = SqlServerTableParams.Create("myParam", typeof(string), DataTypeCode.String, values);

            AssertIsCorrectTableParam(param, "myParam", "SisoStrings", values);
        }

        [Test]
        public void Create_WhenStringsWhereOneIsToLong_ReturnsTableParamForStrings()
        {
            var toLongValue = new string('a', SqlServerTableParams.MaxStringLength + 1);
            var values = new object[] { "Aplha", toLongValue, "Charlie" };

            var ex = Assert.Throws<SisoDbException>(() => SqlServerTableParams.Create("myParam", typeof(string), DataTypeCode.String, values));

            Assert.AreEqual(ExceptionMessages.SqlServerTableParams_ToLongString.Inject(SqlServerTableParams.MaxStringLength, toLongValue), ex.Message);
        }

        [Test]
        public void Create_WhenTexts_ReturnsTableParamForTexts()
        {
            var values = new object[] { "Aplha", "Bravo", "Charlie" };
            var param = SqlServerTableParams.Create("myParam", typeof(string), DataTypeCode.Text, values);

            AssertIsCorrectTableParam(param, "myParam", "SisoTexts", values);
        }

        [Test]
        public void Create_WhenTextsWhereOneIsNull_ReturnsTableParamForTexts()
        {
            var values = new object[] { "Aplha", null, "Charlie" };
            var param = SqlServerTableParams.Create("myParam", typeof(string), DataTypeCode.Text, values);

            AssertIsCorrectTableParam(param, "myParam", "SisoTexts", values);
        }

        [Test]
        public void Create_WhenTextsWhereOneIsToLong_ReturnsTableParamForStrings()
        {
            var toLongValue = new string('a', SqlServerTableParams.MaxTextLength + 1);
            var values = new object[] { "Aplha", toLongValue, "Charlie" };

            var ex = Assert.Throws<SisoDbException>(() => SqlServerTableParams.Create("myParam", typeof(string), DataTypeCode.Text, values));

            Assert.AreEqual(ExceptionMessages.SqlServerTableParams_ToLongString.Inject(SqlServerTableParams.MaxStringLength, toLongValue), ex.Message);
        }

        private void AssertIsCorrectTableParam(SqlParameter param, string expectedParamName, string expectedTypeName, object[] expectedValues)
        {
            Assert.AreEqual(expectedParamName, param.ParameterName);
            Assert.AreEqual(expectedTypeName, param.TypeName);
            Assert.AreEqual(SqlDbType.Structured, param.SqlDbType);

            var paramValue = (IEnumerable<SqlDataRecord>)param.Value;
            var values = paramValue.Select(p => p.IsDBNull(0) ? null : p.GetValue(0)).ToArray();
            Assert.AreEqual(expectedValues, values);
        }
    }
}