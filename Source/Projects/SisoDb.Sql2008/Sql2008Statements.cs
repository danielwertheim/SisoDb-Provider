using SisoDb.Dac;

namespace SisoDb.Sql2008
{
	public class Sql2008Statements : SqlStatementsBase
    {
    	public Sql2008Statements()
			: base(typeof(Sql2008Statements).Assembly, "Resources.Sql2008Statements")
        {
        }
    }
}