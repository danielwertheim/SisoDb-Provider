using System.Data.SqlClient;
using SisoDb.Lambdas.Processors;
using SisoDb.Providers.SqlProvider.DbSchema;
using SisoDb.Querying;
using SisoDb.Resources;
using SisoDb.Structures;
using SisoDb.Structures.Schemas;

namespace SisoDb.Providers.SqlProvider
{
    public class SqlDatabase : ISqlDatabase
    {
        private readonly IProviderFactory _providerFactory;

        public string Name { get; private set; }

        public ISisoConnectionInfo ServerConnectionInfo { get; private set; }

        public ISisoConnectionInfo ConnectionInfo { get; private set; }

        public IDbSchemaManager DbSchemaManager { get; set; }

        public IIdentityGenerator IdentityGenerator { get; set; }

        public IStructureSchemas StructureSchemas { get; set; }

        public IStructureBuilder StructureBuilder { get; set; }

        public SqlDatabase(ISisoConnectionInfo connectionInfo)
        {
            if (connectionInfo.ProviderType != StorageProviders.Sql2008)
                throw new SisoDbException(ExceptionMessages.SqlDatabase_UnsupportedProviderSpecified
                    .Inject(connectionInfo.ProviderType, StorageProviders.Sql2008));

            InitializeConnectionInfo(connectionInfo);

            _providerFactory = SisoDbEnvironment.GetProviderFactory(ConnectionInfo.ProviderType);

            StructureSchemas = new StructureSchemas();
            DbSchemaManager = new SqlDbSchemaManager(ConnectionInfo);
            IdentityGenerator = new SqlIdentityGenerator(ConnectionInfo);
            StructureBuilder = new StructureBuilder(SisoDbEnvironment.JsonSerializer, SisoDbEnvironment.Formatting.StringConverter);
        }

        private void InitializeConnectionInfo(ISisoConnectionInfo connectionInfo)
        {
            ConnectionInfo = connectionInfo;

            var cnStringBuilder = new SqlConnectionStringBuilder(ConnectionInfo.ConnectionString.PlainString);
            
            Name = cnStringBuilder.InitialCatalog;
            if(Name.IsNullOrWhiteSpace())
                throw new SisoDbException(ExceptionMessages.SqlDatabase_ConnectionInfo_MissingName);

            cnStringBuilder.InitialCatalog = string.Empty;
            var cnString = connectionInfo.ConnectionString.ReplacePlain(cnStringBuilder.ConnectionString);
            ServerConnectionInfo = new SisoConnectionInfo(cnString);
        }

        public virtual void EnsureNewDatabase()
        {
            using (var client = new SqlDbClient(ServerConnectionInfo, false))
            {
                if (client.DatabaseExists(Name))
                    client.DropDatabase(Name);

                client.CreateDatabase(Name);
            }
        }

        public virtual void CreateIfNotExists()
        {
            using (var client = new SqlDbClient(ServerConnectionInfo, false))
            {
                if (!client.DatabaseExists(Name))
                    client.CreateDatabase(Name);
            }
        }

        public virtual void InitializeExisting()
        {
            using (var client = new SqlDbClient(ServerConnectionInfo, false))
            {
                if (!client.DatabaseExists(Name))
                    throw new SisoDbException(ExceptionMessages.SqlDatabase_InitializeExisting_DbDoesNotExist.Inject(Name));

                client.CreateSysTables(Name);
            }
        }

        public virtual void DeleteIfExists()
        {
            using (var client = new SqlDbClient(ServerConnectionInfo, false))
            {
                if (client.DatabaseExists(Name))
                    client.DropDatabase(Name);
            }
        }

        public virtual bool Exists()
        {
            using (var client = new SqlDbClient(ServerConnectionInfo, false))
            {
                return client.DatabaseExists(Name);
            }
        }

        public void DropStructureSet<T>() where T : class
        {
            var structureSchema = StructureSchemas.GetSchema<T>();
            DbSchemaManager.DropStructureSet(structureSchema);
        }

        public void UpsertStructureSet<T>() where T : class
        {
            var structureSchema = StructureSchemas.GetSchema<T>();
            DbSchemaManager.UpsertStructureSet(structureSchema);
        }

        public IUnitOfWork CreateUnitOfWork()
        {
            var dbClient = new SqlDbClient(ConnectionInfo, true);

            var queryGenerator = new SqlQueryGenerator(
                new ParsedSelectorSqlProcessor(SisoDbEnvironment.MemberNameGenerator),
                new ParsedSortingSqlProcessor(SisoDbEnvironment.MemberNameGenerator),
                new ParsedIncludeSqlProcessor(SisoDbEnvironment.MemberNameGenerator));

            var commandBuilderFactory = new CommandBuilderFactory();
            
            var unitOfWork = new SqlUnitOfWork(
                dbClient, IdentityGenerator, DbSchemaManager, 
                StructureSchemas, StructureBuilder, 
                SisoDbEnvironment.JsonSerializer, queryGenerator,
                commandBuilderFactory);

            return unitOfWork;
        }
    }
}