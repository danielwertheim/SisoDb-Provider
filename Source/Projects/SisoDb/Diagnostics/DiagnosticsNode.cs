using System;

namespace SisoDb.Diagnostics
{
    [Serializable]
    public class DiagnosticsNode
    {
        public string Name { get; private set; }
        public string Value { get; private set; }

        public DiagnosticsNode(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}