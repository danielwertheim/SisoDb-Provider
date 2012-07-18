using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PineCone.Resources;
using PineCone.Structures.Schemas;
using SisoDb.NCore;

namespace PineCone.Structures
{
    public class StructureIndexesFactory : IStructureIndexesFactory
    {
        public IStructureIndex[] CreateIndexes<T>(IStructureSchema structureSchema, T item, IStructureId structureId) where T : class
        {
            var indexes = new IEnumerable<IStructureIndex>[structureSchema.IndexAccessors.Count];

            Parallel.For(0, indexes.Length, c =>
            {
                var indexAccessor = structureSchema.IndexAccessors[c];
                var values = indexAccessor.GetValues(item);
                var valuesExists = values != null && values.Count > 0;
                var isCollectionOfValues = indexAccessor.IsEnumerable || indexAccessor.IsElement || (values != null && values.Count > 1);

                if (!valuesExists)
                {
                    if (indexAccessor.IsUnique)
                        throw new PineConeException(ExceptionMessages.StructureIndexesFactory_UniqueIndex_IsNull.Inject(structureSchema.Name, indexAccessor.Path));

                    return;
                }

                if (!isCollectionOfValues)
                    indexes[c] = new[] { new StructureIndex(structureId, indexAccessor.Path, values[0], indexAccessor.DataType, indexAccessor.DataTypeCode, indexAccessor.UniqueMode.ToStructureIndexType()) };
                else
                {
                    var subIndexes = new IStructureIndex[values.Count];
                    Parallel.For(0, subIndexes.Length, subC =>
                    {
                        if (values[subC] != null)
                            subIndexes[subC] = new StructureIndex(structureId, indexAccessor.Path, values[subC],
                                                                  indexAccessor.DataType, indexAccessor.DataTypeCode,
                                                                  indexAccessor.UniqueMode.ToStructureIndexType());
                    });
                    indexes[c] = subIndexes;
                }
            });

            return indexes.Where(i => i != null).SelectMany(i => i).Where(i => i != null).ToArray();
        }
    }
}