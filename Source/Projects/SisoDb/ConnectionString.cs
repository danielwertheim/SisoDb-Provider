using System;
using System.Collections.Generic;
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

        private Dictionary<string, string> _sisoDbKeyValues;

        public string SisoDbString { get; private set; }

        public string PlainString { get; private set; }

        public string Provider
        {
            get { return _sisoDbKeyValues["provider"]; }
        }

        public ConnectionString(string value)
        {
            Ensure.That(value, "value").IsNotNullOrWhiteSpace();

            var parts = GetParts(value);

            var plainString = parts.SingleOrDefault(p => p.StartsWith(PlainMarker, StringComparison.InvariantCultureIgnoreCase));
            if (plainString == null)
                throw new ArgumentException(ExceptionMessages.ConnectionString_MissingPlainPart.Inject(PlainMarker, Example));
            PlainString = plainString.Substring(PlainMarker.Length);

            var sisoDbString = parts.SingleOrDefault(p => p.StartsWith(SisoDbMarker, StringComparison.InvariantCultureIgnoreCase));
            if (sisoDbString == null)
                throw new ArgumentException(ExceptionMessages.ConnectionString_MissingSisoDbPart.Inject(SisoDbMarker, Example));
            SisoDbString = sisoDbString.Substring(SisoDbMarker.Length);

            InitializeSisoDbKeyValues();
        }

        private static string[] GetParts(string value)
        {
            var parts = value.Split("||".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2)
                throw new ArgumentException(ExceptionMessages.ConnectionString_MissingParts.Inject(SisoDbMarker, PlainMarker, Example));

            return parts;
        }

        private void InitializeSisoDbKeyValues()
        {
            _sisoDbKeyValues = new Dictionary<string, string>();

            var keyValues = SisoDbString.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (var parts in
                keyValues.Select(keyValue => keyValue.Split("=".ToCharArray(), StringSplitOptions.None)))
                _sisoDbKeyValues.Add(parts[0].ToLower(), parts[1]);

            EnsureRequiredSisoDbKeysExists();
        }

        private void EnsureRequiredSisoDbKeysExists()
        {
            if (!_sisoDbKeyValues.ContainsKey("provider"))
                throw new ArgumentException(ExceptionMessages.ConnectionString_MissingProviderKey);
        }

        public IConnectionString ReplacePlain(string plainString)
        {
            var cnString = string.Format("{0}{1}||{2}{3}", SisoDbMarker, SisoDbString, PlainMarker, plainString);

            return new ConnectionString(cnString);
        }
    }
}