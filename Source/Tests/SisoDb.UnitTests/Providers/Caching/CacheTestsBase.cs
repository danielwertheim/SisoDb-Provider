using System;
using Moq;
using NUnit.Framework;
using SisoDb.Dac;
using SisoDb.DbSchema;

namespace SisoDb.UnitTests.Providers.Caching
{
	[TestFixture]
	public abstract class CacheTestsBase : UnitTestBase
	{
		protected Mock<ISisoConnectionInfo> ConnectionInfoFake;
		protected Mock<IDbProviderFactory> DbProviderFactoryFake;
		protected Mock<IDbSchemas> DbSchemaManagerFake;
		protected Mock<IServerClient> ServerClientFake;
        protected Mock<IDbClient> DbClientFake; 
		protected Mock<ICacheProvider> CacheProviderFake;
		protected Mock<ICache> FooCacheFake;
		protected Mock<ICache> BarCacheFake;
 
		protected override void OnTestInitialize()
		{
			base.OnTestInitialize();

			ConnectionInfoFake = new Mock<ISisoConnectionInfo>();
			DbSchemaManagerFake = new Mock<IDbSchemas>();
			ServerClientFake = new Mock<IServerClient>();
			DbClientFake = new Mock<IDbClient>();
			DbProviderFactoryFake = new Mock<IDbProviderFactory>();
		    DbProviderFactoryFake.Setup(f => f.GetSettings()).Returns(DbSettings.CreateDefault);
			DbProviderFactoryFake.Setup(f => f.GetDbSchemaManagerFor(It.IsAny<ISisoDatabase>())).Returns(DbSchemaManagerFake.Object);
			DbProviderFactoryFake.Setup(f => f.GetServerClient(ConnectionInfoFake.Object)).Returns(ServerClientFake.Object);
			DbProviderFactoryFake.Setup(f => f.GetTransactionalDbClient(ConnectionInfoFake.Object)).Returns(DbClientFake.Object);
			FooCacheFake = new Mock<ICache>();
			FooCacheFake.Setup(f => f.StructureType).Returns(typeof (Foo));
			BarCacheFake = new Mock<ICache>();
			BarCacheFake.Setup(f => f.StructureType).Returns(typeof (Bar));
			CacheProviderFake = new Mock<ICacheProvider>();
			CacheProviderFake.Setup(f => f.Handles(typeof(Foo))).Returns(true);
			CacheProviderFake.Setup(f => f.Handles(typeof(Bar))).Returns(true);
			CacheProviderFake.Setup(f => f[typeof(Foo)]).Returns(FooCacheFake.Object);
			CacheProviderFake.Setup(f => f[typeof(Bar)]).Returns(BarCacheFake.Object);
		}

		protected abstract ISisoDatabase OnCreateSisoDatabase();
		protected abstract ISisoDatabase OnCreateSisoDatabaseWithCaching();

		[Test]
		public void SisoDatabase_CachingIsEnabled_WithOutCacheProvider_ReturnsFalse()
		{
			var db = OnCreateSisoDatabase();

			Assert.IsFalse(db.CachingIsEnabled);
		}

		[Test]
		public void SisoDatabase_CachingIsEnabled_WhenCacheProviderIsAssigned_ReturnsTrue()
		{
			var db = OnCreateSisoDatabaseWithCaching();

			Assert.IsTrue(db.CachingIsEnabled);
		}

		[Test]
		public void SisoDatabase_EnsureNewDatabase_WithOutCacheProvider_DoesNotInvokesClearOnCache()
		{
			var db = OnCreateSisoDatabase();

			db.EnsureNewDatabase();

			CacheProviderFake.Verify(f => f.Clear(), Times.Never());
		}

		[Test]
		public void SisoDatabase_EnsureNewDatabase_WithCacheProvider_InvokesClearOnCache()
		{
			var db = OnCreateSisoDatabaseWithCaching();
			
			db.EnsureNewDatabase();

			CacheProviderFake.Verify(f => f.Clear(), Times.Exactly(1));
		}

		[Test]
		public void SisoDatabase_CreateIfNotExists_WithOutCacheProvider_DoesNotInvokesClearOnCache()
		{
			var db = OnCreateSisoDatabase();

			db.CreateIfNotExists();

			CacheProviderFake.Verify(f => f.Clear(), Times.Never());
		}

		[Test]
		public void SisoDatabase_CreateIfNotExists_WithCacheProvider_InvokesClearOnCache()
		{
			var db = OnCreateSisoDatabaseWithCaching();

			db.CreateIfNotExists();

			CacheProviderFake.Verify(f => f.Clear(), Times.Exactly(1));
		}

		[Test]
		public void SisoDatabase_InitializeExisting_WithOutCacheProvider_DoesNotInvokesClearOnCache()
		{
			var db = OnCreateSisoDatabase();

			db.InitializeExisting();

			CacheProviderFake.Verify(f => f.Clear(), Times.Never());
		}

		[Test]
		public void SisoDatabase_InitializeExisting_WithCacheProvider_InvokesClearOnCache()
		{
			var db = OnCreateSisoDatabaseWithCaching();

			db.InitializeExisting();

			CacheProviderFake.Verify(f => f.Clear(), Times.Exactly(1));
		}

		[Test]
		public void SisoDatabase_DeleteIfExists_WithOutCacheProvider_DoesNotInvokesClearOnCache()
		{
			var db = OnCreateSisoDatabase();

			db.DeleteIfExists();

			CacheProviderFake.Verify(f => f.Clear(), Times.Never());
		}

		[Test]
		public void SisoDatabase_DeleteIfExists_WithCacheProvider_InvokesClearOnCache()
		{
			var db = OnCreateSisoDatabaseWithCaching();

			db.DeleteIfExists();

			CacheProviderFake.Verify(f => f.Clear(), Times.Exactly(1));
		}

		[Test]
		public void SisoDatabase_DropStructureSetOfT_WithOutCacheProvider_DoesNotInvokesClearOnCache()
		{
			var db = OnCreateSisoDatabase();

			db.DropStructureSet<Foo>();

			FooCacheFake.Verify(f => f.Clear(), Times.Never());
		}

		[Test]
		public void SisoDatabase_DropStructureSetOfT_WithCacheProvider_InvokesClearOnCache()
		{
			var db = OnCreateSisoDatabaseWithCaching();

			db.DropStructureSet<Foo>();

			FooCacheFake.Verify(f => f.Clear(), Times.Exactly(1));
		}

		[Test]
		public void SisoDatabase_DropStructureSetOfT_WithCacheProviderThatDoesNotHandleType_DoesNotInvokeClearOnCache()
		{
			var db = OnCreateSisoDatabaseWithCaching();
			CacheProviderFake.Setup(f => f.Handles(typeof (Foo))).Returns(false);

			db.DropStructureSet<Foo>();

			FooCacheFake.Verify(f => f.Clear(), Times.Never());
		}

		[Test]
		public void SisoDatabase_DropStructureSet_WithOutCacheProvider_DoesNotInvokesClearOnCache()
		{
			var db = OnCreateSisoDatabase();

			db.DropStructureSet(typeof(Foo));

			FooCacheFake.Verify(f => f.Clear(), Times.Never());
		}

		[Test]
		public void SisoDatabase_DropStructureSet_WithCacheProvider_InvokesClearOnCache()
		{
			var db = OnCreateSisoDatabaseWithCaching();

			db.DropStructureSet(typeof(Foo));

			FooCacheFake.Verify(f => f.Clear(), Times.Exactly(1));
		}

		[Test]
		public void SisoDatabase_DropStructureSet_WithCacheProviderThatDoesNotHandleType_DoesNotInvokeClearOnCache()
		{
			var db = OnCreateSisoDatabaseWithCaching();
			CacheProviderFake.Setup(f => f.Handles(typeof(Foo))).Returns(false);

			db.DropStructureSet(typeof(Foo));

			FooCacheFake.Verify(f => f.Clear(), Times.Never());
		}

		[Test]
		public void SisoDatabase_DropStructureSets_WithOutCacheProvider_DoesNotInvokesClearOnCache()
		{
			var db = OnCreateSisoDatabase();

			db.DropStructureSets(new[]{typeof(Foo), typeof(Bar)});

			FooCacheFake.Verify(f => f.Clear(), Times.Never());
			BarCacheFake.Verify(f => f.Clear(), Times.Never());
		}

		[Test]
		public void SisoDatabase_DropStructureSets_WithCacheProvider_InvokesClearOnCache()
		{
			var db = OnCreateSisoDatabaseWithCaching();

			db.DropStructureSets(new[] { typeof(Foo), typeof(Bar) });

			FooCacheFake.Verify(f => f.Clear(), Times.Exactly(1));
			BarCacheFake.Verify(f => f.Clear(), Times.Exactly(1));
		}

		[Test]
		public void SisoDatabase_DropStructureSets_WithCacheProviderThatDoesNotHandleType_DoesNotInvokeClearOnCache()
		{
			var db = OnCreateSisoDatabaseWithCaching();
			CacheProviderFake.Setup(f => f.Handles(typeof(Foo))).Returns(false);
			CacheProviderFake.Setup(f => f.Handles(typeof(Bar))).Returns(false);

			db.DropStructureSets(new[] { typeof(Foo), typeof(Bar) });

			FooCacheFake.Verify(f => f.Clear(), Times.Never());
			BarCacheFake.Verify(f => f.Clear(), Times.Never());
		}

		[Test]
		public void SisoDatabase_UpsertStructureSetOfT_WithOutCacheProvider_DoesNotInvokeClearOnCache()
		{
			var db = OnCreateSisoDatabase();

			db.UpsertStructureSet<Foo>();

			FooCacheFake.Verify(f => f.Clear(), Times.Never());
		}

		[Test]
		public void SisoDatabase_UpsertStructureSet_WithOutCacheProvider_DoesNotInvokeClearOnCache()
		{
			var db = OnCreateSisoDatabase();

			db.UpsertStructureSet(typeof(Foo));

			FooCacheFake.Verify(f => f.Clear(), Times.Never());
		}

		[Test]
		public void SisoDatabase_UpsertStructureSetOfT_WithCacheProviderThatDoesNotHandleType_DoesNotInvokeClearOnCache()
		{
			var db = OnCreateSisoDatabaseWithCaching();
			CacheProviderFake.Setup(f => f.Handles(typeof(Foo))).Returns(false);

			db.UpsertStructureSet<Foo>();

			FooCacheFake.Verify(f => f.Clear(), Times.Never());
		}

		[Test]
		public void SisoDatabase_UpsertStructureSet_WithCacheProviderThatDoesNotHandleType_DoesNotInvokeClearOnCache()
		{
			var db = OnCreateSisoDatabaseWithCaching();
			CacheProviderFake.Setup(f => f.Handles(typeof(Foo))).Returns(false);

			db.UpsertStructureSet(typeof(Foo));

			FooCacheFake.Verify(f => f.Clear(), Times.Never());
		}

		private class Foo
		{
			public Guid Id { get; set; }
		}

		private class Bar
		{
			public Guid Id { get; set; } 
		}
	}
}