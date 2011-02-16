using SisoDb.Providers.Shared.DbSchema;

namespace SisoDb.Providers.AzureProvider.DbSchema
{
    public class AzureDbColumnGenerator : IDbColumnGenerator
    {
        public string ToSql(string name, string dbType)
        {
            return string.Format("[{0}] {1} null", name, dbType);
        }
    }
}