using SisoDb.Querying;
using SisoDb.SqlServer;

namespace SisoDb.Sql2005
{
    public class Sql2005ProviderFactory : SqlServerProviderFactory
    {
        public Sql2005ProviderFactory()
            : base(new Sql2005Statements())
        { }

        public override StorageProviders ProviderType
        {
            get { return StorageProviders.Sql2005; }
        }

        public override IDbQueryGenerator GetDbQueryGenerator()
        {
            return new Sql2005QueryGenerator(SqlStatements, GetSqlExpressionBuilder());
        }
    }
}