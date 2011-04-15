using System;

namespace SisoDb.Structures.Schemas
{
    public interface IStructureTypeFactory
    {
        IStructureType CreateFor(Type type);
    }
}