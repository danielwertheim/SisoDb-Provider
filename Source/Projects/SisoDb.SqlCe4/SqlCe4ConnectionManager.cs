using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlServerCe;

namespace SisoDb.SqlCe4
{
    public class SqlCe4ConnectionManager : IConnectionManager
    {
        private readonly ConcurrentDictionary<string, IDbConnection> _warmupConnections;

        public SqlCe4ConnectionManager()
        {
            _warmupConnections = new ConcurrentDictionary<string, IDbConnection>();
            AppDomain.CurrentDomain.DomainUnload += (sender, args) => ReleaseAllDbConnections();
        }

        public virtual IDbConnection OpenServerConnection(ISisoConnectionInfo connectionInfo)
        {
            EnsureWarmedUp((SqlCe4ConnectionInfo)connectionInfo);
            
            var cn = new SqlCeConnection(connectionInfo.ServerConnectionString.PlainString);
            cn.Open();

            return cn;
        }

        public virtual IDbConnection OpenClientDbConnection(ISisoConnectionInfo connectionInfo)
        {
            EnsureWarmedUp((SqlCe4ConnectionInfo)connectionInfo);

            var cn = new SqlCeConnection(connectionInfo.ClientConnectionString.PlainString);
            cn.Open();

            return cn;
        }

        public virtual void ReleaseAllDbConnections()
        {
            if(_warmupConnections.Count < 1)
                return;

            var exceptions = new List<Exception>();

            foreach (var key in _warmupConnections.Keys)
            {
                try
                {
                    IDbConnection connection;
                    if (_warmupConnections.TryRemove(key, out connection))
                    {
                        try
                        {
                           if(connection != null)
                           {
                               connection.Close();
                               connection.Dispose();
                           }
                        }
                        catch (Exception ex)
                        {
                            exceptions.Add(ex);
                        }
                    }
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }

            _warmupConnections.Clear();

            if (exceptions.Count > 0)
                throw new SisoDbException("Exceptions occured while releasing SqlCe4Connections from the pool.", exceptions);
        }

        public virtual void ReleaseServerConnection(IDbConnection dbConnection)
        {
            if (dbConnection == null)
                return;

            dbConnection.Close();
            dbConnection.Dispose();
        }

        public virtual void ReleaseClientDbConnection(IDbConnection dbConnection)
        {
            if (dbConnection == null)
                return;

            dbConnection.Close();
            dbConnection.Dispose();
        }

        private void EnsureWarmedUp(SqlCe4ConnectionInfo connectionInfo)
        {
            if(_warmupConnections.ContainsKey(connectionInfo.FilePath))
                return;

            var cn = new SqlCeConnection(connectionInfo.ServerConnectionString.PlainString);
            if(!_warmupConnections.TryAdd(connectionInfo.FilePath, cn))
            {
                cn.Close();
                cn.Dispose();
            }
        }
    }
}