using SisoDb.Structures.Schemas;

namespace SisoDb.Providers.SqlProvider
{
    public interface IIdentityGenerator
    {
        int CheckOutAndGetSeed(IStructureSchema structureSchema, int numOfIds);
    }
}