using System.Collections.Generic;
using System.Linq;
using System.Text;
using SisoDb.Core;
using SisoDb.Resources;
using SisoDb.Structures.Schemas;

namespace SisoDb.Structures
{
    public class StructureIndexesFactory : IStructureIndexesFactory
    {
        public IStringConverter StringConverter { private get; set; }

        public StructureIndexesFactory(IStringConverter stringConverter)
        {
            StringConverter = stringConverter.AssertNotNull("stringConverter");
        }

        public IEnumerable<IStructureIndex> GetIndexes<T>(IStructureSchema structureSchema, T item, ISisoId id)
            where T : class
        {
            var indexes = new IStructureIndex[structureSchema.IndexAccessors.Count];
            //TODO: Parallel
            for (var c = 0; c < indexes.Length; c++)
            {
                var indexAccessor = structureSchema.IndexAccessors[c];
                var values = indexAccessor.GetValues(item);
                if (values == null || values.Count < 1)
                {
                    if (indexAccessor.IsUnique)
                        throw new SisoDbException(ExceptionMessages.StructureIndexesFactory_UniqueIndex_IsNull.Inject(indexAccessor.Path, indexAccessor.Name));

                    indexes[c] = new StructureIndex(id, indexAccessor.Name, null, indexAccessor.IsUnique);
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
                    indexes[c] = new StructureIndex(id, indexAccessor.Name, valueString.ToString(), indexAccessor.IsUnique);
                }
                else
                    indexes[c] = new StructureIndex(id, indexAccessor.Name, values[0], indexAccessor.IsUnique);
            }

            return indexes;
        }
    }
}