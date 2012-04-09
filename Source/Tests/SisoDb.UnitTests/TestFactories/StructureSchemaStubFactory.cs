using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using PineCone.Structures.Schemas;
using PineCone.Structures.Schemas.Builders;
using PineCone.Structures.Schemas.MemberAccessors;

namespace SisoDb.UnitTests.TestFactories
{
	internal class StructureSchemasStub : IStructureSchemas
	{
		public void Clear()
		{
			throw new NotImplementedException();
		}

		public IEnumerable<KeyValuePair<Type, IStructureSchema>> GetRegistrations()
		{
			throw new NotImplementedException();
		}

		public IStructureSchema GetSchema<T>() where T : class
		{
		    return GetSchema(typeof (T));
		}

		public IStructureSchema GetSchema(Type type)
		{
            return StructureSchemaStubFactory.Stub(type);
		}

		public IEnumerable<IStructureSchema> GetSchemas()
		{
			throw new NotImplementedException();
		}

		public void RemoveSchema(Type type)
		{
			throw new NotImplementedException();
		}

		public IStructureTypeFactory StructureTypeFactory
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		public ISchemaBuilder SchemaBuilder
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}
	}

    internal static class StructureSchemaStubFactory
    {
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
                var indexAccessors = indexAccessorsPaths.Select(path => new IndexAccessor(StructurePropertyTestFactory.GetPropertyByPath(structureType, path)));
                schemaStub.Setup(s => s.IndexAccessors).Returns(indexAccessors.ToArray);
            }

            return schemaStub.Object;
        }
    }
}