using System;
using PineCone.Structures.Schemas;
using SisoDb.Dac;

namespace SisoDb.DbSchema
{
    public interface IDbSchemaUpserter
    {
        void Upsert(IStructureSchema structureSchema, Func<IDbClient> dbClientFn);
    }
}