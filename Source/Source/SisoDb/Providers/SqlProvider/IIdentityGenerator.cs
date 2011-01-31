using SisoDb.Structures.Schemas;

namespace SisoDb.Providers.SqlProvider
{
    internal interface IIdentityGenerator
    {
        int CheckOutAndGetSeed(IStructureSchema structureSchema, int numOfIds);
    }
}