using System;

namespace SisoDb.Dac
{
    [Serializable]
    public class ArrayDacParameter : DacParameter
    {
        public ArrayDacParameter(string name, object[] value) : base(name, value)
        {
        }
    }
}