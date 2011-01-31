using SisoDb.Providers.SqlProvider.DbSchema;

namespace SisoDb.Providers.SqlProvider
{
    internal interface ISqlDatabase : ISisoDatabase
    {
        IDbSchemaManager DbSchemaManager { get; }

        IIdentityGenerator IdentityGenerator { get; }
    }
}