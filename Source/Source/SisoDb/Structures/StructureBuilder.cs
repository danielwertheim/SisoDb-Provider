using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SisoDb.Resources;
using SisoDb.Serialization;
using SisoDb.Structures.Schemas;

namespace SisoDb.Structures
{
    internal class StructureBuilder : IStructureBuilder
    {
        public IList<IStructure> CreateStructures<T>(IEnumerable<T> items, IStructureSchema structureSchema)
            where T : class
        {
            return items.Select(item => CreateStructure(item, structureSchema)).ToList();
        }

        public IStructure CreateStructure<T>(T item, IStructureSchema structureSchema)
            where T : class
        {
            var name = structureSchema.Name;
            var id = GetId(structureSchema, item);
            var indexes = GetIndexes(structureSchema, item, id);
            var json = JsonSerialization.ToJsonOrEmptyString(item);

            return new Structure(name, id, indexes, json);
        }

        private static IStructureId GetId<T>(IStructureSchema structureSchema, T item) where T : class
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
                throw new SisoDbException(ExceptionMessages.StructureBuilder_UnSupportedIdentityType.Inject(structureSchema.IdAccessor.IdType));
            return id;
        }

        private static Guid EnsureGuidIdValueExists<T>(T item, IStructureSchema structureSchema) 
            where T : class
        {
            var idValue = structureSchema.IdAccessor.GetValue<T, Guid>(item);
            var keyIsAssigned = idValue.HasValue && !Guid.Empty.Equals(idValue.Value);

            if (!keyIsAssigned)
            {
                idValue = Guid.NewGuid();
                structureSchema.IdAccessor.SetValue(item, idValue.Value);
            }

            return idValue.Value;
        }

        private static int EnsureIdentityValueExists<T>(T item, IStructureSchema structureSchema)
            where T : class 
        {
            var idValue = structureSchema.IdAccessor.GetValue<T, int>(item);
            if (!idValue.HasValue || idValue < 1)
                throw new SisoDbException(ExceptionMessages.StructureBuilder_MissingIdentityValue);

            return idValue.Value;
        }

        private static IEnumerable<IStructureIndex> GetIndexes<T>(IStructureSchema structureSchema, T item, IStructureId id)
            where T : class 
        {
            var indexes = new IStructureIndex[structureSchema.IndexAccessors.Count];
            for (var c = 0; c < indexes.Length; c++)
            {
                var indexAccessor = structureSchema.IndexAccessors[c];
                var values = indexAccessor.GetValues(item);
                if (values == null || values.Count < 1)
                {
                    if (indexAccessor.IsUnique)
                        throw new SisoDbException(ExceptionMessages.StructureBuilder_UniqueIndex_IsNull.Inject(indexAccessor.Path, indexAccessor.Name));

                    var index = new StructureIndex(id, indexAccessor.Name, null, indexAccessor.IsUnique);
                    indexes[c] = index;
                    continue;
                }

                if (values.Count > 1 || indexAccessor.IsEnumerable || indexAccessor.IsElement)
                {
                    var valueString = new StringBuilder();
                    foreach (var value in values.Distinct())
                    {
                        valueString.Append("<$");
                        valueString.Append(StringConverter.AsString(value));
                        valueString.Append("$>");
                    }
                    var index = new StructureIndex(id, indexAccessor.Name, valueString.ToString(), indexAccessor.IsUnique);
                    indexes[c] = index;
                }
                else
                {
                    var index = new StructureIndex(id, indexAccessor.Name, values[0], indexAccessor.IsUnique);
                    indexes[c] = index;
                }
            }

            return indexes;
        }
    }
}