namespace SisoDb.Structures
{
    public interface IStructureBuilders 
    {
        StructureBuilderFactoryForInserts ForInserts { get; set; }
        StructureBuilderFactoryForUpdates ForUpdates { get; set; }
        void Reset();
    }
}