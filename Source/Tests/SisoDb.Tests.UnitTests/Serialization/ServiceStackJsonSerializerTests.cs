using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using SisoDb.Dynamic;
using SisoDb.Serialization;
using SisoDb.TestUtils;

namespace SisoDb.Tests.UnitTests.Serialization
{
    [TestFixture]
    public class ServiceStackJsonSerializerTests : UnitTestBase
    {
        private readonly IJsonSerializer _jsonSerializer = new ServiceStackJsonSerializer();

        [Test]
        public void ToJsonOrEmptyString_WhenNullEntity_ReturnsEmptyString()
        {
            var json = _jsonSerializer.ToJsonOrEmptyString<JsonEntity>(null);

            Assert.AreEqual(string.Empty, json);
        }

        [Test]
        public void ToJsonOrEmptyString_WhenPrivateGetterExists_ReturnsEmptyJson()
        {
            var entity = new JsonEntityWithPrivateGetter { Name = "Daniel" };

            var json = _jsonSerializer.ToJsonOrEmptyString(entity);

            Assert.AreEqual("{}", json);
        }

        [Test]
        public void ToJsonOrEmptyString_WhenPrivateSetterExists_IsSerialized()
        {
            var entity = new JsonEntity();
            entity.SetName("Daniel");

            var json = _jsonSerializer.ToJsonOrEmptyString(entity);

            Assert.AreEqual("{\"Name\":\"Daniel\"}", json);
        }

        [Test]
        public void ToItemOrNull_WhenPrivateSetterExists_IsDeserialized()
        {
            var json = @"{""Name"":""Daniel""}";

            var entity = _jsonSerializer.ToItemOrNull<JsonEntity>(json);

            Assert.AreEqual("Daniel", entity.Name);
        }

        [Test]
        public void ToJsonOrEmptyString_WhenItemIsYButSerializedAsX_OnlyXMembersAreSerialized()
        {
            var y = new JsonEntityY { Int1 = 42, String1 = "The String1", Data = new MemoryStream(BitConverter.GetBytes(333)) };

            var json = _jsonSerializer.ToJsonOrEmptyString<JsonEntityX>(y);

            Assert.AreEqual("{\"String1\":\"The String1\",\"Int1\":42}", json);
        }

        [Test]
        public void ToTypedKeyValue_WhenJsonWithDataTypes_DictionaryGetsTypedValues()
        {
            const string json = "{\"Name\":\"Daniel\",\"Age\":31,\"Item\":{\"Int1\":42,\"DateTime1\":\"\\/Date(-3600000+0000)\\/\"}}";
            var typeDescriptor = new TypeDescriptor
            {
                {"Name", typeof (string)},
                {"Age", typeof (int)},
                {"Item", typeof (Item)},
            };

            var kv = _jsonSerializer.ToTypedKeyValueOrNull(typeDescriptor, json);

            var expectedKv = new Dictionary<string, object>
            {
                {"Name", "Daniel"},{"Age", 31}, {"Item", new Item{Int1 = 42, DateTime1 = new DateTime(1970,01,01)}}
            };
            CustomAssert.KeyValueEquality(expectedKv, kv);
        }

        [Test]
        public void ToJsonOrEmptyString_WhenReferencingOtherStructure_ReferencedStructureIsNotRepresentedInJson()
        {
            var structure = new Structure
            {
                ReferencedStructureId = 999,
                ReferencedStructure = {OtherStructureString = "Not to be included"},
                Item = {String1 = "To be included"}
            };

            var json = _jsonSerializer.ToJsonOrEmptyString(structure);

            const string expectedJson = "{\"Id\":0,\"ReferencedStructureId\":999,\"Item\":{\"String1\":\"To be included\",\"Int1\":0}}";
            Assert.AreEqual(expectedJson, json);
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

        private class JsonEntityX
        {
            public string String1 { get; set; }

            public int Int1 { get; set; }
        }

        private class JsonEntityY : JsonEntityX
        {
            public Stream Data { get; set; }
        }

        private class Item
        {
            public string String1 { get; set; }

            public int Int1 { get; set; }

            public DateTime? DateTime1 { get; set; }
        }

        private class Structure
        {
            public int Id { get; set; }

            public int ReferencedStructureId 
            {
                get { return ReferencedStructure.Id; }
                set { ReferencedStructure.Id = value; }
            }

            public OtherStructure ReferencedStructure { get; set; }

            public Item Item { get; set; }

            public Structure()
            {
                ReferencedStructure = new OtherStructure();
                Item = new Item();
            }
        }

        private class OtherStructure
        {
            public int Id { get; set; }

            public string OtherStructureString { get; set; }
        }
    }
}