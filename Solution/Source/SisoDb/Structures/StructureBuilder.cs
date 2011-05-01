using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SisoDb.Core;
using SisoDb.Serialization;
using SisoDb.Structures.Schemas;

namespace SisoDb.Structures
{
    public class StructureBuilder : IStructureBuilder
    {
        public IJsonSerializer JsonSerializer { get; private set; }

        public ISisoIdFactory SisoIdFactory { get; private set; }

        public IStructureIndexesFactory IndexesFactory { get; private set; }

        public StructureBuilder(IJsonSerializer jsonSerializer, ISisoIdFactory sisoIdFactory, IStructureIndexesFactory structureIndexesFactory)
        {
            JsonSerializer = jsonSerializer.AssertNotNull("jsonSerializer");
            SisoIdFactory = sisoIdFactory.AssertNotNull("sisoIdFactory");
            IndexesFactory = structureIndexesFactory.AssertNotNull("structureIndexesFactory");
        }

        public IStructure CreateStructure<T>(T item, IStructureSchema structureSchema)
            where T : class
        {
            return CreateStructure(
                item,
                structureSchema,
                SisoIdFactory.GetId(structureSchema.IdAccessor, item));
        }

        public IEnumerable<IStructure[]> CreateBatchedGuidStructures<T>(IEnumerable<T> items, IStructureSchema structureSchema, int maxBatchSize) where T : class
        {
            return CreateBatchedStructures(items, structureSchema, maxBatchSize, null);
        }

        public IEnumerable<IStructure[]> CreateBatchedIdentityStructures<T>(IEnumerable<T> items, IStructureSchema structureSchema, int maxBatchSize, int identitySeed) where T : class
        {
            return CreateBatchedStructures(items, structureSchema, maxBatchSize, identitySeed);
        }

        private IEnumerable<IStructure[]> CreateBatchedStructures<T>(IEnumerable<T> items, IStructureSchema structureSchema, int maxBatchSize, int? identitySeed) where T : class
        {
            var batchSize = items.Count() > maxBatchSize ? maxBatchSize : items.Count();

            Func<ValueType, ISisoId> idCreator;

            if (identitySeed.HasValue)
                idCreator = id => SisoId.NewIdentityId((int)id);
            else
                idCreator = id => SisoId.NewGuidId((Guid)id);

            var batchNo = 0;
            while (true)
            {
                var sourceBatch = items.Skip(batchNo * batchSize).Take(batchSize).ToArray();
                if (sourceBatch.Length < 1)
                    yield break;

                var structures = new IStructure[sourceBatch.Length];
                var ids = (!identitySeed.HasValue)
                    ? SisoIdValueGenerator.CreateGuidIds(sourceBatch.Length)
                    : SisoIdValueGenerator.CreateIdentityIds(CalculateIdentitySeed(identitySeed.Value, batchNo, batchSize), sourceBatch.Length);

                Parallel.For(0, sourceBatch.Length,
                    i =>
                    {
                        var sourceItem = sourceBatch[i];

                        structureSchema.IdAccessor.SetValue(sourceItem, ids[i]);

                        structures[i] = CreateStructure(sourceItem, structureSchema, idCreator(ids[i]));
                    });

                yield return structures;

                batchNo++;
            }
        }

        internal static int CalculateIdentitySeed(int originalSeed, int batchNo, int batchSize)
        {
            var consumedIds = (batchNo * batchSize);
            return originalSeed + consumedIds;
        }

        private IStructure CreateStructure<T>(T item, IStructureSchema structureSchema, ISisoId sisoId)
            where T : class
        {
            return new Structure(
                structureSchema.Name,
                sisoId,
                IndexesFactory.GetIndexes(structureSchema, item, sisoId),
                JsonSerializer.ToJsonOrEmptyString(item));
        }
    }
}