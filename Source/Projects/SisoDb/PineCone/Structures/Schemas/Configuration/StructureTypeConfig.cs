using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using SisoDb.EnsureThat;
using SisoDb.NCore.Expressions;

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

        public ISet<string> MemberPathsBeingIndexed
        {
            get { return _memberPathsBeingIndexed; }
        }

        public ISet<string> MemberPathsNotBeingIndexed
        {
            get { return _memberPathsNotBeingIndexed; }
        }

        public StructureTypeConfig(Type type)
        {
            Ensure.That(type, "type").IsNotNull();
            
            Type = type;

            _memberPathsBeingIndexed = new HashSet<string>();
            _memberPathsNotBeingIndexed = new HashSet<string>();
        }

        public IStructureTypeConfig OnlyIndexThis(params string[] memberPaths)
        {
            MemberPathsNotBeingIndexed.Clear();

            foreach (var memberPath in memberPaths)
                _memberPathsBeingIndexed.Add(memberPath);

            return this;
        }

        public IStructureTypeConfig DoNotIndexThis(params string[] memberPaths)
        {
            MemberPathsBeingIndexed.Clear();

            foreach (var memberPath in memberPaths)
                _memberPathsNotBeingIndexed.Add(memberPath);

            return this;
        }
    }

    [Serializable]
    public class StructureTypeConfig<T> : StructureTypeConfig, IStructureTypeConfig<T> where T : class
    {
        public static readonly Type TypeOfT = typeof(T);

        public StructureTypeConfig()
            : base(TypeOfT)
        {
        }

        public IStructureTypeConfig<T> OnlyIndexThis(params Expression<Func<T, object>>[] members)
        {
            OnlyIndexThis(
                members
                    .Select(e => e.GetRightMostMember().ToPath())
                    .ToArray());

            return this;
        }

        public IStructureTypeConfig<T> DoNotIndexThis(params Expression<Func<T, object>>[] members)
        {
            DoNotIndexThis(
                members
                    .Select(e => e.GetRightMostMember().ToPath())
                    .ToArray());

            return this;
        }
    }
}