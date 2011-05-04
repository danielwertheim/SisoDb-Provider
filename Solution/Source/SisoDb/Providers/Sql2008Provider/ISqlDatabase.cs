using SisoDb.Providers.DbSchema;

namespace SisoDb.Providers.Sql2008Provider
{
    public interface ISqlDatabase : ISisoDatabase
    {
        IDbSchemaManager DbSchemaManager { get; set; }
    }
}