using SisoDb.Dac;
using SisoDb.Querying;
using SisoDb.Querying.Sql;
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

        public override IServerClient GetServerClient(ISisoConnectionInfo connectionInfo)
        {
            return new Sql2005ServerClient(GetAdoDriver(), connectionInfo, ConnectionManager, SqlStatements);
        }

        public override IAdoDriver GetAdoDriver()
        {
            return new Sql2005AdoDriver();
        }

        public override IDbQueryGenerator GetDbQueryGenerator()
        {
            return new Sql2005QueryGenerator(SqlStatements, GetSqlExpressionBuilder());
        }

        public override ISqlWhereCriteriaBuilder GetWhereCriteriaBuilder()
        {
            return new Sql2005WhereCriteriaBuilder();
        }
    }
}