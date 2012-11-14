using System;

namespace SisoDb.Dac
{
    [Serializable]
    public class GeographyDacParameter : DacParameter 
    {
        public GeographyDacParameter(string name, object value) : base(name, value) { }
    }
}