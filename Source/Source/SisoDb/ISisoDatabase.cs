using SisoDb.Structures.Schemas;

namespace SisoDb
{
    public interface ISisoDatabase
    {
        string Name { get; }

        ISisoConnectionInfo ServerConnectionInfo { get; }

        ISisoConnectionInfo ConnectionInfo { get; }

        IStructureSchemas StructureSchemas { get; }

        void EnsureNewDatabase();

        void CreateIfNotExists();

        void InitializeExisting();

        void DeleteIfExists();

        bool Exists();

        void DropStructureSet<T>() where T : class;

        void UpsertStructureSet<T>() where T : class;
        
        IUnitOfWork CreateUnitOfWork();
    }
}