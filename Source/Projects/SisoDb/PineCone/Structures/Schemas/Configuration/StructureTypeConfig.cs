using System;
using System.Collections.Generic;
using SisoDb.EnsureThat;

namespace SisoDb.PineCone.Structures.Schemas.Configuration
{
    [Serializable]
    public class StructureTypeConfig : IStructureTypeConfig
    {
        private readonly HashSet<string> _memberPathsBeingIndexed;
        private readonly HashSet<string> _memberPathsNotBeingIndexed;

        public Type Type { get; private set; }
        public bool IsEmpty
        {
            get { return MemberPathsBeingIndexed.Count < 1 && MemberPathsNotBeingIndexed.Count < 1; }
        }
        public bool IncludeNestedStructureMembers { get; set; }
        public ISet<string> MemberPathsBeingIndexed
        {
            get { return _memberPathsBeingIndexed; }
        }
        public ISet<string> MemberPathsNotBeingIndexed
        {
            get { return _memberPathsNotBeingIndexed; }
        }

        public StructureTypeConfig(Type structureType)
        {
            Ensure.That(structureType, "structureType").IsNotNull();
            
            Type = structureType;

            _memberPathsBeingIndexed = new HashSet<string>();
            _memberPathsNotBeingIndexed = new HashSet<string>();
            IncludeNestedStructureMembers = false;
        }
    }
}