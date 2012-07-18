using System;
using NCore;
using NCore.Reflections;
using PineCone.Resources;

namespace PineCone.Structures.Schemas.MemberAccessors
{
    internal static class StructureIdSetters
    {
        internal static ISetter For(StructureIdTypes structureIdType, Type type)
        {
            switch (structureIdType)
            {
                case StructureIdTypes.String:
                    return new StringSetter();
                case StructureIdTypes.Guid:
                    return type.IsNullableGuidType() ? (ISetter)new NullableGuidSetter() : new GuidSetter();
                case StructureIdTypes.Identity:
                    return type.IsNullableIntType() ? (ISetter)new NullableIntSetter() : new IntSetter();
                case StructureIdTypes.BigIdentity:
                    return type.IsNullableLongType() ? (ISetter)new NullableLongSetter() : new LongSetter();
                default:
                    throw new PineConeException(ExceptionMessages.Setter_Unsupported_type.Inject(structureIdType));
            }
        }

        internal interface ISetter
        {
            void SetIdValue<T>(T item, IStructureId id, IStructureProperty property) where T : class;
        }

        private class StringSetter : ISetter
        {
            public void SetIdValue<T>(T item, IStructureId id, IStructureProperty property) where T : class
            {
                property.SetValue(item, id.Value);
            }
        }

        private class GuidSetter : ISetter
        {
            public void SetIdValue<T>(T item, IStructureId id, IStructureProperty property) where T : class
            {
                property.SetValue(item, (Guid)id.Value);
            }
        }

        private class NullableGuidSetter : ISetter
        {
            public void SetIdValue<T>(T item, IStructureId id, IStructureProperty property) where T : class
            {
                property.SetValue(item, (Guid?)id.Value);
            }
        }

        private class IntSetter : ISetter
        {
            public void SetIdValue<T>(T item, IStructureId id, IStructureProperty property) where T : class
            {
                property.SetValue(item, (int)id.Value);
            }
        }

        private class NullableIntSetter : ISetter
        {
            public void SetIdValue<T>(T item, IStructureId id, IStructureProperty property) where T : class
            {
                property.SetValue(item, (int?)id.Value);
            }
        }

        private class LongSetter : ISetter
        {
            public void SetIdValue<T>(T item, IStructureId id, IStructureProperty property) where T : class
            {
                property.SetValue(item, (long)id.Value);
            }
        }

        private class NullableLongSetter : ISetter
        {
            public void SetIdValue<T>(T item, IStructureId id, IStructureProperty property) where T : class
            {
                property.SetValue(item, (long?)id.Value);
            }
        }
    }
}