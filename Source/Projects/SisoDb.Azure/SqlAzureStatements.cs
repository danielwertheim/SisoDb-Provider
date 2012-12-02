using SisoDb.Dac;

namespace SisoDb.Azure
{
	public class SqlAzureStatements : SqlStatementsBase
    {
    	public SqlAzureStatements()
			: base(typeof(SqlAzureStatements).Assembly, "Resources.SqlAzureStatements")
        {
        }
    }
}