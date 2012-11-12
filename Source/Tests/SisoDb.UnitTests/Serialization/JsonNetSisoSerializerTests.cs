using NUnit.Framework;
using SisoDb.JsonNet;

namespace SisoDb.UnitTests.Serialization
{
    [TestFixture]
    public class JsonNetSisoSerializerTests : SisoSerializerTests
    {
        protected override void OnFixtureInitialize()
        {
            base.OnFixtureInitialize();

            Serializer = new JsonNetSisoSerializer();
        }

        [Test]
        public void Deserialize_WhenPrivateSettersExists_IsDeserialized()
        {
            Serializer = new JsonNetSisoSerializer(s => s.ContractResolver = new PrivateSetterContractResolver());
            var json = @"{""Name"":""Daniel"", ""Age"":32}";
            
            var entity = Serializer.Deserialize<JsonEntity>(json);

            Assert.AreEqual("Daniel", entity.Name);
            Assert.AreEqual(32, entity.Age);
        }
    }
}