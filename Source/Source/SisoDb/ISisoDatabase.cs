using System;
using SisoDb.Structures;
using SisoDb.Structures.Schemas;

namespace SisoDb
{
    public interface ISisoDatabase
    {
        string Name { get; }

        ISisoConnectionInfo ServerConnectionInfo { get; }

        ISisoConnectionInfo ConnectionInfo { get; }

        IStructureSchemas StructureSchemas { get; set; }

        IStructureBuilder StructureBuilder { get; set; }

        void EnsureNewDatabase();

        void CreateIfNotExists();

        void InitializeExisting();

        void DeleteIfExists();

        bool Exists();

        void DropStructureSet<T>() where T : class;

        void UpsertStructureSet<T>() where T : class;

        void UpdateStructureSet<TOld, TNew>(Func<TOld, TNew, StructureSetUpdaterStatuses> onProcess)
            where TOld : class
            where TNew : class;
        
        IUnitOfWork CreateUnitOfWork();
    }
}