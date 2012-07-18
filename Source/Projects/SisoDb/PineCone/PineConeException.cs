using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SisoDb.PineCone
{
    [Serializable]
    public class PineConeException : AggregateException
    {
        public PineConeException(string message)
            : base(message)
        {
        }

        public PineConeException(string message, IEnumerable<Exception> innerExceptions)
            : base(message, innerExceptions)
        {
        }

        protected PineConeException(SerializationInfo info, StreamingContext context)
            : base(info, context) 
        { }
    }
}