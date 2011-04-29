using System;
using System.Collections.Generic;

namespace SisoDb
{
    [Serializable]
    public class SisoDbException : AggregateException
    {
        public SisoDbException(string message)
            : base(message)
        {
        }

        public SisoDbException(string message, IEnumerable<Exception> innerExceptions)
            : base(message, innerExceptions)
        {
        }
    }
}