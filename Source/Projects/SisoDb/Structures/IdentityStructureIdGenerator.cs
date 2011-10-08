using System;
using System.Collections.Generic;
using EnsureThat;
using PineCone.Structures;
using PineCone.Structures.Schemas;
using SisoDb.Dac;

namespace SisoDb.Structures
{
    public class IdentityStructureIdGenerator : IStructureIdGenerator
    {
        private static readonly Type LongType = typeof (long);

        private readonly IDbClient _dbClient;

        public IdentityStructureIdGenerator(IDbClient dbClient)
        {
            Ensure.That(() => dbClient).IsNotNull();

            _dbClient = dbClient;
        }

        public IStructureId CreateId(IStructureSchema structureSchema)
        {
            var nextId = _dbClient.CheckOutAndGetNextIdentity(structureSchema.Hash, 1);

            return StructureId.Create(nextId, LongType);
        }

        public IEnumerable<IStructureId> CreateIds(int numOfIds, IStructureSchema structureSchema)
        {
            var nextId = _dbClient.CheckOutAndGetNextIdentity(structureSchema.Hash, numOfIds);

            for (var c = 0; c < numOfIds; c++)
                yield return StructureId.Create((nextId + c), LongType);
        }
    }
}