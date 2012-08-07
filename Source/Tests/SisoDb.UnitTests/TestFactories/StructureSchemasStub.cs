using System;
using System.Collections.Generic;
using SisoDb.PineCone.Structures.Schemas;
using SisoDb.PineCone.Structures.Schemas.Builders;

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
            return StructureSchemaTestFactory.Stub(type);
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

        public IStructureSchemaBuilder StructureSchemaBuilder
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
    }
}