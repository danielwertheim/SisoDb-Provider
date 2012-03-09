using NUnit.Framework;
using SisoDb.Sql2012;

namespace SisoDb.UnitTests.Providers.Caching
{
	[TestFixture]
	public class Sql2012CacheTests : CacheTestsBase
	{
		protected override ISisoDatabase OnCreateSisoDatabase()
		{
			return new Sql2012Database(ConnectionInfoFake.Object, DbProviderFactoryFake.Object);
		}

		protected override ISisoDatabase OnCreateSisoDatabaseWithCaching()
		{
			return new Sql2012Database(ConnectionInfoFake.Object, DbProviderFactoryFake.Object) { CacheProvider = CacheProviderFake.Object };
		}
	}
}