using System;
using System.Collections.Generic;

namespace SisoDb.PineCone.Structures.Schemas.Configuration
{
    public interface IStructureTypeConfig
    {
        Type Type { get; }
        bool IsEmpty { get; }
        bool IncludeNestedStructureMembers { get; set; }
        ISet<string> MemberPathsBeingIndexed { get; }
        ISet<string> MemberPathsNotBeingIndexed { get; }
    }
}