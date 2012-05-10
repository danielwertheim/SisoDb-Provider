using Moq;
using NUnit.Framework;
using PineCone.Structures.IdGenerators;
using SisoDb.Configurations;
using SisoDb.Serialization;
using SisoDb.Structures;

namespace SisoDb.UnitTests.Configurations
{
    [TestFixture]
    public class DbConfigurationTests : UnitTestBase
    {
        [Test]
        public void UseManualStructureIdAssignment_Should_user_structurebuilder_with_EmptyStructureIdGenerator_and_default_serializer()
        {
            StructureBuilderFactoryForInserts assigned = null;
            var dbMock = new Mock<ISisoDatabase>();
            dbMock.SetupSet(f => f.StructureBuilders.ForInserts = It.IsAny<StructureBuilderFactoryForInserts>()).Callback<StructureBuilderFactoryForInserts>(value => assigned = value);

            dbMock.Object.Configure().UseManualStructureIdAssignment();
            var builder = assigned(null, null);

            Assert.AreEqual(typeof(EmptyStructureIdGenerator), builder.StructureIdGenerator.GetType());
            Assert.AreEqual(typeof(SerializerForStructureBuilder), builder.StructureSerializer.GetType());
        }
    }
}