using NUnit.Framework;
using SisoDb.Serialization;

namespace SisoDb.Tests.UnitTests.Serialization
{
    [TestFixture]
    public class NewtonsoftJsonSerializerTests : JsonSerializerTestBase
    {
        private readonly IJsonSerializer _jsonSertializer = new NewtonsoftJsonSerializer();

        protected override IJsonSerializer JsonSerializer
        {
            get { return _jsonSertializer; }
        }
    }
}