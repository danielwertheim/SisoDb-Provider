using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using SisoDb.EnsureThat;
using SisoDb.NCore;
using SisoDb.Resources;

namespace SisoDb
{
    [Serializable]
    public class ConnectionString : IConnectionString
    {
        private const string Example = "sisodb:[SisoDb configvalues];||plain:[Plain configvalues]";

        private static class CnStringParts
        {
            public const string SisoDbMarker = "sisodb:";
            public const string PlainMarker = "plain:";
            public const string PartsDivider = "||";
        }

        private static class SisoDbCnStringKeys
        {
            public const string Provider = "provider";
            public const string BackgroundIndexing = "backgroundindexing";
        }

        private readonly IDictionary<string, string> _sisoDbKeyValues;

        public string SisoDbString { get; private set; }

        public string PlainString { get; private set; }

        public string Provider
        {
            get { return _sisoDbKeyValues[SisoDbCnStringKeys.Provider]; }
        }

        public string BackgroundIndexing
        {
            get { return _sisoDbKeyValues[SisoDbCnStringKeys.BackgroundIndexing]; }
        }

        public static IConnectionString Get(string connectionStringOrName)
        {
            Ensure.That(connectionStringOrName, "connectionStringOrName").IsNotNullOrWhiteSpace();

            var config =
                ConfigurationManager.ConnectionStrings[string.Concat(Environment.MachineName, "_", connectionStringOrName)]
                ?? ConfigurationManager.ConnectionStrings[connectionStringOrName];

            return config == null
                ? new ConnectionString(connectionStringOrName)
                : new ConnectionString(config.ConnectionString);
        }

        private ConnectionString()
        {
            SisoDbString = string.Empty;
            PlainString = string.Empty;
            _sisoDbKeyValues = CreateDefaultSisoDbKeyValues();
        }

        public ConnectionString(string value) : this()
        {
            Ensure.That(value, "value").IsNotNullOrWhiteSpace();

            var parts = GetParts(value);
            
            var containsOnlyPlainPart = parts.Length == 1;
            if (containsOnlyPlainPart) 
                Initialize(parts[0]); 
            else 
                Initialize(parts);
        }

        private void Initialize(string cnString)
        {
            PlainString = cnString;
        }

        private void Initialize(string[] parts)
        {
            var plainString = parts.SingleOrDefault(p => p.StartsWith(CnStringParts.PlainMarker, StringComparison.OrdinalIgnoreCase));
            if (plainString == null)
                throw new ArgumentException(ExceptionMessages.ConnectionString_MissingPlainPart.Inject(CnStringParts.PlainMarker, Example));
            PlainString = plainString.Substring(CnStringParts.PlainMarker.Length);

            var sisoDbString = parts.SingleOrDefault(p => p.StartsWith(CnStringParts.SisoDbMarker, StringComparison.OrdinalIgnoreCase));
            if (sisoDbString == null)
                throw new ArgumentException(ExceptionMessages.ConnectionString_MissingSisoDbPart.Inject(CnStringParts.SisoDbMarker, Example));
            SisoDbString = sisoDbString.Substring(CnStringParts.SisoDbMarker.Length);

            InitializeWithKeyValues(_sisoDbKeyValues, SisoDbString);
        }

        private static string[] GetParts(string value)
        {
            if (!value.Contains(CnStringParts.PartsDivider))
            {
                var isNotPureCnString = value.Contains(CnStringParts.SisoDbMarker);
                if (isNotPureCnString)
                    throw new SisoDbException(ExceptionMessages.ConnectionString_ShouldBePureIfNoPartsDividerExists.Inject(CnStringParts.PartsDivider, CnStringParts.SisoDbMarker, CnStringParts.PlainMarker));

                return new[] { value.Replace(CnStringParts.PlainMarker, string.Empty) };
            }

            var parts = value.Split(new[] { CnStringParts.PartsDivider }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2)
                throw new SisoDbException(ExceptionMessages.ConnectionString_MissingParts.Inject(CnStringParts.SisoDbMarker, CnStringParts.PlainMarker, Example));

            return parts;
        }

        private static IDictionary<string, string> CreateDefaultSisoDbKeyValues()
        {
            var container = new Dictionary<string, string>();
            container[SisoDbCnStringKeys.Provider] = string.Empty;
            container[SisoDbCnStringKeys.BackgroundIndexing] = string.Empty;

            return container;
        }

        private static void InitializeWithKeyValues(IDictionary<string, string> container, string sisoDbString)
        {
            var keyValues = sisoDbString.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (var parts in
                keyValues.Select(keyValue => keyValue.Split("=".ToCharArray(), StringSplitOptions.None)))
                container[parts[0].ToLower()] = parts[1] ?? string.Empty;

            if (!container.ContainsKey("provider"))
                throw new ArgumentException(ExceptionMessages.ConnectionString_MissingProviderKey);
        }

        public IConnectionString ReplacePlain(string plainString)
        {
            var sisoDbPartExists = !string.IsNullOrWhiteSpace(SisoDbString);
            var plainPartExists = !string.IsNullOrWhiteSpace(PlainString);

            var cnString = string.Format("{0}{1}{2}{3}{4}",
                sisoDbPartExists ? CnStringParts.SisoDbMarker : string.Empty,
                sisoDbPartExists ? SisoDbString : string.Empty,
                
                sisoDbPartExists 
                && plainPartExists ? CnStringParts.PartsDivider : string.Empty,
                
                plainPartExists ? CnStringParts.PlainMarker : string.Empty,
                plainPartExists ? plainString : string.Empty).Trim();

            return new ConnectionString(cnString);
        }
    }
}