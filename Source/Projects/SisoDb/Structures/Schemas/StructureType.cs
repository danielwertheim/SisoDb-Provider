using System.Collections.Generic;
using SisoDb.Core;

namespace SisoDb.Structures.Schemas
{
    public class StructureType : IStructureType
    {
        public string Name { get; private set; }

        public IStructureProperty IdProperty { get; private set; }

        public IEnumerable<IStructureProperty> IndexableProperties { get; private set; }

        public StructureType(string name, IStructureProperty idProperty, IEnumerable<IStructureProperty> indexableProperties)
        {
            Name = name.AssertNotNullOrWhiteSpace("name");
            IdProperty = idProperty.AssertNotNull("idProperty");
            IndexableProperties = indexableProperties.AssertHasItems("indexableProperties");
        }
    }
}