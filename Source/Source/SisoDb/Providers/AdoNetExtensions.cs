using System;
using System.Data;

namespace SisoDb.Providers
{
    internal static class AdoNetExtensions
    {
        internal static T GetScalarResult<T>(this IDbCommand cmd)
        {
            var value = cmd.ExecuteScalar();

            if (value == null || value == DBNull.Value)
                return default(T);

            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}