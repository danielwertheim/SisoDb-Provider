using SisoDb.Querying;
using SisoDb.SqlServer;
using SisoDb.Structures.IdGenerators;

namespace SisoDb.Azure
{
    public class SqlAzureProviderFactory : SqlServerProviderFactory
    {
        public SqlAzureProviderFactory() 
            : base(StorageProviders.SqlAzure,  new SqlAzureStatements()) { }

	    public override IDbQueryGenerator GetDbQueryGenerator()
	    {
	        return new SqlAzureQueryGenerator(SqlStatements, GetSqlExpressionBuilder());
	    }

        public override Structures.IStructureIdGenerator GetGuidStructureIdGenerator()
        {
            return new CombGuidStructureIdGenerator();
        }
    }
}