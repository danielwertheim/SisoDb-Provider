using System;
using Moq;
using NUnit.Framework;
using SisoDb.Resources;
using SisoDb.Structures.Schemas;
using SisoDb.TestUtils;

namespace SisoDb.Tests.UnitTests.Structures.Schemas
{
    [TestFixture]
    public class StructureTypeFactoryTests
    {
        [Test]
        public void Ctor_WhenNoReflecter_ThrowsArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new StructureTypeFactory(null));

            Assert.AreEqual("reflecter", ex.ParamName);
        }

        [Test]
        public void CreateFor_Calls_GetIndexableProperties_On_Reflecter_Excluding_IdName()
        {
            var type = typeof(MyClass);
            var reflecterMock = new Mock<IStructureTypeReflecter>();
            reflecterMock.Setup(m => m.GetIdProperty(type)).Returns(() =>
            {
                var idProperty = new Mock<IStructureProperty>();

                return idProperty.Object;
            });
            reflecterMock.Setup(m => m.GetIndexableProperties(type, It.IsAny<string[]>())).Returns(() =>
            {
                var indexProperty = new Mock<IStructureProperty>();

                return new[] { indexProperty.Object };
            });

            var factory = new StructureTypeFactory(reflecterMock.Object);
            factory.CreateFor(type);

            reflecterMock.Verify(m => m.GetIndexableProperties(type, new[] { StructureSchema.IdMemberName }));
        }

        private class MyClass
        {
        }
    }
}