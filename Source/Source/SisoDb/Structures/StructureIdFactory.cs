using System;
using SisoDb.Core;
using SisoDb.Resources;
using SisoDb.Structures.Schemas;

namespace SisoDb.Structures
{
    public class StructureIdFactory : IStructureIdFactory
    {
        public IStructureId GetId<T>(IStructureSchema structureSchema, T item) where T : class
        {
            IStructureId id;
            if (structureSchema.IdAccessor.IdType == IdTypes.Guid)
            {
                var idValue = EnsureGuidIdValueExists(item, structureSchema);
                id = StructureId.NewGuidId(idValue);
            }
            else if (structureSchema.IdAccessor.IdType == IdTypes.Identity)
            {
                var idValue = EnsureIdentityValueExists(item, structureSchema);
                id = StructureId.NewIdentityId(idValue);
            }
            else
                throw new SisoDbException(ExceptionMessages.StructureIdFactory_UnSupportedIdentityType.Inject(structureSchema.IdAccessor.IdType));

            return id;
        }

        private static Guid EnsureGuidIdValueExists<T>(T item, IStructureSchema structureSchema)
            where T : class
        {
            var idValue = structureSchema.IdAccessor.GetValue<T, Guid>(item);
            var keyIsAssigned = idValue.HasValue && !Guid.Empty.Equals(idValue.Value);

            if (!keyIsAssigned)
            {
                idValue = SequentialGuid.NewSqlCompatibleGuid(); //Guid.NewGuid();
                structureSchema.IdAccessor.SetValue(item, idValue.Value);
            }

            return idValue.Value;
        }

        private static int EnsureIdentityValueExists<T>(T item, IStructureSchema structureSchema)
            where T : class
        {
            var idValue = structureSchema.IdAccessor.GetValue<T, int>(item);
            if (!idValue.HasValue || idValue < 1)
                throw new SisoDbException(ExceptionMessages.StructureIdFactory_MissingIdentityValue);

            return idValue.Value;
        }
    }
}