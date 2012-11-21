using SisoDb.Structures;

namespace SisoDb.Dac
{
    public interface IDefaultUniquesChecksumGenerator 
    {
        string Generate(IStructureIndex structureIndex);
    }
}