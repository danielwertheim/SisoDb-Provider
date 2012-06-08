using System;
using SisoDb.Testing;
using SisoDb.Testing.Sql2005;
using SisoDb.Testing.Sql2008;
using SisoDb.Testing.Sql2012;
using SisoDb.Testing.SqlCe4;
using SisoDb.Testing.SqlProfiler;

namespace SisoDb.Specifications
{
    public static class TestContextFactory
    {
        public static ITestContext Create()
        {
#if Sql2005Provider
            return new Sql2005TestContext(TestConstants.ConnectionStringNameForSql2005);
#endif
#if Sql2008Provider
            return new Sql2008TestContext(TestConstants.ConnectionStringNameForSql2008);
#endif
#if Sql2012Provider
            return new Sql2012TestContext(TestConstants.ConnectionStringNameForSql2012);
#endif
#if SqlCe4Provider
            return new SqlCe4TestContext(TestConstants.ConnectionStringNameForSqlCe4);
#endif
#if SqlProfilerProvider
            return new SqlProfilerTestContext(TestConstants.ConnectionStringNameForSqlProfiler);
#endif
			throw new NotSupportedException("No provider has been specified for the test context.");
        }

        public static ITestContext CreateTemp()
        {
#if Sql2005Provider
            return new Sql2005TestContext(TestConstants.ConnectionStringNameForSql2005Temp);
#endif
#if Sql2008Provider
            return new Sql2008TestContext(TestConstants.ConnectionStringNameForSql2008Temp);
#endif
#if Sql2012Provider
            return new Sql2012TestContext(TestConstants.ConnectionStringNameForSql2012Temp);
#endif
#if SqlCe4Provider
            return new SqlCe4TestContext(TestConstants.ConnectionStringNameForSqlCe4Temp);
#endif
#if SqlProfilerProvider
            return new SqlProfilerTestContext(TestConstants.ConnectionStringNameForSqlProfilerTemp);
#endif
            throw new NotSupportedException("No provider has been specified for the test context.");
        }
    }
}