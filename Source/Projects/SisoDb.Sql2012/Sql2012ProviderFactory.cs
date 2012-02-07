﻿using System;
using PineCone.Structures.Schemas;
using SisoDb.Dac;
using SisoDb.Dac.BulkInserts;
using SisoDb.DbSchema;
using SisoDb.Querying;
using SisoDb.Querying.Lambdas.Parsers;
using SisoDb.Sql2012.Dac;
using SisoDb.Structures;

namespace SisoDb.Sql2012
{
	public class Sql2012ProviderFactory : IDbProviderFactory
    {
		private readonly IConnectionManager _connectionManager;
        private readonly ISqlStatements _sqlStatements;
		
        public Sql2012ProviderFactory()
        {
			_connectionManager = new Sql2012ConnectionManager();
            _sqlStatements = new Sql2012Statements();
        }

        public StorageProviders ProviderType
        {
            get { return StorageProviders.Sql2012; }
        }

		public ISqlStatements GetSqlStatements()
		{
			return _sqlStatements;
		}

		public virtual IServerClient GetServerClient(ISisoConnectionInfo connectionInfo)
        {
            return new Sql2012ServerClient(connectionInfo, _connectionManager, _sqlStatements);
        }

        public ISisoTransaction GetRequiredTransaction()
        {
            return Sql2012DbTransaction.CreateRequired();
        }

        public ISisoTransaction GetSuppressedTransaction()
        {
            return Sql2012DbTransaction.CreateSuppressed();
        }

	    public IDbClient GetDbClient(ISisoConnectionInfo connectionInfo)
        {
            return new Sql2012DbClient(connectionInfo, _connectionManager, _sqlStatements);
        }

        public virtual IDbSchemaManager GetDbSchemaManager()
        {
			return new DbSchemaManager(new SqlDbSchemaUpserter(_sqlStatements));
        }

        public virtual IStructureInserter GetStructureInserter(IDbClient dbClient)
        {
            return new DbStructureInserter(dbClient, () => GetDbClient(dbClient.ConnectionInfo));
        }

    	public IIdentityStructureIdGenerator GetIdentityStructureIdGenerator(CheckOutAngGetNextIdentity action)
    	{
    		return new DbIdentityStructureIdGenerator(action);
    	}

    	public virtual IDbQueryGenerator GetDbQueryGenerator()
        {
            return new Sql2012QueryGenerator(_sqlStatements);
        }

    	public IQueryBuilder<T> GetQueryBuilder<T>(IStructureSchemas structureSchemas) where T : class
    	{
    		return new QueryBuilder<T>(structureSchemas, new ExpressionParsers());
    	}
    }
}