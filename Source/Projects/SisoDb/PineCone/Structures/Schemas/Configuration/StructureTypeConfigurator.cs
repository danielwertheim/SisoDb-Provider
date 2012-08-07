using System;
using System.Linq;
using System.Linq.Expressions;
using SisoDb.EnsureThat;
using SisoDb.NCore.Expressions;

namespace SisoDb.PineCone.Structures.Schemas.Configuration
{
    public class StructureTypeConfigurator : IStructureTypeConfigurator
    {
        private readonly IStructureTypeConfig _config;

        public IStructureTypeConfig Config
        {
            get { return _config; }
        }

        public StructureTypeConfigurator(IStructureTypeConfig config)
        {
            Ensure.That(config, "config").IsNotNull();

            _config = config;
        }

        public virtual IStructureTypeConfigurator AllowNestedStructures()
        {
            Config.IncludeContainedStructureMembers = true;

            return this;
        }

        public virtual IStructureTypeConfigurator OnlyIndexThis(params string[] memberPaths)
        {
            Config.MemberPathsNotBeingIndexed.Clear();

            foreach (var memberPath in memberPaths)
                Config.MemberPathsBeingIndexed.Add(memberPath);

            return this;
        }

        public virtual IStructureTypeConfigurator DoNotIndexThis(params string[] memberPaths)
        {
            Config.MemberPathsBeingIndexed.Clear();

            foreach (var memberPath in memberPaths)
                Config.MemberPathsNotBeingIndexed.Add(memberPath);

            return this;
        }
    }

    public class StructureTypeConfigurator<T> : IStructureTypeConfigurator<T> where T : class
    {
        protected readonly IStructureTypeConfigurator InternalConfigurator;

        public IStructureTypeConfig Config
        {
            get { return InternalConfigurator.Config; }
        }

        public StructureTypeConfigurator(IStructureTypeConfig config)
        {
            InternalConfigurator = new StructureTypeConfigurator(config);
        }

        public virtual IStructureTypeConfigurator<T> AllowNestedStructures()
        {
            Config.IncludeContainedStructureMembers = true;

            return this;
        }

        public virtual IStructureTypeConfigurator<T> OnlyIndexThis(params string[] memberPaths)
        {
            InternalConfigurator.OnlyIndexThis(memberPaths);

            return this;
        }

        public virtual IStructureTypeConfigurator<T> OnlyIndexThis(params Expression<Func<T, object>>[] members)
        {
            OnlyIndexThis(members
                .Select(e => e.GetRightMostMember().ToPath())
                .ToArray());

            return this;
        }

        public virtual IStructureTypeConfigurator<T> DoNotIndexThis(params string[] memberPaths)
        {
            InternalConfigurator.DoNotIndexThis(memberPaths);

            return this;
        }

        public virtual IStructureTypeConfigurator<T> DoNotIndexThis(params Expression<Func<T, object>>[] members)
        {
            DoNotIndexThis(members
                .Select(e => e.GetRightMostMember().ToPath())
                .ToArray());

            return this;
        }
    }
}