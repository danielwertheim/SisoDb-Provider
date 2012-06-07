using System;

namespace SisoDb
{
    [Serializable]
    public class SisoDbNotSupportedException : SisoDbException
    {
        public SisoDbNotSupportedException(string message) : base(message)
        {
        }
    }
}