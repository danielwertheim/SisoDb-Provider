using System;
using System.Collections.Generic;
using NUnit.Framework;
using SisoDb.Dynamic;
using SisoDb.TestUtils;

namespace SisoDb.Tests.UnitTests.Dynamic
{
    [TestFixture]
    public class DynamicServiceStackJsonSerializerTests : UnitTestBase
    {
        private readonly IDynamicJsonSerializer _jsonSerializer = new DynamicServiceStackJsonSerializer();

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

        private class Item
        {
            public string String1 { get; set; }

            public int Int1 { get; set; }

            public DateTime? DateTime1 { get; set; }
        }
    }
}