using NUnit.Framework;
using SisoDb.Core;
using SisoDb.Providers;
using SisoDb.SqlCe4;

namespace SisoDb.Tests.IntegrationTests.SqlCe4
{
    [TestFixture]
    public abstract class SqlCe4IntegrationTestBase : IntegrationTestBase<SqlCe4Database>
    {
        protected SqlCe4DbUtils DbHelper { get; set; }

        protected SqlCe4IntegrationTestBase()
            : base(new SqlCe4DbFactory(), LocalConstants.ConnectionStringNameForSqlCe4)
        {
            DbHelper = new SqlCe4DbUtils(Database.ConnectionInfo.ConnectionString.PlainString);
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