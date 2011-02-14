using System;
using SisoDb.Structures.Schemas.MemberAccessors;

namespace SisoDb.Providers.SqlProvider.DbSchema
{
    public interface ISqlDbDataTypeTranslator
    {
        string ToDbType(IIndexAccessor indexAccessor);
        string ToDbType(Type dataType);
    }
}