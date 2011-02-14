using NUnit.Framework;
using SisoDb.Serialization;

namespace SisoDb.Tests.UnitTests.Serialization
{
    public abstract class JsonSerializerTestBase
    {
        protected abstract IJsonSerializer JsonSerializer { get; }

        [Test]
        public void ToJsonOrEmptyString_WhenNullEntity_ReturnsEmptyString()
        {
            var json = JsonSerializer.ToJsonOrEmptyString<JsonEntity>(null);

            Assert.AreEqual(string.Empty, json);
        }

        [Test]
        public void ToJsonOrEmptyString_WhenPrivateGetterExists_ReturnsEmptyJson()
        {
            var entity = new JsonEntityWithPrivateGetter { Name = "Daniel" };

            var json = JsonSerializer.ToJsonOrEmptyString(entity);

            Assert.AreEqual("{}", json);
        }

        [Test]
        public void ToJsonOrEmptyString_WhenPrivateSetterExists_IsSerialized()
        {
            var entity = new JsonEntity();
            entity.SetName("Daniel");

            var json = JsonSerializer.ToJsonOrEmptyString(entity);

            Assert.AreEqual("{\"Name\":\"Daniel\"}", json);
        }

        [Test]
        public void ToItemOrNull_WhenPrivateSetterExists_IsDeserialized()
        {
            var json = @"{""Name"":""Daniel""}";

            var entity = JsonSerializer.ToItemOrNull<JsonEntity>(json);

            Assert.AreEqual("Daniel", entity.Name);
        }

        private class JsonEntity
        {
            public string Name { get; private set; }

            public void SetName(string name)
            {
                Name = name;
            }
        }

        private class JsonEntityWithPrivateGetter
        {
            public string Name { private get; set; }
        }
    }
}