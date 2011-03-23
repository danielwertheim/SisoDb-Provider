using System;
using System.Collections.Generic;
using SisoDb.Core;

namespace SisoDb.Structures.Schemas
{
    public static class StructureType<T>
    {
        private static readonly StructureType State;

        public static string Name
        {
            get { return State.Name; }
        }

        public static IProperty IdProperty
        {
            get { return State.IdProperty; }
        }

        public static IEnumerable<IProperty> IndexableProperties
        {
            get { return State.IndexableProperties; }
        }

        static StructureType()
        {
            State = new StructureType(typeof(T));
        }
    }

    public class StructureType
    {
        private readonly Type _type;

        public string Name { get; private set; }

        public IProperty IdProperty { get; private set; }

        public IEnumerable<IProperty> IndexableProperties { get; private set; }

        public StructureType(Type type)
        {
            _type = type.AssertNotNull("type");
            Name = _type.Name;
            IdProperty = SisoDbEnvironment.StructureTypeReflecter.GetIdProperty(_type);
            IndexableProperties = SisoDbEnvironment.StructureTypeReflecter.GetIndexableProperties(_type, new[] { StructureSchema.IdMemberName });
        }
    }
}