using NUnit.Framework;
using SisoDb.Sql2008;

namespace SisoDb.UnitTests.Providers.Caching
{
	[TestFixture]
	public class Sql2008CacheTests : CacheTestsBase
	{
		protected override ISisoDatabase OnCreateSisoDatabase()
		{
			return new Sql2008Database(ConnectionInfoFake.Object, DbProviderFactoryFake.Object);
		}

		protected override ISisoDatabase OnCreateSisoDatabaseWithCaching()
		{
			return new Sql2008Database(ConnectionInfoFake.Object, DbProviderFactoryFake.Object) { CacheProvider = CacheProviderFake.Object };
		}
	}
}