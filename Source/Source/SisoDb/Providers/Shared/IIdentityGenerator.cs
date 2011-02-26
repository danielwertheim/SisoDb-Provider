using SisoDb.Structures.Schemas;

namespace SisoDb.Providers.Shared
{
    public interface IIdentityGenerator
    {
        int CheckOutAndGetSeed(IStructureSchema structureSchema, int numOfIds);
    }
}