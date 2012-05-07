using System;
using System.Collections.Generic;
using System.Linq;

namespace SisoDb.Diagnostics
{
    [Serializable]
    public class DiagnosticsContext
    {
        private readonly Dictionary<string, DiagnosticsSection> _sections;

        public string Name { get; private set; }
        public DiagnosticsSection this[string name]
        {
            get { return _sections[name]; }
        }
        public IEnumerable<DiagnosticsSection> Sections
        {
            get { return _sections.Values; }
        }

        public DiagnosticsContext(string name, params object[] formattingArgs)
        {
            Name = formattingArgs.Any() ? string.Format(name, formattingArgs) : name;

            _sections = new Dictionary<string, DiagnosticsSection>();
        }

        public DiagnosticsSection AddSection(string name)
        {
            var section = new DiagnosticsSection(name);
            _sections[name] = section;

            return section;
        }
    }
}