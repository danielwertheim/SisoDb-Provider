using System;
using SisoDb.Structures.Schemas.MemberAccessors;

namespace SisoDb.Providers.DbSchema
{
    public interface IDbDataTypeTranslator
    {
        string ToDbType(IIndexAccessor indexAccessor);
        string ToDbType(Type dataType);
    }
}