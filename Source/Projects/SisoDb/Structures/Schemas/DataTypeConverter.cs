using System;
using System.Linq;
using SisoDb.EnsureThat;
using SisoDb.NCore.Reflections;

namespace SisoDb.Structures.Schemas
{
    public class DataTypeConverter : IDataTypeConverter
    {
        private Func<string, bool> _memberNameIsForTextType;

        public static readonly string[] DefaultTextDataTypeConventions = new[] { "Text", "Content", "Description", "Body" };
        
        public Func<string, bool> MemberNameIsForTextType
        {
            get { return _memberNameIsForTextType; }
            set
            {
                Ensure.That(value, "MemberNameIsForTextType").IsNotNull();
                _memberNameIsForTextType = value;
            }
        }

        public DataTypeConverter()
        {
            MemberNameIsForTextType = OnMemberNameIsForTextType;
        }

        protected virtual bool OnMemberNameIsForTextType(string memberName)
        {
            return DefaultTextDataTypeConventions.Any(convention => memberName.EndsWith(convention, Sys.StringComparision));
        }

        public virtual DataTypeCode Convert(IStructureProperty property)
        {
            return Convert(property.ElementDataType ?? property.DataType, property.Name);
        }

        public virtual DataTypeCode Convert(Type dataType, string memberName)
        {
            if (dataType.IsAnySignedIntegerNumberType())
                return DataTypeCode.IntegerNumber;

            if (dataType.IsAnyUnsignedType())
                return DataTypeCode.UnsignedIntegerNumber;

            if (dataType.IsAnyFractalNumberType())
                return DataTypeCode.FractalNumber;

            if (dataType.IsAnyBoolType())
                return DataTypeCode.Bool;

            if (dataType.IsAnyDateTimeType())
                return DataTypeCode.DateTime;

            if (dataType.IsAnyGuidType())
                return DataTypeCode.Guid;

            if (dataType.IsStringType())
            {
                return MemberNameIsForTextType(memberName)
                    ? DataTypeCode.Text
                    : DataTypeCode.String;
            }

            if (dataType.IsAnyEnumType())
                return DataTypeCode.Enum;

            return DataTypeCode.Unknown;
        }
    }
}