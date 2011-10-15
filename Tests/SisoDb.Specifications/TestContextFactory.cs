using System;
using NCore;
using SisoDb.Specifications.Sql2008;

namespace SisoDb.Specifications
{
    public static class TestContextFactory
    {
        public static ITestContext Create(StorageProviders storageProvider)
        {
            if (storageProvider == StorageProviders.Sql2008)
                return new Sql2008TestContext(LocalConstants.ConnectionStringNameForSql2008);

            throw new NotSupportedException("Provided StorageProvider '{0}' has no associated TestContext.".Inject(storageProvider));
        }

        public static ITestContext CreateTemp(StorageProviders storageProvider)
        {
            if (storageProvider == StorageProviders.Sql2008)
                return new Sql2008TestContext(LocalConstants.ConnectionStringNameForSql2008Temp);

            throw new NotSupportedException("Provided StorageProvider '{0}' has no associated Temp TestContext.".Inject(storageProvider));
        }
    }
}