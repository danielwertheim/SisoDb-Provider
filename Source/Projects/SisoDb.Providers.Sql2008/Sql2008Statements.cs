using System.Reflection;
using SisoDb.Providers;

namespace SisoDb.Sql2008
{
    internal class Sql2008Statements : SqlStatementsBase
    {
		private static readonly ISqlStatements SingletonInstance;

    	public static ISqlStatements Instance
    	{
			get { return SingletonInstance; }
    	}

    	static Sql2008Statements()
        {
			SingletonInstance = new Sql2008Statements(typeof(Sql2008Statements).Assembly);
        }

		private Sql2008Statements(Assembly resourceAssembly)
            : base(resourceAssembly, "Resources.Sql2008Statements")
        {
        }
    }
}