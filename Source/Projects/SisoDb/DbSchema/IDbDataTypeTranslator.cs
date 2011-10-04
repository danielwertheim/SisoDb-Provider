using System;
using PineCone.Structures.Schemas.MemberAccessors;

namespace SisoDb.DbSchema
{
    public interface IDbDataTypeTranslator
    {
        string ToDbType(IIndexAccessor indexAccessor);
        string ToDbType(Type dataType);
    }
}