using System;
using EnsureThat;
using PineCone.Resources;

namespace PineCone.Structures
{
	[Serializable]
    public class StructureIndex : IStructureIndex
    {
        public IStructureId StructureId { get; private set; }
        
        public StructureIndexType IndexType { get; private set; }

        public string Path { get; private set; }

        public object Value { get; private set; }

		public Type DataType { get; private set; }

		public DataTypeCode DataTypeCode { get; private set; }

        public bool IsUnique { get; private set; }

        public StructureIndex(IStructureId structureId, string path, object value, Type dataType, DataTypeCode dataTypeCode, StructureIndexType indexType = StructureIndexType.Normal)
        {
            var valueIsOkType = value is string || value is ValueType;

            if (value != null && !valueIsOkType)
                throw new ArgumentException(ExceptionMessages.StructureIndex_ValueArgument_IncorrectType);

            Ensure.That(structureId, "structureId").IsNotNull();
            Ensure.That(path, "path").IsNotNullOrWhiteSpace();
			Ensure.That(dataType, "dataType").IsNotNull();

            StructureId = structureId;
            Path = path;
            Value = value;
        	DataType = dataType;
			DataTypeCode = dataTypeCode;
            IndexType = indexType;
            IsUnique = indexType.IsUnique();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IStructureIndex);
        }

        public bool Equals(IStructureIndex other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.StructureId, StructureId) && Equals(other.Path, Path) && Equals(other.Value, Value);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (StructureId.GetHashCode()*397) ^ (Path != null ? Path.GetHashCode() : 0);
            }
        }
    }
}