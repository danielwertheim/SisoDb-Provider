using System;
using System.Collections.Generic;

namespace SisoDb.Structures.Schemas.Configuration
{
    public interface IStructureTypeConfig
    {
        Type Type { get; }
        bool IndexConfigIsEmpty { get; }
        ISet<string> MemberPathsBeingIndexed { get; }
        ISet<string> MemberPathsNotBeingIndexed { get; }
    }
}