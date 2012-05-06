using System;
using System.Collections.Generic;
using System.Linq;

namespace SisoDb.Diagnostics
{
    [Serializable]
    public class DiagnosticsSection
    {
        private readonly Dictionary<string, DiagnosticsGroup> _groups;
        public string Name { get; private set; }
        public IEnumerable<DiagnosticsGroup> Groups
        {
            get { return _groups.Values; }
        }

        public DiagnosticsSection(string name, params object[] formattingArgs)
        {
            Name = formattingArgs.Any() ? string.Format(name, formattingArgs) : name;

            _groups = new Dictionary<string, DiagnosticsGroup>();
        }

        public DiagnosticsSection AddGroup(DiagnosticsGroup group)
        {
            _groups.Add(group.Name, group);

            return this;
        }

        public DiagnosticsSection AddGroupsFor<T>(IEnumerable<T> items, Func<T, DiagnosticsGroup> fn)
        {
            foreach (var grp in items.Select(fn.Invoke))
                _groups.Add(grp.Name, grp);

            return this;
        }
    }
}