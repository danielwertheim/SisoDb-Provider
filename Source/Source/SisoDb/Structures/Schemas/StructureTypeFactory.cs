using System;
using SisoDb.Core;

namespace SisoDb.Structures.Schemas
{
    public class StructureTypeFactory : IStructureTypeFactory
    {
        public IStructureTypeReflecter Reflecter { protected get; set; }

        public StructureTypeFactory(IStructureTypeReflecter reflecter)
        {
            Reflecter = reflecter.AssertNotNull("reflecter");
        }

        public IStructureType CreateFor(Type type)
        {
            return new StructureType(
                type.Name,
                Reflecter.GetIdProperty(type),
                Reflecter.GetIndexableProperties(type, new[] { StructureSchema.IdMemberName }));
        }
    }
}