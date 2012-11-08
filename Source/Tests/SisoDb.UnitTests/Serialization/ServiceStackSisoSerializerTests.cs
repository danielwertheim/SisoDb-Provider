using NUnit.Framework;
using SisoDb.ServiceStack;

namespace SisoDb.UnitTests.Serialization
{
    [TestFixture]
    public class ServiceStackSisoSerializerTests : SisoSerializerTests
    {
        protected override void OnFixtureInitialize()
        {
            base.OnFixtureInitialize();

            Serializer = new ServiceStackSisoSerializer();
        }

        [Test]
        public void Deserialize_WhenPrivateSettersExists_IsDeserialized()
        {
            var json = @"{""Name"":""Daniel"", ""Age"":32}";

            var entity = Serializer.Deserialize<JsonEntity>(json);

            Assert.AreEqual("Daniel", entity.Name);
            Assert.AreEqual(32, entity.Age);
        }
    }
}