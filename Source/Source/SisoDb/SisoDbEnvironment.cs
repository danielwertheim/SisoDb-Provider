using System.Collections.Generic;
using SisoDb.Core;
using SisoDb.Cryptography;
using SisoDb.Providers;
using SisoDb.Serialization;
using SisoDb.Structures.Schemas;

namespace SisoDb
{
    public static class SisoDbEnvironment
    {
        private static readonly Dictionary<StorageProviders, IProviderFactory> ProviderFactories;

        public static readonly ISisoDbFormatting Formatting;
        public static readonly IHashService HashService;
        public static readonly IMemberNameGenerator MemberNameGenerator;
        public static readonly IJsonSerializer JsonSerializer;
        public static readonly IStructureTypeReflecter StructureTypeReflecter;

        static SisoDbEnvironment()
        {
            Formatting = new SisoDbFormatting();
            HashService = new HashService();
            MemberNameGenerator = new HashMemberNameGenerator(HashService);
            JsonSerializer = new ServiceStackJsonSerializer();
            StructureTypeReflecter = new StructureTypeReflecter();
            
            ProviderFactories = new Dictionary<StorageProviders, IProviderFactory>();
            RegisterProviderFactory(StorageProviders.SqlAzure, new SqlAzureProviderFactory());
            RegisterProviderFactory(StorageProviders.Sql2008, new Sql2008ProviderFactory());
        }

        public static IProviderFactory GetProviderFactory(StorageProviders storageProvider)
        {
            return ProviderFactories[storageProvider];
        }

        public static void RegisterProviderFactory(StorageProviders storageProvider, IProviderFactory providerFactory)
        {
            ProviderFactories.Add(storageProvider, providerFactory);
        }

        public static void ReplaceProviderFactory(StorageProviders storageProvider, IProviderFactory providerFactory)
        {
            ProviderFactories.Remove(storageProvider);
            RegisterProviderFactory(storageProvider, providerFactory);
        }
    }
}