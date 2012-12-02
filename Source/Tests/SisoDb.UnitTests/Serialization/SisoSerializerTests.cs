using System;
using NUnit.Framework;
using SisoDb.Serialization;

namespace SisoDb.UnitTests.Serialization
{
    public abstract class SisoSerializerTests : UnitTestBase
    {
        protected ISisoSerializer Serializer;

        [Test]
        public abstract void Serialize_WhenReferencingOtherStructure_ReferencedStructureIsRepresentedInJson();

        [Test]
        public abstract void Serialize_WhenPrivateSetterExists_IsSerialized();

        [Test]
        public abstract void Deserialize_WhenPrivateSettersExists_IsDeserialized();

        [Test]
        public abstract void Serialize_WhenItemIsYButSerializedAsX_AllYMembersAreSerialized();

        [Test]
        public abstract void Serialize_WhenOnlyPrivateGetterAndStructureIdExists_ReturnsCorrectJson();

        [Test]
        public virtual void Serialize_WhenNullEntity_ReturnsEmptyString()
        {
            var json = Serializer.Serialize<JsonEntity>(null);

            Assert.AreEqual(string.Empty, json);
        }

        protected class JsonEntity
        {
            public int StructureId { get; set; }
            public string Name { get; private set; }
            public int Age { get; private set; }

            public virtual void SetName(string name)
            {
                Name = name;
            }

            public virtual void SetAge(int age)
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