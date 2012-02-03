using NUnit.Framework;
using SisoDb.SqlCe4;

namespace SisoDb.UnitTests.Providers.Caching
{
	[TestFixture]
	public class SqlCe4CacheTests : CacheTestsBase
	{
		protected override ISisoDatabase OnCreateSisoDatabase()
		{
			return new SqlCe4Database(ConnectionInfoFake.Object, DbProviderFactoryFake.Object);
		}

		protected override ISisoDatabase OnCreateSisoDatabaseWithCaching()
		{
			return new SqlCe4Database(ConnectionInfoFake.Object, DbProviderFactoryFake.Object) { CacheProvider = CacheProviderFake.Object };
		}
	}
}