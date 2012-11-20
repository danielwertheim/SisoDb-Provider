using System;
using Moq;
using NUnit.Framework;
using SisoDb.Dac;
using SisoDb.NCore;
using SisoDb.Querying.Sql;
using SisoDb.Structures;

namespace SisoDb.UnitTests.Querying
{
    [TestFixture]
    public class DbQueryChecksumTests : UnitTestBase
    {
        private DbQueryChecksumGenerator _checksumGenerator;

        protected override void OnFixtureInitialize()
        {
            _checksumGenerator = new DbQueryChecksumGenerator();
        }

        [Test]
        public void Checksum_When_empty_query_Returns_Null()
        {
            var q = DbQueries.Empty();

            var c = _checksumGenerator.Generate(q);

            Assert.That(c, Is.EqualTo(null));
        }

        [Test]
        public void Checksum_When_query_has_sql_but_no_params_Returns_checksum()
        {
            var q = DbQueries.Sql();

            var c = _checksumGenerator.Generate(q);

            Assert.That(c, Is.EqualTo(ChecksumsFor.Sql));
        }

        [Test]
        public void Checksum_When_query_has_sql_and_params_Returns_checksum_for_both_sql_and_params()
        {
            var q = DbQueries.SqlAndSimpleDacParams();

            var c = _checksumGenerator.Generate(q);

            Assert.That(c, Is.EqualTo(ChecksumsFor.SqlAndSimpleDacParams));
        }

        [Test]
        public void Checksum_When_query_has_sql_and_array_params_Returns_checksum_for_both_sql_and_params()
        {
            var q = DbQueries.SqlAndArrayDacParams();

            var c = _checksumGenerator.Generate(q);

            Assert.That(c, Is.EqualTo(ChecksumsFor.SqlAndArrayDacParams));
        }

        [Test]
        public void Checksum_When_query_has_sql_and_both_simple_and_array_params_Returns_checksum_for_both_sql_and_params()
        {
            var q = DbQueries.SqlAndSimpleAndArrayDacParams();

            var c = _checksumGenerator.Generate(q);

            Assert.That(c, Is.EqualTo(ChecksumsFor.SqlAndSimpleAndArrayDacParams));
        }

        private static class ChecksumsFor
        {
            public const string Sql = "10872599";
            public const string SqlAndSimpleDacParams = "dc383ab4";
            public const string SqlAndArrayDacParams = "36f4d0d1";
            public const string SqlAndSimpleAndArrayDacParams = "f46085e1";
        }

        private static class DbQueries
        {
            public static IDbQuery Empty()
            {
                var fake = new Mock<IDbQuery>();
                fake.SetupGet(f => f.IsEmpty).Returns(true);

                return fake.Object;
            }

            public static IDbQuery Sql()
            {
                var fake = new Mock<IDbQuery>();
                fake.SetupGet(f => f.IsEmpty).Returns(false);
                fake.SetupGet(f => f.Sql).Returns("select * from foo;");

                return fake.Object;
            }

            public static IDbQuery SqlAndSimpleDacParams()
            {
                var fake = new Mock<IDbQuery>();
                fake.SetupGet(f => f.IsEmpty).Returns(false);
                fake.SetupGet(f => f.Sql).Returns("select * from foo where nullv in(@pNull) and stringv=@pString and intv=@pInt and doublev=@pDouble and decimalv=@pDecimal and boolv=@pBool and datetimev=@pDateTime and guidv=@pGuid;");
                fake.SetupGet(f => f.Parameters).Returns(() => new IDacParameter[]
                {
                    new DacParameter("pNull", null),
                    new DacParameter("pString", "MyValue"),
                    new DacParameter("pInt", 42),
                    new DacParameter("pDouble", 33.3d),
                    new DacParameter("pDecimal", 33.3m),
                    new DacParameter("pBool", true),
                    new DacParameter("pDateTime", SysDateTime.Now),
                    new DacParameter("pGuid", Guid.Parse("d65010a6-e063-433a-8e0e-7a22b23ed311"))
                });
                return fake.Object;
            }

            public static IDbQuery SqlAndArrayDacParams()
            {
                var fake = new Mock<IDbQuery>();
                fake.SetupGet(f => f.IsEmpty).Returns(false);
                fake.SetupGet(f => f.Sql).Returns("select * from foo where stringv in(@pString) and intv in(@pInt) and doublev in(@pDouble) and decimalv in(@pDecimal) and boolv in(@pBool) and datetimev in(@pDateTime) and guidv in(@pGuid);");
                fake.SetupGet(f => f.Parameters).Returns(() => new IDacParameter[]
                {
                    new ArrayDacParameter("pString", typeof(string), DataTypeCode.String, new object[] {"MyValue"}),
                    new ArrayDacParameter("pInt", typeof(int), DataTypeCode.IntegerNumber,  new object[] {42}),
                    new ArrayDacParameter("pDouble", typeof(double), DataTypeCode.FractalNumber,  new object[] {33.3d}),
                    new ArrayDacParameter("pDecimal", typeof(decimal), DataTypeCode.FractalNumber,  new object[] {33.3m}),
                    new ArrayDacParameter("pBool", typeof(bool), DataTypeCode.Bool,  new object[] {true}),
                    new ArrayDacParameter("pDateTime", typeof(DateTime), DataTypeCode.DateTime,  new object[] {SysDateTime.Now}),
                    new ArrayDacParameter("pGuid", typeof(Guid), DataTypeCode.Guid,  new object[] {Guid.Parse("d65010a6-e063-433a-8e0e-7a22b23ed311")})
                });
                return fake.Object;
            }

            public static IDbQuery SqlAndSimpleAndArrayDacParams()
            {
                var fake = new Mock<IDbQuery>();
                fake.SetupGet(f => f.IsEmpty).Returns(false);
                fake.SetupGet(f => f.Sql).Returns("select * from foo where stringv in(@pString) and intv=@pInt;");
                fake.SetupGet(f => f.Parameters).Returns(() => new IDacParameter[]
                {
                    new ArrayDacParameter("pString", typeof(string), DataTypeCode.String, new object[] {"MyValue"}),
                    new DacParameter("pInt", 42)
                });
                return fake.Object;
            }
        }
    }
}