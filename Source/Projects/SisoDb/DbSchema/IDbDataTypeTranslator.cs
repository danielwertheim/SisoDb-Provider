using System;

namespace SisoDb.DbSchema
{
    public interface IDbDataTypeTranslator
    {
        string ToDbType(Type dataType);
    }
}