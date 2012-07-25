using System;
using SisoDb.PineCone.Structures.Schemas.Configuration;

namespace SisoDb.PineCone.Structures.Schemas
{
    public class StructureTypeFactory : IStructureTypeFactory
    {
        public Func<Type, IStructureTypeReflecter> ReflecterFn { get; set; }

        public IStructureTypeConfigurations Configurations { get; set; }

        public StructureTypeFactory(Func<Type, IStructureTypeReflecter> reflecterFn = null, IStructureTypeConfigurations configurations = null)
        {
            ReflecterFn = reflecterFn ?? (t => new StructureTypeReflecter(t));
            Configurations = configurations ?? new StructureTypeConfigurations();
        }

        public virtual IStructureType CreateFor<T>() where T : class 
        {
            return CreateFor(typeof(T));
        }

        public virtual IStructureType CreateFor(Type type)
        {
            var reflecter = ReflecterFn(type);
            var config = Configurations.GetConfiguration(type);
            var shouldIndexAllMembers = config == null || config.IsEmpty;

            if (shouldIndexAllMembers)
                return new StructureType(
                    type,
                    reflecter.GetIdProperty(),
                    reflecter.GetConcurrencyTokenProperty(),
                    reflecter.GetTimeStampProperty(),
                    reflecter.GetIndexableProperties());

            var shouldIndexAllMembersExcept = config.MemberPathsNotBeingIndexed.Count > 0;
            return new StructureType(
                type,
                reflecter.GetIdProperty(),
                reflecter.GetConcurrencyTokenProperty(),
                reflecter.GetTimeStampProperty(),
                (shouldIndexAllMembersExcept
                    ? reflecter.GetIndexablePropertiesExcept(config.MemberPathsNotBeingIndexed)
                    : reflecter.GetSpecificIndexableProperties(config.MemberPathsBeingIndexed)));
        }
    }
}