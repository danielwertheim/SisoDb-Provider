using System;
using System.Linq;
using Moq;
using SisoDb.Structures.Schemas;
using SisoDb.Structures.Schemas.Builders;
using SisoDb.Structures.Schemas.MemberAccessors;

namespace SisoDb.UnitTests.TestFactories
{
    internal static class StructureSchemaTestFactory
    {
        private static readonly IStructureTypeFactory StructureTypeFactory = new StructureTypeFactory();
        private static readonly IStructureSchemaBuilder StructureSchemaBuilder = new AutoStructureSchemaBuilder();
        private static readonly DataTypeConverter DataTypeConverter = new DataTypeConverter();

        internal static IStructureSchema CreateRealFrom<T>() where T : class
        {
            return StructureSchemaBuilder.CreateSchema(StructureTypeFactory.CreateFor<T>());
        }

        internal static IStructureSchema CreateRealFrom(Type type)
        {
            return StructureSchemaBuilder.CreateSchema(StructureTypeFactory.CreateFor(type));
        }

        internal static IStructureSchema CreateRealFrom(IStructureType structureType)
        {
            return StructureSchemaBuilder.CreateSchema(structureType);
        }

        internal static IStructureSchema Stub(string name = "Temp")
        {
            var schemaStub = new Mock<IStructureSchema>();
            schemaStub.Setup(s => s.Name).Returns(name);

            return schemaStub.Object;
        }

        internal static IStructureSchema Stub<T>(bool generateIdAccessor = false, string[] indexAccessorsPaths = null) where T : class
        {
            return Stub(typeof (T), generateIdAccessor, indexAccessorsPaths);
        }

        internal static IStructureSchema Stub(Type structureType, bool generateIdAccessor = false, string[] indexAccessorsPaths = null)
        {
            var schemaStub = new Mock<IStructureSchema>();

            schemaStub.Setup(s => s.Name).Returns(structureType.Name);

            if (generateIdAccessor)
            {
                var idAccessor = new IdAccessor(StructurePropertyTestFactory.GetIdProperty(structureType));
                schemaStub.Setup(s => s.IdAccessor).Returns(idAccessor);
            }

            if (indexAccessorsPaths != null)
            {
                var indexAccessors = indexAccessorsPaths.Select(path =>
                {
                    var property = StructurePropertyTestFactory.GetPropertyByPath(structureType, path);
                    return new IndexAccessor(property, DataTypeConverter.Convert(property));
                });
                schemaStub.Setup(s => s.IndexAccessors).Returns(indexAccessors.ToArray);
            }

            return schemaStub.Object;
        }
    }
}