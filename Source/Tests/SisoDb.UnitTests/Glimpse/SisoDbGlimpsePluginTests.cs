using System;
using System.Web;
using Moq;
using NUnit.Framework;
using SisoDb.Glimpse;
using SisoDb.PineCone.Structures.Schemas;
using SisoDb.PineCone.Structures.Schemas.Builders;
using SisoDb.Serialization;
using SisoDb.Sql2012;

namespace SisoDb.UnitTests.Glimpse
{
    [TestFixture]
    public class SisoDbGlimpsePluginTests : UnitTestBase
    {
        [Test]
        public void GetData()
        {
            var structureSchemas = new StructureSchemas(new StructureTypeFactory(), new AutoStructureSchemaBuilder());
            structureSchemas.GetSchema<MyDummy>();
            var dbFake = new Mock<ISisoDatabase>();
            dbFake.SetupGet(f => f.Name).Returns("UnitTestDb");
            dbFake.Setup(f => f.ConnectionInfo).Returns(new Sql2012ConnectionInfo("data source=.;initial catalog=foo;integrated security=true;"));
            dbFake.Setup(f => f.Settings).Returns(DbSettings.CreateDefault());
            dbFake.Setup(f => f.Serializer).Returns(new ServiceStackJsonSerializer());
            dbFake.Setup(f => f.StructureSchemas).Returns(structureSchemas);
            SisoDbGlimpsePlugin.ResolveDatabasesUsing = () => new[] { dbFake.Object };
            
            var plugin = new SisoDbGlimpsePlugin();
            var data = plugin.GetData(Mock.Of<HttpContextBase>());

            JsonApprovals.VerifyAsJson(data);
        }

        private class MyDummy
        {
            public Guid StructureId { get; set; }
            public DateTime TimeStamp { get; set; }
            public Guid ConcurrencyToken { get; set; }
            public string StringValue { get; set; }
        }
    }
}