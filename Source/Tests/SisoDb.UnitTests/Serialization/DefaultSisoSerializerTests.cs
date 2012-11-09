using NUnit.Framework;
using SisoDb.Serialization;

namespace SisoDb.UnitTests.Serialization
{
    [TestFixture]
    public class DefaultSisoSerializerTests : SisoSerializerTests
    {
        protected override void OnFixtureInitialize()
        {
            base.OnFixtureInitialize();

            Serializer = new DefaultSisoSerializer();
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