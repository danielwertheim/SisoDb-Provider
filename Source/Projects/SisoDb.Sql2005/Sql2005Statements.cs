using SisoDb.Dac;

namespace SisoDb.Sql2005
{
	public class Sql2005Statements : SqlStatementsBase
    {
    	public Sql2005Statements()
			: base(typeof(Sql2005Statements).Assembly, "Resources.Sql2005Statements")
        {
        }
    }
}