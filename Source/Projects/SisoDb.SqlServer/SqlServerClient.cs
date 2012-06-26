using System;
using System.Data;
using EnsureThat;
using NCore;
using SisoDb.Dac;
using SisoDb.DbSchema;
using SisoDb.Resources;

namespace SisoDb.SqlServer
{
    public class SqlServerClient : IServerClient
    {
        protected readonly IAdoDriver Driver;
        protected readonly ISisoConnectionInfo ConnectionInfo;
        protected readonly IConnectionManager ConnectionManager;
        protected readonly ISqlStatements SqlStatements;

		public SqlServerClient(IAdoDriver driver, ISisoConnectionInfo connectionInfo, IConnectionManager connectionManager, ISqlStatements sqlStatements)
		{
		    Ensure.That(driver, "driver").IsNotNull();
            Ensure.That(connectionInfo, "connectionInfo").IsNotNull();
			Ensure.That(connectionManager, "connectionManager").IsNotNull();
			Ensure.That(sqlStatements, "sqlStatements").IsNotNull();

		    Driver = driver;
            ConnectionInfo = connectionInfo;
        	ConnectionManager = connectionManager;
            SqlStatements = sqlStatements;
        }

        protected virtual void WithConnection(Action<IDbConnection> cnConsumer)
        {
            IDbConnection cn = null;

            try
            {
				cn = ConnectionManager.OpenServerConnection(ConnectionInfo);
                cnConsumer.Invoke(cn);
            }
            finally
            {
				ConnectionManager.ReleaseServerConnection(cn);
            }
        }

        protected virtual T WithConnection<T>(Func<IDbConnection, T> cnConsumer)
        {
            T result;
            IDbConnection cn = null;

            try
            {
				cn = ConnectionManager.OpenServerConnection(ConnectionInfo);
                result = cnConsumer.Invoke(cn);
            }
            finally
            {
				ConnectionManager.ReleaseServerConnection(cn);
            }

            return result;
        }

        public virtual void EnsureNewDb()
        {
			ConnectionManager.ReleaseAllConnections();

            WithConnection(cn =>
            {
                OnExecuteNonQuery(cn, SqlStatements.GetSql("DropDatabase").Inject(ConnectionInfo.DbName));
                OnExecuteNonQuery(cn, SqlStatements.GetSql("CreateDatabase").Inject(ConnectionInfo.DbName));
                OnInitializeSysTables(cn);
                OnInitializeSysTypes(cn);
            });
        }

        public virtual void CreateDbIfItDoesNotExist()
        {
			ConnectionManager.ReleaseAllConnections();

            WithConnection(cn =>
            {
                var exists = OnExecuteScalar<int>(cn, SqlStatements.GetSql("DatabaseExists"), new DacParameter(DbSchemas.Parameters.DbNameParamPrefix, ConnectionInfo.DbName)) > 0;

                if(exists)
                    return;
                
                OnExecuteNonQuery(cn, SqlStatements.GetSql("CreateDatabase").Inject(ConnectionInfo.DbName));
                OnInitializeSysTables(cn);
                OnInitializeSysTypes(cn);
            });
        }

        public virtual void InitializeExistingDb()
        {
			ConnectionManager.ReleaseAllConnections();

            WithConnection(cn =>
            {
                var exists = OnExecuteScalar<int>(cn, SqlStatements.GetSql("DatabaseExists"), new DacParameter(DbSchemas.Parameters.DbNameParamPrefix, ConnectionInfo.DbName)) > 0;

                if (!exists)
                    throw new SisoDbException(ExceptionMessages.SqlDatabase_InitializeExisting_DbDoesNotExist.Inject(ConnectionInfo.DbName));

                OnInitializeSysTables(cn);
                OnInitializeSysTypes(cn);
            });
        }

        protected virtual void OnInitializeSysTables(IDbConnection cn)
        {
            OnExecuteNonQuery(cn, SqlStatements.GetSql("Sys_Identities_CreateIfNotExists").Inject(ConnectionInfo.DbName));
        }

        protected virtual void OnInitializeSysTypes(IDbConnection cn)
        {
            OnExecuteNonQuery(cn, SqlStatements.GetSql("Sys_Types_CreateIfNotExists").Inject(ConnectionInfo.DbName));
        }

        public virtual bool DbExists()
        {
            return WithConnection(cn => OnExecuteScalar<int>(cn, SqlStatements.GetSql("DatabaseExists"), new DacParameter(DbSchemas.Parameters.DbNameParamPrefix, ConnectionInfo.DbName)) > 0);
        }

        public virtual void DropDbIfItExists()
        {
			ConnectionManager.ReleaseAllConnections();

            WithConnection(cn => OnExecuteNonQuery(cn, SqlStatements.GetSql("DropDatabase").Inject(ConnectionInfo.DbName)));
        }

        protected virtual T OnExecuteScalar<T>(IDbConnection connection, string sql, params IDacParameter[] parameters)
        {
            using (var cmd = CreateCommand(connection, sql, parameters))
            {
                var value = cmd.ExecuteScalar();

                if (value == null || value == DBNull.Value)
                    return default(T);

                return (T)Convert.ChangeType(value, typeof(T));
            }
        }

        protected virtual void OnExecuteNonQuery(IDbConnection connection, string sql, params IDacParameter[] parameters)
        {
            using (var cmd = CreateCommand(connection, sql, parameters))
            {
                cmd.ExecuteNonQuery();
            }
        }

        protected virtual IDbCommand CreateCommand(IDbConnection connection, string sql, params IDacParameter[] parameters)
        {
            return Driver.CreateCommand(connection, sql, null, parameters);
        }

        protected virtual IDbCommand CreateSpCommand(IDbConnection connection, string spName, params IDacParameter[] parameters)
        {
            return Driver.CreateSpCommand(connection, spName, null, parameters);
        }
    }
}