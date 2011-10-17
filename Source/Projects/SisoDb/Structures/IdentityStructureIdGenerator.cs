using System.Collections.Generic;
using EnsureThat;
using PineCone.Structures;
using PineCone.Structures.Schemas;
using SisoDb.Dac;

namespace SisoDb.Structures
{
    public class IdentityStructureIdGenerator : IStructureIdGenerator
    {
        private readonly IDbClient _dbClient;

        public IdentityStructureIdGenerator(IDbClient dbClient)
        {
            Ensure.That(dbClient, "dbClient").IsNotNull();

            _dbClient = dbClient;
        }

        public IStructureId CreateId(IStructureSchema structureSchema)
        {
            var nextId = _dbClient.CheckOutAndGetNextIdentity(structureSchema.Hash, 1);

            if (structureSchema.IdAccessor.IdType == StructureIdTypes.Identity)
                return StructureId.Create((int)nextId);

            return StructureId.Create(nextId);
        }

        public IEnumerable<IStructureId> CreateIds(int numOfIds, IStructureSchema structureSchema)
        {
            var nextId = _dbClient.CheckOutAndGetNextIdentity(structureSchema.Hash, numOfIds);

            if (structureSchema.IdAccessor.IdType == StructureIdTypes.Identity)
                return CreateIdentityIds(numOfIds, (int)nextId);

            return CreateBigIdentityIds(numOfIds, nextId);
        }

        private IEnumerable<IStructureId> CreateIdentityIds(int numOfIds, int startId)
        {
            for (var c = 0; c < numOfIds; c++)
                yield return StructureId.Create((startId + c));
        }

        private IEnumerable<IStructureId> CreateBigIdentityIds(int numOfIds, long startId)
        {
            for (var c = 0; c < numOfIds; c++)
                yield return StructureId.Create((startId + c));
        }
    }
}