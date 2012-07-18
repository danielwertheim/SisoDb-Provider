using System;
using System.Linq;
using SisoDb.PineCone.Structures.Schemas.Configuration;

namespace SisoDb.PineCone.Structures.Schemas
{
    public class StructureTypeFactory : IStructureTypeFactory
    {
        public IStructureTypeReflecter Reflecter { get; private set; }

        public IStructureTypeConfigurations Configurations { get; private set; }

        public StructureTypeFactory(IStructureTypeReflecter reflecter = null, IStructureTypeConfigurations configurations = null)
        {
            Reflecter = reflecter ?? new StructureTypeReflecter();
            Configurations = configurations ?? new StructureTypeConfigurations();
        }

        public virtual IStructureType CreateFor<T>() where T : class 
        {
            return CreateFor(typeof(T));
        }

        public virtual IStructureType CreateFor(Type type)
        {
            var config = Configurations.GetConfiguration(type);

            //Scenario: Index ALL which is the default behavior
            if (config == null || config.IsEmpty)
                return new StructureType(
                    type,
                    Reflecter.GetIdProperty(type),
                    Reflecter.GetConcurrencyTokenProperty(type),
                    Reflecter.GetTimeStampProperty(type),
                    Reflecter.GetIndexableProperties(type).ToArray());

            return new StructureType(
                type,
                Reflecter.GetIdProperty(type),
                Reflecter.GetConcurrencyTokenProperty(type),
                Reflecter.GetTimeStampProperty(type),
                ((config.MemberPathsNotBeingIndexed.Count > 0)
                ? Reflecter.GetIndexablePropertiesExcept(type, config.MemberPathsNotBeingIndexed) //Scenario: Index ALL EXCEPT
                : Reflecter.GetSpecificIndexableProperties(type, config.MemberPathsBeingIndexed)).ToArray());//Scenario: Index only THIS
        }
    }
}