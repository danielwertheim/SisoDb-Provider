﻿using SisoDb.Sql2012;
using SisoDb.MiniProfiler;

namespace SisoDb.Testing.SqlProfiler
{
    public class SqlProfilerTestContext : TestContextBase
    {
        public SqlProfilerTestContext(string connectionStringName)
            : base(new Sql2012DbFactory(), new Sql2012ConnectionInfo(connectionStringName), new Sql2012ProviderFactory())
        {
            DbHelper = new SqlProfilerTestDbUtils(Database.ConnectionInfo.ClientConnectionString.PlainString);
            Database.ActivateProfiler();
        }
    }
}