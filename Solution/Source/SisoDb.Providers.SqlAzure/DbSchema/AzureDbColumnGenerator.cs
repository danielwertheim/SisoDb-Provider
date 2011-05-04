using SisoDb.Providers.DbSchema;

namespace SisoDb.Providers.SqlAzure.DbSchema
{
    public class AzureDbColumnGenerator : IDbColumnGenerator
    {
        public string ToSql(string name, string dbType)
        {
            return string.Format("[{0}] {1} null", name, dbType);
        }
    }
}