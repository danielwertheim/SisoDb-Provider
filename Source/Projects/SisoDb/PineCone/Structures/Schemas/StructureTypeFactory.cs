using System;
using SisoDb.PineCone.Structures.Schemas.Configuration;

namespace SisoDb.PineCone.Structures.Schemas
{
    public class StructureTypeFactory : IStructureTypeFactory
    {
        public Func<IStructureTypeConfig, IStructureTypeReflecter> ReflecterFn { get; set; }

        public IStructureTypeConfigurations Configurations { get; set; }

        public StructureTypeFactory(Func<IStructureTypeConfig, IStructureTypeReflecter> reflecterFn = null, IStructureTypeConfigurations configurations = null)
        {
            ReflecterFn = reflecterFn ?? (cfg => new StructureTypeReflecter(cfg));
            Configurations = configurations ?? new StructureTypeConfigurations();
        }

        public virtual IStructureType CreateFor<T>() where T : class 
        {
            return CreateFor(typeof(T));
        }

        public virtual IStructureType CreateFor(Type type)
        {
            var config = Configurations.GetConfiguration(type);
            var reflecter = ReflecterFn(config);
            var shouldIndexAllMembers = config.IndexConfigIsEmpty;

            if (shouldIndexAllMembers)
                return new StructureType(
                    type,
                    reflecter.GetIdProperty(),
                    reflecter.GetConcurrencyTokenProperty(),
                    reflecter.GetTimeStampProperty(),
                    reflecter.GetIndexableProperties(),
                    reflecter.GetContainedStructureProperties());

            var shouldIndexAllMembersExcept = config.MemberPathsNotBeingIndexed.Count > 0;
            return new StructureType(
                type,
                reflecter.GetIdProperty(),
                reflecter.GetConcurrencyTokenProperty(),
                reflecter.GetTimeStampProperty(),
                (shouldIndexAllMembersExcept
                    ? reflecter.GetIndexablePropertiesExcept(config.MemberPathsNotBeingIndexed)
                    : reflecter.GetSpecificIndexableProperties(config.MemberPathsBeingIndexed)),
                reflecter.GetContainedStructureProperties());
        }
    }
}