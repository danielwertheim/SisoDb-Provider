using System;

namespace SisoDb.Structures.Schemas
{
    public interface IStructureSchemas
    {
        IStructureSchema GetSchema(Type type);

        void RemoveSchema(Type type);
        
        void Clear();
    }
}