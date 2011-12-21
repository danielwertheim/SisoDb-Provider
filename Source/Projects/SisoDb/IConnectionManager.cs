using System.Data;

namespace SisoDb
{
	public interface IConnectionManager
	{
		IDbConnection OpenServerConnection(IConnectionString connectionString);

		void ReleaseServerConnection(IDbConnection dbConnection);

		IDbConnection OpenDbConnection(IConnectionString connectionString);

		void ReleaseAllDbConnections();

		void ReleaseDbConnection(IDbConnection dbConnection);
	}
}