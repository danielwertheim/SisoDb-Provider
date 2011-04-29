using System;

namespace SisoDb.Structures
{
    public interface ISisoId
    {
        IdTypes IdType { get; }

        Type DataType { get; }
        
        ValueType Value { get; }

        void SetIdentityValue(int value);
    }
}