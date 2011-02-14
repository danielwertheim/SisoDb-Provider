using NUnit.Framework;
using SisoDb.Serialization;

namespace SisoDb.Tests.UnitTests.Serialization
{
    [TestFixture]
    public class ServiceStackJsonSerializerTests : JsonSerializerTestBase
    {
        private readonly IJsonSerializer _jsonSertializer = new ServiceStackJsonSerializer();

        protected override IJsonSerializer JsonSerializer
        {
            get { return _jsonSertializer; }
        }
    }
}