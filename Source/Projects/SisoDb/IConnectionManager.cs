using System.Data;

namespace SisoDb
{
	public interface IConnectionManager
	{
		IDbConnection OpenServerConnection(ISisoConnectionInfo connectionInfo);
        IDbConnection OpenClientDbConnection(ISisoConnectionInfo connectionInfo);

	    void ReleaseAllDbConnections();
	    void ReleaseServerConnection(IDbConnection dbConnection);
        void ReleaseClientDbConnection(IDbConnection dbConnection);
	}
}