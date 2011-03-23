using SisoDb.Providers.DbSchema;

namespace SisoDb.Providers.SqlProvider
{
    public interface ISqlDatabase : ISisoDatabase
    {
        IDbSchemaManager DbSchemaManager { get; set; }
    }
}