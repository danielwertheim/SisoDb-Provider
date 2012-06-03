using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using EnsureThat;
using Microsoft.SqlServer.Server;

namespace SisoDb.SqlServer
{
    public static class SqlServerTableParams
    {
        private static readonly Dictionary<Type, Func<string, object[], SqlParameter>> Creators;

        static SqlServerTableParams()
        {
            Creators = new Dictionary<Type, Func<string, object[], SqlParameter>>
            {
                { typeof (int), CreateForIntegers },
                { typeof (int?), CreateForIntegers },
                { typeof (long), CreateForIntegers },
                { typeof (long?), CreateForIntegers }
            };
        }

        public static SqlParameter Create(string name, object[] values)
        {
            Ensure.That(values, "values").HasItems();

            return Creators[values[0].GetType()].Invoke(name, values);
        }

        private static SqlParameter CreateForIntegers(string name, object[] values)
        {
            return new SqlParameter(name, SqlDbType.Structured)
            {
                Value = values.Cast<long>().Select(CreateIntegerRecord),
                TypeName = "SisoIntegers"
            };
        }

        private static SqlDataRecord CreateIntegerRecord(long value)
        {
            var record = new SqlDataRecord(new SqlMetaData("Value", SqlDbType.BigInt));

            record.SetInt64(0, value);

            return record;
        }
    }
}