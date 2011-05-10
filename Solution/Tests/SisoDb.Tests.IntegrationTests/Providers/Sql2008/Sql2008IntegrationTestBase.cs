using NUnit.Framework;
using SisoDb.Core;
using SisoDb.Providers;
using SisoDb.Providers.Sql2008;

namespace SisoDb.Tests.IntegrationTests.Providers.Sql2008
{
    [TestFixture]
    public abstract class Sql2008IntegrationTestBase : IntegrationTestBase<Sql2008Database>
    {
        protected Sql2008DbUtils DbHelper { get; set; }

        protected Sql2008IntegrationTestBase()
            : base(new Sql2008DbFactory(), LocalConstants.ConnectionStringNameForSql2008)
        {
            DbHelper = new Sql2008DbUtils(Database.ConnectionInfo.ConnectionString.PlainString);
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