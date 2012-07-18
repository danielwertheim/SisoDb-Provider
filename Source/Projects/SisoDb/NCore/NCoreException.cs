using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NCore
{
    [Serializable]
    public class NCoreException : AggregateException
    {
        public NCoreException(string message)
            : base(message)
        {
        }

        public NCoreException(string message, IEnumerable<Exception> innerExceptions)
            : base(message, innerExceptions)
        {
        }

        protected NCoreException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}