using System;
using System.IO;
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
        public override void Serialize_WhenReferencingOtherStructure_ReferencedStructureIsRepresentedInJson()
        {
            var structure = new Structure
            {
                ReferencedStructureId = 999,
                ReferencedStructure = { OtherStructureString = "To be included" },
                Item = { String1 = "To be included" }
            };

            var json = Serializer.Serialize(structure);

            const string expectedJson = "{\"StructureId\":0,\"ReferencedStructureId\":999,\"ReferencedStructure\":{\"StructureId\":999,\"OtherStructureString\":\"To be included\"},\"Item\":{\"String1\":\"To be included\",\"Int1\":0}}";
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public override void Serialize_WhenPrivateSetterExists_IsSerialized()
        {
            var entity = new JsonEntity();
            entity.SetName("Daniel");
            entity.SetAge(32);

            var json = Serializer.Serialize(entity);

            Assert.AreEqual("{\"StructureId\":0,\"Name\":\"Daniel\",\"Age\":32}", json);
        }

        [Test]
        public override void Serialize_WhenOnlyPrivateGetterAndStructureIdExists_ReturnsCorrectJson()
        {
            var entity = new JsonEntityWithPrivateGetter { Name = "Daniel" };

            var json = Serializer.Serialize(entity);

            Assert.AreEqual("{\"StructureId\":0}", json);
        }

        [Test]
        public override void Deserialize_WhenPrivateSettersExists_IsDeserialized()
        {
            const string json = @"{""Name"":""Daniel"", ""Age"":32}";

            var entity = Serializer.Deserialize<JsonEntity>(json);

            Assert.AreEqual("Daniel", entity.Name);
            Assert.AreEqual(32, entity.Age);
        }

        [Test]
        public override void Serialize_WhenItemIsYButSerializedAsX_AllYMembersAreSerialized()
        {
            var y = new JsonEntityY { Int1 = 42, String1 = "The String1", Data = new MemoryStream(BitConverter.GetBytes(333)).ToArray() };

            var json = Serializer.Serialize<JsonEntityX>(y);

            Assert.AreEqual("{\"Data\":\"TQEAAA==\",\"StructureId\":0,\"String1\":\"The String1\",\"Int1\":42}", json);
        }
    }
}