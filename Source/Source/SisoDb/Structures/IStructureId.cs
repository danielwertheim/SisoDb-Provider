using System;

namespace SisoDb.Structures
{
    public interface IStructureId
    {
        IdTypes IdType { get; }

        Type DataType { get; }
        
        ValueType Value { get; }

        void SetIdentityValue(int value);
    }
}