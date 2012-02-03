using System.Data.Common;
using SisoDb.Testing.Sql2008;

namespace SisoDb.Testing.Sql2012
{
	public class Sql2012TestDbUtils : Sql2008TestDbUtils
    {
        private readonly string _connectionString;
        private readonly DbProviderFactory _factory;

        public Sql2012TestDbUtils(string connectionString) : base(connectionString)
        {}
    }
}