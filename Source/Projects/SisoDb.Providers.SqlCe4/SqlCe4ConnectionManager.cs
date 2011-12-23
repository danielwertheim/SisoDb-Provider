using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlServerCe;

namespace SisoDb.SqlCe4
{
	public class SqlCe4ConnectionManager : IConnectionManager
	{
		private readonly ConcurrentDictionary<string, BlockingCollection<IDbConnection>> _clientConnections;

		public SqlCe4ConnectionManager()
		{
			_clientConnections = new ConcurrentDictionary<string, BlockingCollection<IDbConnection>>();
		}

		~SqlCe4ConnectionManager()
		{
			ReleaseAllDbConnections();
		}

		public IDbConnection OpenServerConnection(IConnectionString connectionString)
		{
			var cn = new SqlCeConnection(connectionString.PlainString);
			cn.Open();

			return cn;
		}

		public void ReleaseServerConnection(IDbConnection dbConnection)
		{
			if (dbConnection == null)
				return;

			if (dbConnection.State != ConnectionState.Closed)
				dbConnection.Close();

			dbConnection.Dispose();
		}

		public IDbConnection OpenDbConnection(IConnectionString connectionString)
		{
			IDbConnection cn;

			var connectionsPerCnString = GetConnectionsForConnectionString(connectionString.PlainString);
			if (connectionsPerCnString.TryTake(out cn))
				return cn;

			cn = new SqlCeConnection(connectionString.PlainString);
			cn.Open();
			return cn;
		}

		public void ReleaseAllDbConnections()
		{
			var exceptions = new List<Exception>();

			foreach (var key in _clientConnections.Keys)
			{
				try
				{
					BlockingCollection<IDbConnection> connections;
					if (_clientConnections.TryRemove(key, out connections))
					{
						while (connections.Count > 0)
						{
							try
							{
								IDbConnection cn;
								if (connections.TryTake(out cn) && cn != null)
								{
									if (cn.State != ConnectionState.Closed)
										cn.Close();

									cn.Dispose();
								}
							}
							catch (Exception ex)
							{
								exceptions.Add(ex);
							}
						}
					}
				}
				catch (Exception ex)
				{
					exceptions.Add(ex);
				}
			}

			_clientConnections.Clear();

			if (exceptions.Count > 0)
				throw new SisoDbException("Exceptions occured while releasing SqlCe4Connections from the pool.", exceptions);
		}

		public void ReleaseDbConnection(IDbConnection dbConnection)
		{
			var connectionsPerCnString = GetConnectionsForConnectionString(dbConnection.ConnectionString);
			if (!connectionsPerCnString.TryAdd(dbConnection))
			{
				dbConnection.Close();
				dbConnection.Dispose();
			}
		}

		private BlockingCollection<IDbConnection> GetConnectionsForConnectionString(string connectionString)
		{
			return _clientConnections.GetOrAdd(connectionString, new BlockingCollection<IDbConnection>());
		}
	}
}