using SisoDb.Structures.Schemas;

namespace SisoDb.Providers
{
    public interface IIdentityGenerator
    {
        int CheckOutAndGetSeed(IStructureSchema structureSchema, int numOfIds);
    }
}