namespace SisoDb.Dac
{
    /// <summary>
    /// Defines operations that Siso needs to perform on a server-level. For db-level
    /// operations, see <see cref="IDbClient"/>.
    /// </summary>
    public interface IServerClient
    {
        void EnsureNewDb();
        void CreateDbIfItDoesNotExist();
        void InitializeExistingDb();
        bool DbExists();
        void DropDbIfItExists();
    }
}