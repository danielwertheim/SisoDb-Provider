using System.Linq;
using Moq;
using NUnit.Framework;
using SisoDb.Structures.Schemas;

namespace SisoDb.UnitTests.Structures.Schemas
{
    [TestFixture]
    public class StructureTypeFactoryTests
    {
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
            reflecterMock.Setup(m => m.GetIndexableProperties(type, false)).Returns(() =>
            {
                var indexProperty = new Mock<IStructureProperty>();

                return new[] { indexProperty.Object };
            });

            var factory = new StructureTypeFactory(t => reflecterMock.Object);
            factory.CreateFor(type);

            reflecterMock.Verify(m => m.GetIndexableProperties(type, false));
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
            reflecterMock.Setup(m => m.GetIndexablePropertiesExcept(type, false, new[] { "ExcludeTEMP" })).Returns(() =>
            {
                var indexProperty = new Mock<IStructureProperty>();

                return new[] { indexProperty.Object };
            });

            var factory = new StructureTypeFactory(t => reflecterMock.Object);
            factory.Configurations.Configure(type, cfg => cfg.DoNotIndexThis("ExcludeTEMP"));
            factory.CreateFor(type);

            reflecterMock.Verify(m => m.GetIndexablePropertiesExcept(type, false, new[] { "ExcludeTEMP" }));
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
            reflecterMock.Setup(m => m.GetSpecificIndexableProperties(type, false, new[] { "IncludeTEMP" })).Returns(() =>
            {
                var indexProperty = new Mock<IStructureProperty>();

                return new[] { indexProperty.Object };
            });

            var factory = new StructureTypeFactory(t => reflecterMock.Object);
            factory.Configurations.Configure(type, cfg => cfg.OnlyIndexThis("IncludeTEMP"));
            factory.CreateFor(type);

            reflecterMock.Verify(m => m.GetSpecificIndexableProperties(type, false, new[] { "IncludeTEMP" }));
        }

        [Test]
        public void CreateFor_When_no_config_for_allowing_nested_structures_exists_It_should_not_include_nested_members()
        {
            var factory = new StructureTypeFactory();

            var structureType = factory.CreateFor<WithContainedStructure>();

            Assert.AreEqual(0, structureType.IndexableProperties.Length);
            Assert.IsFalse(structureType.IndexableProperties.Any(p => p.Path == "Contained.StructureId"));
            Assert.IsFalse(structureType.IndexableProperties.Any(p => p.Path == "Contained.NestedValue"));
        }

        [Test]
        public void CreateFor_When_config_for_allowing_nested_structures_exists_It_should_include_nested_members()
        {
            var factory = new StructureTypeFactory();
            factory.Configurations.Configure<WithContainedStructure>(cfg => cfg.AllowNestedStructures());

            var structureType = factory.CreateFor<WithContainedStructure>();

            Assert.AreEqual(2, structureType.IndexableProperties.Length);
            Assert.IsTrue(structureType.IndexableProperties.Any(p => p.Path == "Contained.StructureId"));
            Assert.IsTrue(structureType.IndexableProperties.Any(p => p.Path == "Contained.NestedValue"));
        }

        [Test]
        public void CreateFor_When_no_config_for_allowing_nested_structures_exists_It_should_extract_contained_structure_properties()
        {
            var factory = new StructureTypeFactory();

            var structureType = factory.CreateFor<WithContainedStructures>();

            Assert.AreEqual(2, structureType.ContainedStructureProperties.Length);
            Assert.AreEqual(1, structureType.ContainedStructureProperties.Count(p => p.Path == "Contained1"));
            Assert.AreEqual(1, structureType.ContainedStructureProperties.Count(p => p.Path == "Contained2"));
        }

        private class MyClass
        {
        }

        private class WithContainedStructure
        {
            public Structure Contained { get; set; }
        }

        private class WithContainedStructures
        {
            public Structure Contained1 { get; set; }
            public Structure Contained2 { get; set; }
        }

        private class Structure
        {
            public int StructureId { get; set; }
            public int NestedValue { get; set; }
        }
    }
}