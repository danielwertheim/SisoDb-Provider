using System;
using System.Collections.Generic;
using System.Linq;

namespace SisoDb.Diagnostics
{
    [Serializable]
    public class DiagnosticsGroup
    {
        private readonly Dictionary<string, DiagnosticsNode> _nodes;

        public string Name { get; private set; }
        public IEnumerable<DiagnosticsNode> Nodes
        {
            get { return _nodes.Values; }
        }

        public DiagnosticsGroup(string name, params object[] formattingArgs)
        {
            Name = formattingArgs.Any() ? string.Format(name, formattingArgs) : name;
            _nodes = new Dictionary<string, DiagnosticsNode>();
        }

        public DiagnosticsGroup AddNode(string name, object value)
        {
            _nodes.Add(name, new DiagnosticsNode(name, value.ToString()));

            return this;
        }

        public DiagnosticsGroup AddNode(string name, string value)
        {
            _nodes.Add(name, new DiagnosticsNode(name, value));

            return this;
        }
    }
}