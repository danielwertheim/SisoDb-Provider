using System;
using System.Collections.Generic;
using System.Linq;

namespace SisoDb
{
    [Serializable]
    internal class ConnectionString : IConnectionString
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

        internal ConnectionString(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException("value");

            var parts = GetParts(value);

            var plainString = parts.SingleOrDefault(p => p.StartsWith(PlainMarker, StringComparison.InvariantCultureIgnoreCase));
            if (plainString == null)
                throw new ArgumentException(
                    string.Format("The connectionstring is missing the Plain-part, indicated by '{0}'. Example: '{1}'.", PlainMarker, Example));
            PlainString = plainString.Substring(PlainMarker.Length);

            var sisoDbString = parts.SingleOrDefault(p => p.StartsWith(SisoDbMarker, StringComparison.InvariantCultureIgnoreCase));
            if (sisoDbString == null)
                throw new ArgumentException(
                    string.Format("The connectionstring is missing the SisoDb-part, indicated by '{0}'. Example: '{1}'.", SisoDbMarker, Example));
            SisoDbString = sisoDbString.Substring(SisoDbMarker.Length);

            InitializeSisoDbKeyValues();
        }

        private static string[] GetParts(string value)
        {
            var parts = value.Split("||".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2)
                throw new ArgumentException("The connectionstring should have exactly two parts ('{0}' and '{1}'). Example: '{2}'.".Inject(SisoDbMarker, PlainMarker, Example));

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
                throw new ArgumentException("The SisoDb-part is missing required key: 'provider'.");
        }

        public IConnectionString ReplacePlain(string plainString)
        {
            var cnString = string.Format("{0}{1}||{2}{3}", SisoDbMarker, SisoDbString, PlainMarker, plainString);

            return new ConnectionString(cnString);
        }
    }
}