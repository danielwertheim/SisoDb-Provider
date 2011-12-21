using System.Reflection;
using SisoDb.Providers;

namespace SisoDb.SqlCe4
{
    internal class SqlCe4Statements : SqlStatementsBase
    {
        private static readonly ISqlStatements SingletonInstance;

    	public static ISqlStatements Instance
    	{
			get { return SingletonInstance; }
    	}

    	static SqlCe4Statements()
        {
			SingletonInstance = new SqlCe4Statements(typeof(SqlCe4Statements).Assembly);
        }

    	private SqlCe4Statements(Assembly resourceAssembly)
            : base(resourceAssembly, "Resources.SqlCe4Statements")
        {
        }
    }
}