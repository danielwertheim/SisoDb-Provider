using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using EnsureThat;
using NCore;
using SisoDb.Resources;

namespace SisoDb
{
    [Serializable]
    public class ConnectionString : IConnectionString
    {
        private const string SisoDbMarker = "sisodb:";
        private const string PlainMarker = "plain:";
        private const string Example = "sisodb:[SisoDb configvalues];||plain:[Plain configvalues]";

        private readonly IDictionary<string, string> _sisoDbKeyValues;

        public string SisoDbString { get; private set; }

        public string PlainString { get; private set; }

        public string Provider
        {
            get { return _sisoDbKeyValues["provider"]; }
        }

        public string BackgroundIndexing
        {
            get { return _sisoDbKeyValues["backgroundindexing"]; }
        }

        public ConnectionString(string value)
        {
            Ensure.That(value, "value").IsNotNullOrWhiteSpace();

            var parts = GetParts(value);

            var plainString = parts.SingleOrDefault(p => p.StartsWith(PlainMarker, StringComparison.OrdinalIgnoreCase));
            if (plainString == null)
                throw new ArgumentException(ExceptionMessages.ConnectionString_MissingPlainPart.Inject(PlainMarker, Example));
            PlainString = plainString.Substring(PlainMarker.Length);

			var sisoDbString = parts.SingleOrDefault(p => p.StartsWith(SisoDbMarker, StringComparison.OrdinalIgnoreCase));
            if (sisoDbString == null)
                throw new ArgumentException(ExceptionMessages.ConnectionString_MissingSisoDbPart.Inject(SisoDbMarker, Example));
            SisoDbString = sisoDbString.Substring(SisoDbMarker.Length);

            _sisoDbKeyValues = GetSisoDbKeyValuesFrom(SisoDbString);
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

        private static string[] GetParts(string value)
        {
            var parts = value.Split(new[]{"||"}, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2)
                throw new ArgumentException(ExceptionMessages.ConnectionString_MissingParts.Inject(SisoDbMarker, PlainMarker, Example));

            return parts;
        }

        private static IDictionary<string, string> GetSisoDbKeyValuesFrom(string sisoDbString)
        {
            var container = new Dictionary<string, string>();
            container["provider"] = string.Empty;
            container["backgroundindexing"] = string.Empty;

            var keyValues = sisoDbString.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (var parts in
                keyValues.Select(keyValue => keyValue.Split("=".ToCharArray(), StringSplitOptions.None)))
                container[parts[0].ToLower()] = parts[1] ?? string.Empty;

            EnsureRequiredSisoDbKeysExists(container);

            return container;
        }

        private static void EnsureRequiredSisoDbKeysExists(IDictionary<string, string> container)
        {
            if (!container.ContainsKey("provider"))
                throw new ArgumentException(ExceptionMessages.ConnectionString_MissingProviderKey);
        }

        public IConnectionString ReplacePlain(string plainString)
        {
            var cnString = string.Format("{0}{1}||{2}{3}", SisoDbMarker, SisoDbString, PlainMarker, plainString);

            return new ConnectionString(cnString);
        }
    }
}