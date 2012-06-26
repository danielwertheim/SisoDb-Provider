using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlServerCe;
using System.Linq;
using SisoDb.Dac;
using SisoDb.Resources;
using SisoDb.SqlServer;

namespace SisoDb.SqlCe4
{
    public class SqlCe4ConnectionManager : SqlServerConnectionManager
    {
        private readonly ConcurrentDictionary<string, IDbConnection> _warmupConnections;

        public SqlCe4ConnectionManager(IAdoDriver driver) : base(driver)
        {
            _warmupConnections = new ConcurrentDictionary<string, IDbConnection>();

            AppDomain.CurrentDomain.DomainUnload += (sender, args) => ReleaseAllConnections();
        }

        public override IDbConnection OpenServerConnection(ISisoConnectionInfo connectionInfo)
        {
            EnsureWarmedUp((SqlCe4ConnectionInfo)connectionInfo);

            return base.OpenServerConnection(connectionInfo);
        }

        public override IDbConnection OpenClientConnection(ISisoConnectionInfo connectionInfo)
        {
            EnsureWarmedUp((SqlCe4ConnectionInfo)connectionInfo);

            return base.OpenClientConnection(connectionInfo);
        }

        public override void ReleaseAllConnections()
        {
            if(!_warmupConnections.Any())
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
                throw new SisoDbException(ExceptionMessages.SqlCe4ConnectionManager_ReleaseAllDbConnections, exceptions);
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