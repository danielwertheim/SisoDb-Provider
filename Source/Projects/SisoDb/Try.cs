using System;
using System.Collections.Generic;
using System.Linq;

namespace SisoDb
{
    public static class Try //TODO: Move to NCore
    {
        public static AggregateException This(params Action[] actions)
        {
            var exceptions = new List<Exception>();

            foreach (var action in actions)
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }

            return exceptions.Any() 
                ? new AggregateException(exceptions) 
                : null;
        }
    }
}