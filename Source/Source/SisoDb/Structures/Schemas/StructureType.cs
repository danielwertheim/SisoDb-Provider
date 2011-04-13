using System;
using System.Collections.Generic;
using SisoDb.Core;

namespace SisoDb.Structures.Schemas
{
    public class StructureType : IStructureType
    {
        private readonly Type _type;

        public string Name { get; private set; }

        public IStructureProperty IdProperty { get; private set; }

        public IEnumerable<IStructureProperty> IndexableProperties { get; private set; }

        public StructureType(Type type)
        {
            _type = type.AssertNotNull("type");

            Name = _type.Name;
            IdProperty = SisoDbEnvironment.StructureTypeReflecter.GetIdProperty(_type);
            IndexableProperties = SisoDbEnvironment.StructureTypeReflecter.GetIndexableProperties(_type, new[] { StructureSchema.IdMemberName });
        }
    }
}