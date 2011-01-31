using SisoDb.Providers.SqlProvider.DbSchema;

namespace SisoDb.Providers.AzureProvider.DbSchema
{
    internal class AzureDbColumnGenerator : ISqlDbColumnGenerator
    {
        public string ToSql(string name, string dbType)
        {
            return string.Format("[{0}] {1} null", name, dbType);
        }
    }
}