using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                var sourceBatch = sourceElements.Skip(batchCount * batchSize).Take(batchSize).ToArray();
                if (sourceBatch.Length < 1)
                    yield break;

                var structures = new IStructure[sourceBatch.Length];
                var consumedIds = (batchCount * _maxBatchSize);

                Parallel.For(0, sourceBatch.Length, 
                    i =>
                    {
                        var sourceItem = sourceBatch[i];

                        if (_identitySeed.HasValue)
                        {
                            var id = _identitySeed.Value + consumedIds + i;
                            _structureSchema.IdAccessor.SetValue(sourceItem, id);
                        }

                        structures[i] = _structureBuilder.CreateStructure(sourceItem, _structureSchema);
                    });

                yield return structures;

                batchCount++;
            }
        }
    }
}