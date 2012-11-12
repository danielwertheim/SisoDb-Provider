using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

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

        protected SisoDbException(SerializationInfo info, StreamingContext context)
            : base(info, context) 
        { }
    }
}