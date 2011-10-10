using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.SqlServer.Server;
using NCore;
using PineCone.Structures;
using SisoDb.Sql2008.Resources;

namespace SisoDb.Sql2008.Dac
{
    internal static class Sql2008IdsTableParam
    {
        internal static SqlParameter CreateIdsTableParam(StructureIdTypes idType, IEnumerable<ValueType> ids)
        {
            switch (idType)
            {
                case StructureIdTypes.Guid:
                    return CreateGuidIdsTableParam(ids);
                case StructureIdTypes.Identity:
                    return CreateBigIdentityIdsTableParam(ids);
            }

            throw new SisoDbException(Sql2008Exceptions.SqlIdsTableParam_CreateIdsTableParam.Inject(idType));
        }

        private static SqlParameter CreateBigIdentityIdsTableParam(IEnumerable<ValueType> ids)
        {
            return new SqlParameter("@ids", SqlDbType.Structured)
            {
                Value = ids.Select(id => CreateBigIdentityIdRecord((long)id)),
                TypeName = "dbo.StructureIdentityIds"
            };
        }

        private static SqlDataRecord CreateBigIdentityIdRecord(long id)
        {
            var record = new SqlDataRecord(new SqlMetaData("Id", SqlDbType.BigInt));

            record.SetInt64(0, id);

            return record;
        }

        private static SqlParameter CreateGuidIdsTableParam(IEnumerable<ValueType> ids)
        {
            return new SqlParameter("@ids", SqlDbType.Structured)
            {
                Value = ids.Select(id => CreateGuidIdRecord((Guid)id)),
                TypeName = "dbo.SisoGuidIds"
            };
        }

        private static SqlDataRecord CreateGuidIdRecord(Guid id)
        {
            var record = new SqlDataRecord(new SqlMetaData("Id", SqlDbType.UniqueIdentifier));

            record.SetGuid(0, id);

            return record;
        }
    }
}