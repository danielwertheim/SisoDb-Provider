using SisoDb.Dac;

namespace SisoDb.SqlCe4
{
	public class SqlCe4Statements : SqlStatementsBase
    {
		public SqlCe4Statements()
			: base(typeof(SqlCe4Statements).Assembly, "Resources.SqlCe4Statements")
        {
        }
    }
}