using SisoDb.Providers.SqlProvider.DbSchema;

namespace SisoDb.Providers.SqlProvider
{
    public interface ISqlDatabase : ISisoDatabase
    {
        IDbSchemaManager DbSchemaManager { get; set; }

        IIdentityGenerator IdentityGenerator { get; set; }
    }
}