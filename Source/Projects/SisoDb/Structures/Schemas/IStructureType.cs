using System.Collections.Generic;

namespace SisoDb.Structures.Schemas
{
    public interface IStructureType
    {
        string Name { get; }
        IStructureProperty IdProperty { get; }
        IEnumerable<IStructureProperty> IndexableProperties { get; }
    }
}