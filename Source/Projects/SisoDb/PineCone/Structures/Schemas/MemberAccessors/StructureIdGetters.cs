using System;
using PineCone.Resources;
using SisoDb.NCore;
using SisoDb.NCore.Reflections;

namespace PineCone.Structures.Schemas.MemberAccessors
{
    internal static class StructureIdGetters
    {
        internal static IGetter For(StructureIdTypes structureIdType, Type type)
        {
            switch (structureIdType)
            {
                case StructureIdTypes.String:
                    return new StringGetter();
                case StructureIdTypes.Guid:
                    return type.IsNullableGuidType() ? (IGetter)new NullableGuidGetter() : new GuidGetter();
                case StructureIdTypes.Identity:
                    return type.IsNullableIntType() ? (IGetter)new NullableIntGetter() : new IntGetter();
                case StructureIdTypes.BigIdentity:
                    return type.IsNullableLongType() ? (IGetter)new NullableLongGetter() : new LongGetter();
                default:
                    throw new PineConeException(ExceptionMessages.Getter_Unsupported_type.Inject(structureIdType));
            }
        }

        internal interface IGetter
        {
            IStructureId GetIdValue<T>(T item, IStructureProperty property) where T : class;
        }

        private class StringGetter : IGetter
        {
            public IStructureId GetIdValue<T>(T item, IStructureProperty property) where T : class
            {
                return StructureId.Create((string)property.GetValue(item));
            }
        }

        private class GuidGetter : IGetter
        {
            public IStructureId GetIdValue<T>(T item, IStructureProperty property) where T : class
            {
                return StructureId.Create((Guid)property.GetValue(item));
            }
        }

        private class NullableGuidGetter : IGetter
        {
            public IStructureId GetIdValue<T>(T item, IStructureProperty property) where T : class
            {
                return StructureId.Create((Guid?)property.GetValue(item));
            }
        }

        private class IntGetter : IGetter
        {
            public IStructureId GetIdValue<T>(T item, IStructureProperty property) where T : class
            {
                return StructureId.Create((int)property.GetValue(item));
            }
        }

        private class NullableIntGetter : IGetter
        {
            public IStructureId GetIdValue<T>(T item, IStructureProperty property) where T : class
            {
                return StructureId.Create((int?)property.GetValue(item));
            }
        }

        private class LongGetter : IGetter
        {
            public IStructureId GetIdValue<T>(T item, IStructureProperty property) where T : class
            {
                return StructureId.Create((long)property.GetValue(item));
            }
        }

        private class NullableLongGetter : IGetter
        {
            public IStructureId GetIdValue<T>(T item, IStructureProperty property) where T : class
            {
                return StructureId.Create((long?)property.GetValue(item));
            }
        }
    }
}