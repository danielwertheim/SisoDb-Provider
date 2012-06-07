using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using EnsureThat;
using Microsoft.SqlServer.Server;
using NCore;
using PineCone.Structures;
using NCore.Reflections;
using SisoDb.Resources;

namespace SisoDb.SqlServer
{
    public static class SqlServerTableParams
    {
        public const int MaxStringLength = 50;
        public const int MaxTextLength = 50;

        private static readonly Dictionary<DataTypeCode, Func<string, Type, object[], SqlParameter>> Creators;

        static SqlServerTableParams()
        {
            Creators = new Dictionary<DataTypeCode, Func<string, Type, object[], SqlParameter>>
            {
                { DataTypeCode.IntegerNumber, CreateForIntegers },
                { DataTypeCode.FractalNumber, CreateForFractals },
                { DataTypeCode.Bool, CreateForBooleans },
                { DataTypeCode.DateTime, CreateForDateTimes },
                { DataTypeCode.Guid, CreateForGuids },
                { DataTypeCode.Enum, CreateForStrings },
                { DataTypeCode.String, CreateForStrings },
                { DataTypeCode.Text, CreateForTexts }
            };
        }

        public static SqlParameter Create(string name, Type dataType, DataTypeCode dataTypeCode, object[] values)
        {
            Ensure.That(values, "values").HasItems();

            if(dataTypeCode == DataTypeCode.Unknown)
                throw new SisoDbException("Don't know how to create Table param for data type code: '{0}'".Inject(dataTypeCode));

            return Creators[dataTypeCode].Invoke(name, dataType, values);
        }

        private static SqlParameter CreateForIntegers(string name, Type dataType, object[] values)
        {
            return new SqlParameter(name, SqlDbType.Structured)
            {
                Value = (dataType.IsAnyIntType() 
                    ? values.Cast<int?>().Select(CreateIntegerRecord)
                    : values.Cast<long?>().Select(CreateLongRecord)).ToArray(),
                TypeName = "SisoIntegers"
            };
        }

        private static SqlParameter CreateForFractals(string name, Type dataType, object[] values)
        {
            var p = new SqlParameter(name, SqlDbType.Structured) { TypeName = "SisoFractals" };
            
            if(dataType.IsAnyDecimalType())
            {
                p.Value = values.Cast<decimal?>().Select(CreateFractalRecord).ToArray();
                return p;
            }

            if (dataType.IsAnyDoubleType())
            {
                p.Value = values.Cast<double?>().Select(CreateFractalRecord).ToArray();
                return p;
            }

            if (dataType.IsAnyFloatType())
            {
                p.Value = values.Cast<float?>().Select(CreateFractalRecord).ToArray();
                return p;
            }

            throw new SisoDbException("Don't know how to create table param SisoFractals for data type '{0}'".Inject(dataType.Name));
        }

        private static SqlParameter CreateForBooleans(string name, Type dataType, object[] values)
        {
            return new SqlParameter(name, SqlDbType.Structured)
            {
                Value = values.Cast<bool?>().Select(CreateBooleanRecord).ToArray(),
                TypeName = "SisoBooleans"
            };
        }

        private static SqlParameter CreateForDateTimes(string name, Type dataType, object[] values)
        {
            return new SqlParameter(name, SqlDbType.Structured)
            {
                Value = values.Cast<DateTime?>().Select(CreateDateTimeRecord).ToArray(),
                TypeName = "SisoDates"
            };
        }

        private static SqlParameter CreateForGuids(string name, Type dataType, object[] values)
        {
            return new SqlParameter(name, SqlDbType.Structured)
            {
                Value = values.Cast<Guid?>().Select(CreateGuidRecord).ToArray(),
                TypeName = "SisoGuids"
            };
        }

        private static SqlParameter CreateForStrings(string name, Type dataType, object[] values)
        {
            return new SqlParameter(name, SqlDbType.Structured)
            {
                Value = values.Select(v => v == null
                    ? CreateStringRecord(null)
                    : CreateStringRecord(v.ToString())).ToArray(),
                TypeName = "SisoStrings"
            };
        }

        private static SqlParameter CreateForTexts(string name, Type dataType, object[] values)
        {
            return new SqlParameter(name, SqlDbType.Structured)
            {
                Value = values.Select(v => v == null
                    ? CreateStringRecord(null)
                    : CreateStringRecord(v.ToString())).ToArray(),
                TypeName = "SisoTexts"
            };
        }

        private static SqlDataRecord CreateIntegerRecord(int? value)
        {
            var record = new SqlDataRecord(new SqlMetaData("Value", SqlDbType.Int));

            if(value.HasValue)
                record.SetInt32(0, value.Value);
            else
                record.SetDBNull(0);

            return record;
        }

        private static SqlDataRecord CreateLongRecord(long? value)
        {
            var record = new SqlDataRecord(new SqlMetaData("Value", SqlDbType.BigInt));

            if (value.HasValue)
                record.SetInt64(0, value.Value);
            else
                record.SetDBNull(0);

            return record;
        }

        private static SqlDataRecord CreateFractalRecord(decimal? value)
        {
            var record = new SqlDataRecord(new SqlMetaData("Value", SqlDbType.Decimal));

            if (value.HasValue)
                record.SetDecimal(0, value.Value);
            else
                record.SetDBNull(0);

            return record;
        }

        private static SqlDataRecord CreateFractalRecord(double? value)
        {
            var record = new SqlDataRecord(new SqlMetaData("Value", SqlDbType.Float));

            if (value.HasValue)
                record.SetDouble(0, value.Value);
            else
                record.SetDBNull(0);

            return record;
        }

        private static SqlDataRecord CreateFractalRecord(float? value)
        {
            var record = new SqlDataRecord(new SqlMetaData("Value", SqlDbType.Real));

            if (value.HasValue)
                record.SetFloat(0, value.Value);
            else
                record.SetDBNull(0);

            return record;
        }

        private static SqlDataRecord CreateBooleanRecord(bool? value)
        {
            var record = new SqlDataRecord(new SqlMetaData("Value", SqlDbType.Bit));

            if (value.HasValue)
                record.SetBoolean(0, value.Value);
            else
                record.SetDBNull(0);

            return record;
        }

        private static SqlDataRecord CreateDateTimeRecord(DateTime? value)
        {
            var record = new SqlDataRecord(new SqlMetaData("Value", SqlDbType.DateTime2)); //TODO: Sql2005

            if (value.HasValue)
                record.SetDateTime(0, value.Value);
            else
                record.SetDBNull(0);

            return record;
        }

        private static SqlDataRecord CreateGuidRecord(Guid? value)
        {
            var record = new SqlDataRecord(new SqlMetaData("Value", SqlDbType.UniqueIdentifier));

            if (value.HasValue)
                record.SetGuid(0, value.Value);
            else
                record.SetDBNull(0);

            return record;
        }

        private static SqlDataRecord CreateStringRecord(string value)
        {
            var record = new SqlDataRecord(new SqlMetaData("Value", SqlDbType.NVarChar, MaxStringLength));

            if (!string.IsNullOrWhiteSpace(value))
            {
                if(value.Length > MaxStringLength)
                    throw new SisoDbException(ExceptionMessages.SqlServerTableParams_ToLongString.Inject(MaxStringLength, value));

                record.SetString(0, value);
            }
            else
                record.SetDBNull(0);

            return record;
        }
    }
}