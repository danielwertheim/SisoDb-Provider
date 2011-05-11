﻿using NUnit.Framework;
using SisoDb.Providers.Sql2008;
using SisoDb.Providers.Sql2008.Dac;

namespace SisoDb.Tests.IntegrationTests.Providers.Sql2008.Dac
{
    [TestFixture]
    public class Sql2008ServerClientTests : Sql2008TembDbIntegrationTestBase
    {
        protected override string TempDbName
        {
            get { return "DAD8CF7872044B4795C4034F5DF37C6E"; }
        }

        [Test]
        public void ForServerConnection_WhenServerExists_ConnectionIsAgainstMaster()
        {
            using (var serverClient = new Sql2008ServerClient((Sql2008ConnectionInfo)Database.ConnectionInfo))
            {
                Assert.AreEqual("Data Source=.;Initial Catalog=;Integrated Security=True", serverClient.ConnectionString.PlainString);
            }
        }
    }
}