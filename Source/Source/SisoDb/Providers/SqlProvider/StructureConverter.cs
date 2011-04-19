using System.Collections.Generic;
using System.Linq;
using SisoDb.Structures;
using SisoDb.Structures.Schemas;

namespace SisoDb.Providers.SqlProvider
{
    public class StructureConverter
    {
        private readonly IStructureSchema _structureSchema;

        private readonly IStructureBuilder _structureBuilder;

        private readonly int _maxBatchSize;

        private readonly int? _identitySeed;

        public StructureConverter(IStructureSchema structureSchema, IStructureBuilder structureBuilder, int maxBatchSize, int? identitySeed)
        {
            _structureSchema = structureSchema;
            _structureBuilder = structureBuilder;
            _maxBatchSize = maxBatchSize;
            _identitySeed = identitySeed;
        }

        public IEnumerable<IStructure[]> Convert<T>(IEnumerable<T> sourceElements) where T : class
        {
            var sourceCount = sourceElements.Count();
            var batchSize = sourceCount > _maxBatchSize ? _maxBatchSize : sourceCount;

            var batchCount = 0;
            while (true)
            {
                //TODO: Pointer instead?
                var sourceBatch = sourceElements.Skip(batchCount * batchSize).Take(batchSize);
                if (sourceBatch.Count() < 1)
                    yield break;

                var structures = new IStructure[sourceBatch.Count()];
                var c = 0;
                var consumedIds = (batchCount * _maxBatchSize);
                foreach (var sourceItem in sourceBatch)
                {
                    if (_identitySeed.HasValue)
                    {
                        var id = _identitySeed.Value + consumedIds + c;
                        _structureSchema.IdAccessor.SetValue(sourceItem, id);
                    }

                    structures[c] = _structureBuilder.CreateStructure(sourceItem, _structureSchema);
                    c++;
                }

                yield return structures;

                batchCount++;
            }
        }
    }
}