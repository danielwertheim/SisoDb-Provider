namespace SisoDb.Dac
{
    public interface IServerClient
    {
        void EnsureNewDb();
        void CreateDbIfDoesNotExists();
        void InitializeExistingDb();
        bool DbExists();
        void DropDbIfItExists();
    }
}