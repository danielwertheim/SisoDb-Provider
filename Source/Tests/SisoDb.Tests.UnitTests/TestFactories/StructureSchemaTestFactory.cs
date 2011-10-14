using System.Linq;
using Moq;
using PineCone.Structures.Schemas;
using PineCone.Structures.Schemas.MemberAccessors;

namespace SisoDb.Tests.UnitTests.TestFactories
{
    internal static class StructureSchemaTestFactory
    {
        internal static IStructureSchema Stub(string name = "Temp")
        {
            var schemaStub = new Mock<IStructureSchema>();
            schemaStub.Setup(s => s.Name).Returns(name);

            return schemaStub.Object;
        }

        internal static IStructureSchema Stub<T>(bool generateIdAccessor = false, string[] indexAccessorsPaths = null) where T : class
        {
            var schemaStub = new Mock<IStructureSchema>();

            schemaStub.Setup(s => s.Name).Returns(typeof(T).Name);

            if (generateIdAccessor)
            {
                var idAccessor = new IdAccessor(StructurePropertyTestFactory.GetIdProperty<T>());
                schemaStub.Setup(s => s.IdAccessor).Returns(idAccessor);
            }

            if (indexAccessorsPaths != null)
            {
                var indexAccessors = indexAccessorsPaths.Select(path => new IndexAccessor(StructurePropertyTestFactory.GetPropertyByPath<T>(path)));
                schemaStub.Setup(s => s.IndexAccessors).Returns(indexAccessors.ToArray);
            }

            return schemaStub.Object;
        }
    }
}