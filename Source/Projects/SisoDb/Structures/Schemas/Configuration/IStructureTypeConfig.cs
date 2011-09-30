using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SisoDb.Structures.Schemas.Configuration
{
    public interface IStructureTypeConfig
    {
        Type Type { get; }
        bool IsEmpty { get; }
        ISet<string> MemberPathsBeingIndexed { get; }
        ISet<string> MemberPathsNotBeingIndexed { get; }

        IStructureTypeConfig OnlyIndexThis(params string[] memberPaths);
        IStructureTypeConfig DoNotIndexThis(params string[] memberPaths);
    }

    public interface IStructureTypeConfig<T> : IStructureTypeConfig where T : class 
    {
        IStructureTypeConfig<T> OnlyIndexThis(params Expression<Func<T, object>>[] members);
        IStructureTypeConfig<T> DoNotIndexThis(params Expression<Func<T, object>>[] members);
    }
}