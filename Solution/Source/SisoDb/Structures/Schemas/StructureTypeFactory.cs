using System;
using SisoDb.Core;
using SisoDb.Structures.Schemas.Configuration;

namespace SisoDb.Structures.Schemas
{
    public class StructureTypeFactory : IStructureTypeFactory
    {
        public IStructureTypeReflecter Reflecter { protected get; set; }

        public IStructureTypeConfigurations Configurations { get; set; }

        public StructureTypeFactory(IStructureTypeReflecter reflecter)
        {
            Reflecter = reflecter.AssertNotNull("reflecter");
            Configurations = new StructureTypeConfigurations();
        }

        public IStructureType CreateFor(Type type)
        {
            var config = Configurations.GetConfiguration(type);

            //Scenario: Index ALL which is the default behavior
            if (config == null || config.IsEmpty)
                return new StructureType(
                    type.Name,
                    Reflecter.GetIdProperty(type),
                    Reflecter.GetIndexableProperties(type));

            return new StructureType(
                type.Name,
                Reflecter.GetIdProperty(type),
                (config.MemberPathsNotBeingIndexed.Count > 0)
                ? Reflecter.GetIndexablePropertiesExcept(type, config.MemberPathsNotBeingIndexed) //Scenario: Index ALL EXCEPT
                : Reflecter.GetSpecificIndexableProperties(type, config.MemberPathsBeingIndexed));//Scenario: Index only THIS
        }
    }
}