using System.Data;

namespace SisoDb
{
    /// <summary>
    /// This exists because SqlBulkCopy depends on SqlConnection and that class is sealed
    /// so we can't derive from it to add profiling
    /// </summary>
    public interface IWrappedConnection
    {
        IDbConnection GetInnerConnection();
    }
}
