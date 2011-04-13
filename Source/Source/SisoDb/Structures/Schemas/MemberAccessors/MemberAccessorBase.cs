using System;

namespace SisoDb.Structures.Schemas.MemberAccessors
{
    public abstract class MemberAccessorBase : IMemberAccessor
    {
        protected IStructureProperty Property { get; private set; }

        public string Name { get; private set; }

        public string Path
        {
            get { return Property.Path; }
        }

        public Type DataType
        {
            get { return Property.PropertyType; }
        }

        protected MemberAccessorBase(IStructureProperty property)
        {
            Property = property;
            Name = SisoDbEnvironment.ResourceContainer.ResolveMemberNameGenerator().Generate(property.Path);
        }
    }
}