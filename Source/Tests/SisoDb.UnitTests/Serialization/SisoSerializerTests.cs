using System;
using System.IO;
using NUnit.Framework;
using SisoDb.Serialization;

namespace SisoDb.UnitTests.Serialization
{
    public abstract class SisoSerializerTests : UnitTestBase
    {
        protected ISisoSerializer Serializer;

        [Test]
        public void Serialize_WhenNullEntity_ReturnsEmptyString()
        {
            var json = Serializer.Serialize<JsonEntity>(null);

            Assert.AreEqual(string.Empty, json);
        }

        [Test]
        public void Serialize_WhenOnlyPrivateGetterAndStructureIdExists_ReturnsJsonWithStructureId()
        {
            var entity = new JsonEntityWithPrivateGetter { Name = "Daniel" };

            var json = Serializer.Serialize(entity);

            Assert.AreEqual("{\"StructureId\":0}", json);
        }

        [Test]
        public void Serialize_WhenPrivateSetterExists_IsSerialized()
        {
            var entity = new JsonEntity();
            entity.SetName("Daniel");
            entity.SetAge(32);

            var json = Serializer.Serialize(entity);

            Assert.AreEqual("{\"StructureId\":0,\"Name\":\"Daniel\",\"Age\":32}", json);
        }

        [Test]
        public void Serialize_WhenItemIsYButSerializedAsX_AllYMembersAreSerialized()
        {
            var y = new JsonEntityY { Int1 = 42, String1 = "The String1", Data = new MemoryStream(BitConverter.GetBytes(333)).ToArray() };

            var json = Serializer.Serialize<JsonEntityX>(y);

			Assert.AreEqual("{\"Data\":\"TQEAAA==\",\"StructureId\":0,\"String1\":\"The String1\",\"Int1\":42}", json);
        }

        [Test]
        public void Serialize_WhenReferencingOtherStructure_ReferencedStructureIsRepresentedInJson()
        {
            var structure = new Structure
            {
                ReferencedStructureId = 999,
                ReferencedStructure = {OtherStructureString = "To be included"},
                Item = {String1 = "To be included"}
            };

            var json = Serializer.Serialize(structure);

            const string expectedJson = "{\"StructureId\":0,\"ReferencedStructureId\":999,\"ReferencedStructure\":{\"StructureId\":999,\"OtherStructureString\":\"To be included\"},\"Item\":{\"String1\":\"To be included\",\"Int1\":0}}";
            Assert.AreEqual(expectedJson, json);
        }

        protected class JsonEntity
        {
            public int StructureId { get; set; }
            public string Name { get; private set; }
            public int Age { get; private set; }

            public void SetName(string name)
            {
                Name = name;
            }

            public void SetAge(int age)
            {
                Age = age;
            }
        }

        protected class JsonEntityWithPrivateGetter
        {
            public int StructureId { get; set; }
            public string Name { private get; set; }
        }

        protected class JsonEntityX
        {
            public int StructureId { get; set; }
            public string String1 { get; set; }
            public int Int1 { get; set; }
        }

        protected class JsonEntityY : JsonEntityX
        {
            public byte[] Data { get; set; }
        }

        protected class Item
        {
            public string String1 { get; set; }
            public int Int1 { get; set; }
            public DateTime? DateTime1 { get; set; }
        }

        protected class Structure
        {
            public int StructureId { get; set; }
            public int ReferencedStructureId 
            {
                get { return ReferencedStructure.StructureId; }
                set { ReferencedStructure.StructureId = value; }
            }

            public OtherStructure ReferencedStructure { get; set; }
            public Item Item { get; set; }
            public Structure()
            {
                ReferencedStructure = new OtherStructure();
                Item = new Item();
            }
        }

        protected class OtherStructure
        {
            public int StructureId { get; set; }
            public string OtherStructureString { get; set; }
        }
    }
}