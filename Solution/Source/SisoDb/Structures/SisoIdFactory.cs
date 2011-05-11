using System;
using SisoDb.Core;
using SisoDb.Resources;
using SisoDb.Structures.Schemas.MemberAccessors;

namespace SisoDb.Structures
{
    public class SisoIdFactory : ISisoIdFactory
    {
        public ISisoId GetId<T>(IIdAccessor idAccessor, T item)
            where T : class
        {
            if (idAccessor.IdType == IdTypes.Guid)
                return SisoId.NewGuidId(
                    EnsureGuidIdValueExists(idAccessor, item));

            if (idAccessor.IdType == IdTypes.Identity)
                return SisoId.NewIdentityId(
                    EnsureIdentityValueExists(idAccessor, item));
            
            throw new SisoDbException(
                ExceptionMessages.SisoIdFactory_UnSupportedIdentityType.Inject(idAccessor.IdType));
        }

        private static Guid EnsureGuidIdValueExists<T>(IIdAccessor idAccessor, T item)
            where T : class
        {
            var idValue = idAccessor.GetValue<T, Guid>(item);
            
            var idIsAssigned = idValue.HasValue && !Guid.Empty.Equals(idValue.Value);

            if (!idIsAssigned)
                throw new SisoDbException(ExceptionMessages.SisoIdFactory_MissingGuidValue);

            return idValue.Value;
        }

        private static int EnsureIdentityValueExists<T>(IIdAccessor idAccessor, T item)
            where T : class
        {
            var idValue = idAccessor.GetValue<T, int>(item);
            
            var idIsAssigned = idValue.HasValue && idValue.Value > 0;
            
            if (!idIsAssigned)
                throw new SisoDbException(ExceptionMessages.SisoIdFactory_MissingIdentityValue);

            return idValue.Value;
        }
    }
}