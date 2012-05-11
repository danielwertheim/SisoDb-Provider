using System;
using System.Collections.Generic;
using System.Linq;

namespace SisoDb.Diagnostics
{
    [Serializable]
    public class DiagnosticsInfo
    {
        private readonly Dictionary<string, DiagnosticsGroup> _groups;
        private readonly Dictionary<string, DiagnosticsNode> _nodes;

        public string Name { get; private set; }
        public bool HasGroups
        {
            get { return _groups.Any(); }
        }
        public bool HasNodes
        {
            get { return _nodes.Any(); }
        }
        public IEnumerable<DiagnosticsGroup> Groups
        {
            get { return _groups.Values; }
        }
        public IEnumerable<DiagnosticsNode> Nodes
        {
            get { return _nodes.Values; }
        }

        public DiagnosticsInfo(string name, params object[] formattingArgs)
        {
            Name = formattingArgs.Any() ? string.Format(name, formattingArgs) : name;

            _groups = new Dictionary<string, DiagnosticsGroup>();
            _nodes = new Dictionary<string, DiagnosticsNode>();
        }

        public DiagnosticsGroup AddGroup(string name, params object[] formattingArgs)
        {
            var group = new DiagnosticsGroup(name, formattingArgs);
            _groups.Add(group.Name, group);

            return group;
        }

        public DiagnosticsInfo AddNode(string name, object value)
        {
            _nodes.Add(name, new DiagnosticsNode(name, value.ToString()));

            return this;
        }

        public DiagnosticsInfo AddNode(string name, string value)
        {
            _nodes.Add(name, new DiagnosticsNode(name, value));

            return this;
        }
    }
}