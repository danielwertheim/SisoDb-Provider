using System;
using System.Collections.Generic;
using System.Reflection;

namespace SisoDb.Structures.Schemas
{
    public interface IStructureProperty
    {
        PropertyInfo Member { get; }

        string Name { get; }

        string Path { get; }

        Type PropertyType { get; }

        IStructureProperty Parent { get; }

        bool IsRootMember { get; }

        bool IsUnique { get; }
        
        bool IsEnumerable { get; }

        bool IsElement { get; }
        
        Type ElementType { get; }

        TOut? GetIdValue<TRoot, TOut>(TRoot root)
            where TRoot : class 
            where TOut : struct;

        void SetIdValue<TRoot, TIn>(TRoot root, TIn value)
            where TRoot : class
            where TIn : struct;

        IList<object> GetValues<TRoot>(TRoot root)
            where TRoot : class;
    }
}