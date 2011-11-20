namespace SisoDb.Dac
{
    public interface IServerClient
    {
        void EnsureNewDb();
        void CreateDbIfItDoesNotExist();
        void InitializeExistingDb();
        bool DbExists();
        void DropDbIfItExists();
    }
}