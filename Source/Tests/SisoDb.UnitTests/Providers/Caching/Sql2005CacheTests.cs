using NUnit.Framework;
using SisoDb.Sql2005;

namespace SisoDb.UnitTests.Providers.Caching
{
	[TestFixture]
	public class Sql2005CacheTests : CacheTestsBase
	{
		protected override ISisoDatabase OnCreateSisoDatabase()
		{
			return new Sql2005Database(ConnectionInfoFake.Object, DbProviderFactoryFake.Object);
		}

		protected override ISisoDatabase OnCreateSisoDatabaseWithCaching()
		{
			return new Sql2005Database(ConnectionInfoFake.Object, DbProviderFactoryFake.Object) { CacheProvider = CacheProviderFake.Object };
		}
	}
}