using System;
using System.Collections.Generic;
using SisoDb.Structures.Schemas.Builders;

namespace SisoDb.Structures.Schemas
{
    public interface IStructureSchemas
    {
        IStructureTypeFactory StructureTypeFactory { get; set; }
        IStructureSchemaBuilder StructureSchemaBuilder { get; set; }

        void Clear();
        IEnumerable<KeyValuePair<Type, IStructureSchema>> GetRegistrations();
        IStructureSchema GetSchema<T>() where T : class;
        IStructureSchema GetSchema(Type type);
        IEnumerable<IStructureSchema> GetSchemas();
        void RemoveSchema(Type type);
    }
}