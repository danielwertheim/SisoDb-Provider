using SisoDb.Providers.DbSchema;

namespace SisoDb.Providers.Sql2008
{
    public interface ISqlDatabase : ISisoDatabase
    {
        IDbSchemaManager DbSchemaManager { get; set; }
    }
}