using SisoDb.Querying;
using SisoDb.SqlServer;

namespace SisoDb.Sql2012
{
    public class Sql2012ProviderFactory : SqlServerProviderFactory
    {
        public Sql2012ProviderFactory() : base(new Sql2012Statements())
        { }

        public override StorageProviders ProviderType
        {
            get { return StorageProviders.Sql2012; }
        }

	    public override IDbQueryGenerator GetDbQueryGenerator()
	    {
	        return new Sql2012QueryGenerator(SqlStatements, GetSqlExpressionBuilder());
	    }
    }
}