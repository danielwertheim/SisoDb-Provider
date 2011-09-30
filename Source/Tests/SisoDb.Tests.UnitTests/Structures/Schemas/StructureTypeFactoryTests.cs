using System;
using Moq;
using NUnit.Framework;
using SisoDb.Structures.Schemas;

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
        public void CreateFor_WhenNoExplicitConfigExistsForType_InvokesGetIndexablePropertiesOnReflecter()
        {
            var type = typeof(MyClass);
            var reflecterMock = new Mock<IStructureTypeReflecter>();
            reflecterMock.Setup(m => m.GetIdProperty(type)).Returns(() =>
            {
                var idProperty = new Mock<IStructureProperty>();

                return idProperty.Object;
            });
            reflecterMock.Setup(m => m.GetIndexableProperties(type)).Returns(() =>
            {
                var indexProperty = new Mock<IStructureProperty>();

                return new[] { indexProperty.Object };
            });

            var factory = new StructureTypeFactory(reflecterMock.Object);
            factory.CreateFor(type);

            reflecterMock.Verify(m => m.GetIndexableProperties(type));
        }

        [Test]
        public void CreateFor_WhenConfigExcludingMembersExistsForType_InvokesGetIndexablePropertiesExceptOnReflecter()
        {
            var type = typeof(MyClass);
            var reflecterMock = new Mock<IStructureTypeReflecter>();
            reflecterMock.Setup(m => m.GetIdProperty(type)).Returns(() =>
            {
                var idProperty = new Mock<IStructureProperty>();

                return idProperty.Object;
            });
            reflecterMock.Setup(m => m.GetIndexablePropertiesExcept(type, new[] { "ExcludeTEMP" })).Returns(() =>
            {
                var indexProperty = new Mock<IStructureProperty>();

                return new[] { indexProperty.Object };
            });

            var factory = new StructureTypeFactory(reflecterMock.Object);
            factory.Configurations.NewForType(type).DoNotIndexThis("ExcludeTEMP");
            factory.CreateFor(type);

            reflecterMock.Verify(m => m.GetIndexablePropertiesExcept(type, new[] { "ExcludeTEMP" }));
        }

        [Test]
        public void CreateFor_WhenConfigIncludingMembersExistsForType_InvokesGetSpecificIndexablePropertiesOnReflecter()
        {
            var type = typeof(MyClass);
            var reflecterMock = new Mock<IStructureTypeReflecter>();
            reflecterMock.Setup(m => m.GetIdProperty(type)).Returns(() =>
            {
                var idProperty = new Mock<IStructureProperty>();

                return idProperty.Object;
            });
            reflecterMock.Setup(m => m.GetSpecificIndexableProperties(type, new[] { "IncludeTEMP" })).Returns(() =>
            {
                var indexProperty = new Mock<IStructureProperty>();

                return new[] { indexProperty.Object };
            });

            var factory = new StructureTypeFactory(reflecterMock.Object);
            factory.Configurations.NewForType(type).OnlyIndexThis("IncludeTEMP");
            factory.CreateFor(type);

            reflecterMock.Verify(m => m.GetSpecificIndexableProperties(type, new[] { "IncludeTEMP" }));
        }

        private class MyClass
        {
        }
    }
}