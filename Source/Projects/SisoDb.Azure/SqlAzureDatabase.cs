using SisoDb.Resources;

namespace SisoDb.Azure
{
    public class SqlAzureDatabase : SisoDatabase
    {
    	public SqlAzureDatabase(ISisoConnectionInfo connectionInfo, IDbProviderFactory dbProviderFactory) 
			: base(connectionInfo, dbProviderFactory)
        {
        }

        public override ISisoDatabase CreateIfNotExists()
        {
            throw new SisoDbNotSupportedException(ExceptionMessages.DbOp_NotSupportedByAzure);
        }

        public override ISisoDatabase DeleteIfExists()
        {
            throw new SisoDbNotSupportedException(ExceptionMessages.DbOp_NotSupportedByAzure);
        }

        public override ISisoDatabase EnsureNewDatabase()
        {
            throw new SisoDbNotSupportedException(ExceptionMessages.DbOp_NotSupportedByAzure);
        }

        public override bool Exists()
        {
            throw new SisoDbNotSupportedException(ExceptionMessages.DbOp_NotSupportedByAzure);
        }

        public override ISisoDatabase InitializeExisting()
        {
            throw new SisoDbNotSupportedException(ExceptionMessages.DbOp_NotSupportedByAzure);
        }

        protected override DbSession CreateSession()
        {
            return new SqlAzureSession(this);
        }
    }
}