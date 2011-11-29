using PineCone.Structures.Schemas;

namespace SisoDb.Dac
{
    public delegate long GetNextIdentity(IStructureSchema structureSchema, int numOfIds);
}