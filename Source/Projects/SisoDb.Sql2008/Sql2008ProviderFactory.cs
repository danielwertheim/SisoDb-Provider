using SisoDb.Querying;
using SisoDb.SqlServer;

namespace SisoDb.Sql2008
{
    public class Sql2008ProviderFactory : SqlServerProviderFactory
    {
        public Sql2008ProviderFactory()
            : base(new Sql2008Statements())
        { }

        public override StorageProviders ProviderType
        {
            get { return StorageProviders.Sql2008; }
        }

        public override IDbQueryGenerator GetDbQueryGenerator()
        {
            return new Sql2008QueryGenerator(SqlStatements, GetSqlExpressionBuilder());
        }
    }
}