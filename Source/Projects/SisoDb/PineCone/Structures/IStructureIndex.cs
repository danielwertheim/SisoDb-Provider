using System;

namespace SisoDb.PineCone.Structures
{
    public interface IStructureIndex : IEquatable<IStructureIndex>
    {
        IStructureId StructureId { get; }

        StructureIndexType IndexType { get; }

        string Path { get;  }
        
        object Value { get; }

		Type DataType { get; }

    	DataTypeCode DataTypeCode { get; }

		bool IsUnique { get; }
    }
}