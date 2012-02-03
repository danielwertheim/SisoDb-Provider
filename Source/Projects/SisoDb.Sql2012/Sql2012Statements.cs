using SisoDb.Dac;

namespace SisoDb.Sql2012
{
	public class Sql2012Statements : SqlStatementsBase
    {
    	public Sql2012Statements()
			: base(typeof(Sql2012Statements).Assembly, "Resources.Sql2012Statements")
        {
        }
    }
}