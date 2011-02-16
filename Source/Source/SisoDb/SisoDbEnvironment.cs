using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using SisoDb.Cryptography;
using SisoDb.Providers;
using SisoDb.Serialization;

namespace SisoDb
{
    public static class SisoDbEnvironment
    {
        private static readonly Dictionary<StorageProviders, IProviderFactory> ProviderFactories;

        public static readonly IFormatProvider FormatProvider;
        public static readonly IFormatProvider DateTimeFormatProvider;
        public static readonly string DateTimePattern;
        public static readonly Encoding Encoding;
        public static readonly IHashService HashService;
        public static readonly IMemberNameGenerator MemberNameGenerator;
        public static readonly IStringConverter StringConverter;
        public static readonly IJsonSerializer JsonSerializer;

        static SisoDbEnvironment()
        {
            FormatProvider = CultureInfo.InvariantCulture;
            DateTimeFormatProvider = new CultureInfo("sv-SE");
            DateTimePattern = "yyyy-MM-dd HH:mm:ss.FFFFFFFK";
            Encoding = Encoding.UTF8;
            HashService = new HashService();
            MemberNameGenerator = new HashMemberNameGenerator(HashService);
            StringConverter = new StringConverter(FormatProvider, DateTimeFormatProvider, DateTimePattern);
            //JsonSerializer = new NewtonsoftJsonSerializer(); //TODO: Experimental
            JsonSerializer = new ServiceStackJsonSerializer();
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