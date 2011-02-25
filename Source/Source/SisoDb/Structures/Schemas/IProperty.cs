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

        IList<object> GetValues<T>(T item) where T : class;

        void SetValue<TItem, TValue>(TItem item, TValue value) where TItem : class;
    }
}