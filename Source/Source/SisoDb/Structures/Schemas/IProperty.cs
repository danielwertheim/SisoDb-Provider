using System;
using System.Collections.Generic;
using System.Reflection;

namespace SisoDb.Structures.Schemas
{
    public interface IProperty
    {
        PropertyInfo Member { get; }

        string Name { get; }

        string Path { get; }

        Type PropertyType { get; }

        int Level { get; }

        IProperty Parent { get; }
        
        bool IsSimpleType { get; }

        bool IsUnique { get; }
       
        bool IsEnumerable { get; }

        bool IsElement { get; }
        
        Type ElementType { get; }

        TReturn? GetIdValue<T, TReturn>(T item)
            where T : class 
            where TReturn : struct;

        void SetIdValue<T, TIn>(T item, TIn value)
            where T : class
            where TIn : struct;

        IList<object> GetValues(object item);

        //void SetValue<TValue>(object item, TValue value);
    }
}