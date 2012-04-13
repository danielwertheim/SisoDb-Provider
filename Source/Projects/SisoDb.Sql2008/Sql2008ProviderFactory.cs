using System;
using System.Data;
using EnsureThat;
using PineCone.Structures.Schemas;
using SisoDb.Dac;
using SisoDb.Dac.BulkInserts;
using SisoDb.DbSchema;
using SisoDb.Querying;
using SisoDb.Querying.Lambdas.Parsers;
using SisoDb.Sql2008.Dac;
using SisoDb.Structures;

namespace SisoDb.Sql2008
{
    public class Sql2008ProviderFactory : IDbProviderFactory
    {
        private IConnectionManager _connectionManager;
        private readonly ISqlStatements _sqlStatements;

        public Sql2008ProviderFactory()
        {
            _connectionManager = new ConnectionManager(GetAdoDriver());
            _sqlStatements = new Sql2008Statements();
        }

        public StorageProviders ProviderType
        {
            get { return StorageProviders.Sql2008; }
        }

        public IConnectionManager ConnectionManager
        {
            get { return _connectionManager; }
            set
            {
                Ensure.That(value, "ConnectionManager").IsNotNull();
                _connectionManager = value;
            }
        }

        public virtual IAdoDriver GetAdoDriver()
        {
            return new AdoDriver();
        }

        public virtual IDbSettings GetSettings()
        {
            return DbSettings.CreateDefault();
        }

        public virtual ISqlStatements GetSqlStatements()
        {
            return _sqlStatements;
        }

        public virtual IServerClient GetServerClient(ISisoConnectionInfo connectionInfo)
        {
            return new DbServerClient(GetAdoDriver(), connectionInfo, _connectionManager, _sqlStatements);
        }

        public virtual ITransactionalDbClient GetTransactionalDbClient(ISisoConnectionInfo connectionInfo)
        {
            var connection = _connectionManager.OpenClientDbConnection(connectionInfo);
            var transaction = Transactions.ActiveTransactionExists ? null : connection.BeginTransaction(IsolationLevel.ReadCommitted);

            return new Sql2008DbClient(
                GetAdoDriver(),
                connectionInfo,
                connection,
                transaction,
                _connectionManager,
                _sqlStatements);
        }

        public virtual IDbClient GetNonTransactionalDbClient(ISisoConnectionInfo connectionInfo)
        {
            IDbConnection connection = null;
            if (Transactions.ActiveTransactionExists)
                Transactions.SuppressOngoingTransactionWhile(() => connection = _connectionManager.OpenClientDbConnection(connectionInfo));
            else
                connection = _connectionManager.OpenClientDbConnection(connectionInfo);

            return new Sql2008DbClient(
                GetAdoDriver(), 
                connectionInfo,
                connection,
                null,
                _connectionManager,
                _sqlStatements);
        }

        public virtual IDbSchemaManager GetDbSchemaManagerFor(ISisoDatabase db)
        {
            return new DbSchemaManager(new SqlDbSchemaUpserter(db, _sqlStatements));
        }

        public virtual IStructureInserter GetStructureInserter(IDbClient dbClient)
        {
            return new DbStructureInserter(dbClient);
        }

        public IIdentityStructureIdGenerator GetIdentityStructureIdGenerator(CheckOutAngGetNextIdentity action)
        {
            return new DbIdentityStructureIdGenerator(action);
        }

        public virtual IDbQueryGenerator GetDbQueryGenerator()
        {
            return new Sql2008QueryGenerator(_sqlStatements);
        }

        public virtual IQueryBuilder GetQueryBuilder(Type structureType, IStructureSchemas structureSchemas)
        {
            return new QueryBuilder(structureType, structureSchemas, new ExpressionParsers());
        }

        public virtual IQueryBuilder<T> GetQueryBuilder<T>(IStructureSchemas structureSchemas) where T : class
        {
            return new QueryBuilder<T>(structureSchemas, new ExpressionParsers());
        }

        public virtual INamedQueryGenerator<T> GetNamedQueryGenerator<T>(IStructureSchemas structureSchemas) where T : class
        {
            return new NamedQueryGenerator<T>(GetQueryBuilder<T>(structureSchemas), GetDbQueryGenerator(), new DbDataTypeTranslator());
        }
    }
}