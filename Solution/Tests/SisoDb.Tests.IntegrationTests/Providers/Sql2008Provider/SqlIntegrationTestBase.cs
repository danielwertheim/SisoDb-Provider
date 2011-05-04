using NUnit.Framework;
using SisoDb.Core;
using SisoDb.Providers.Sql2008Provider;

namespace SisoDb.Tests.IntegrationTests.Providers.Sql2008Provider
{
    [TestFixture]
    public abstract class SqlIntegrationTestBase : IntegrationTestBase
    {
        protected DbHelper DbHelper { get; private set; }

        protected SqlIntegrationTestBase() : base(LocalConstants.ConnectionStringNameForSql2008)
        {
            DbHelper = new DbHelper(Database.ConnectionInfo.ConnectionString.PlainString);
        }

        protected string GetStructureTableName<T>() where T : class
        {
            return Database.StructureSchemas.GetSchema(TypeFor<T>.Type).GetStructureTableName();
        }

        protected string GetIndexesTableName<T>() where T : class
        {
            return Database.StructureSchemas.GetSchema(TypeFor<T>.Type).GetIndexesTableName();
        }

        protected string GetUniquesTableName<T>() where T : class
        {
            return Database.StructureSchemas.GetSchema(TypeFor<T>.Type).GetUniquesTableName();
        }
    }
}