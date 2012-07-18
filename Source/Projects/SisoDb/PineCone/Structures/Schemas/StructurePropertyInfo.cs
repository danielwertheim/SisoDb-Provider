using System;
using EnsureThat;
using PineCone.Annotations;

namespace PineCone.Structures.Schemas
{
    [Serializable]
    public struct StructurePropertyInfo
    {
        public readonly IStructureProperty Parent;
        public readonly string Name;
        public readonly Type DataType;
        public readonly UniqueModes? UniqueMode;

        public StructurePropertyInfo(string name, Type dataType, IStructureProperty parent = null, UniqueModes? uniqueMode = null)
        {
            Ensure.That(name, "name").IsNotNullOrWhiteSpace();
            Ensure.That(dataType, "dataType").IsNotNull();

            Parent = parent;
            Name = name;
            DataType = dataType;
            UniqueMode = uniqueMode;
        }
    }
}