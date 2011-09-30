using System;
using SisoDb.Structures.Schemas.Builders;

namespace SisoDb.Structures.Schemas
{
    public interface IStructureSchemas
    {
        IStructureTypeFactory StructureTypeFactory { get; set; }
        
        ISchemaBuilder SchemaBuilder { get; set; }

        IStructureSchema GetSchema(Type type);

        void RemoveSchema(Type type);
        
        void Clear();
    }
}