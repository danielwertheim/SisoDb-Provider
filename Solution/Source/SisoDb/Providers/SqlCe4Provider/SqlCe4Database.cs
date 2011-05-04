using System;
using System.Data.SqlServerCe;
using System.IO;
using SisoDb.Core;
using SisoDb.Providers.DbSchema;
using SisoDb.Providers.Sql2008Provider;
using SisoDb.Resources;
using SisoDb.Structures;
using SisoDb.Structures.Schemas;

namespace SisoDb.Providers.SqlCe4Provider
{
    public class SqlCe4Database : ISqlDatabase
    {
        public string Name { get; private set; }

        public ISisoConnectionInfo ServerConnectionInfo
        {
            get { return ConnectionInfo; }
        }

        public ISisoConnectionInfo ConnectionInfo { get; private set; }

        public IStructureSchemas StructureSchemas { get; set; }

        public IStructureBuilder StructureBuilder { get; set; }

        public IDbSchemaManager DbSchemaManager { get; set; }

        public SqlCe4Database(ISisoConnectionInfo connectionInfo)
        {
            ConnectionInfo = connectionInfo.AssertNotNull("connectionInfo");
            
            Name = ExtractName(connectionInfo);
            
            if (ConnectionInfo.ProviderType != StorageProviders.SqlCe4)
                throw new SisoDbException(ExceptionMessages.SqlCe4Database_UnsupportedProviderSpecified.Inject(
                    ConnectionInfo.ProviderType, StorageProviders.SqlCe4));

            StructureSchemas = SisoEnvironment.Resources.ResolveStructureSchemas();
            StructureBuilder = SisoEnvironment.Resources.ResolveStructureBuilder();
            DbSchemaManager = SisoEnvironment.Resources.ResolveDbSchemaManager();
        }

        private static string ExtractName(ISisoConnectionInfo connectionInfo)
        {
            var cnStringBuilder = new SqlCeConnectionStringBuilder(connectionInfo.ConnectionString.PlainString);
            return cnStringBuilder.DataSource.Contains(Path.PathSeparator.ToString())
                       ? Path.GetFileNameWithoutExtension(cnStringBuilder.DataSource)
                       : cnStringBuilder.DataSource;
        }

        public void EnsureNewDatabase()
        {
            throw new NotImplementedException();
        }

        public void CreateIfNotExists()
        {
            using (var engine = new SqlCeEngine(ConnectionInfo.ConnectionString.PlainString))
            {
                engine.CreateDatabase();
            }
        }

        public void InitializeExisting()
        {
            throw new NotImplementedException();
        }

        public void DeleteIfExists()
        {
            throw new NotImplementedException();
        }

        public bool Exists()
        {
            throw new NotImplementedException();
        }

        public void DropStructureSet<T>() where T : class
        {
            throw new NotImplementedException();
        }

        public void UpsertStructureSet<T>() where T : class
        {
            throw new NotImplementedException();
        }

        public void UpdateStructureSet<TOld, TNew>(Func<TOld, TNew, StructureSetUpdaterStatuses> onProcess)
            where TOld : class
            where TNew : class
        {
            throw new NotImplementedException();
        }

        public IQueryEngine CreateQueryEngine()
        {
            throw new NotImplementedException();
        }

        public IUnitOfWork CreateUnitOfWork()
        {
            throw new NotImplementedException();
        }
    }
}