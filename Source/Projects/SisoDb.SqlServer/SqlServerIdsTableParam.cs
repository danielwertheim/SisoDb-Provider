using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.SqlServer.Server;
using SisoDb.NCore;
using SisoDb.PineCone.Structures;

namespace SisoDb.SqlServer
{
    public static class SqlServerIdsTableParam
    {
        public static SqlParameter CreateIdsTableParam(StructureIdTypes idType, IEnumerable<IStructureId> ids)
        {
            if (idType == StructureIdTypes.String)
                return CreateStringIdsTableParam(ids.Select(id => (string)id.Value));

            if(idType == StructureIdTypes.Guid)
                return CreateGuidIdsTableParam(ids.Select(id => (Guid)id.Value));

            if(idType == StructureIdTypes.Identity)
                return CreateIdentityIdsTableParam(ids.Select(id => (int)id.Value));

            if(idType == StructureIdTypes.BigIdentity)
                return CreateBigIdentityIdsTableParam(ids.Select(id => (long)id.Value));

			throw new SisoDbException("Can not create Id-Table parameter for IdType: '{0}'.".Inject(idType));
        }

        private static SqlParameter CreateIdentityIdsTableParam(IEnumerable<int> ids)
        {
            return new SqlParameter("@ids", SqlDbType.Structured)
            {
                Value = ids.Select(id => CreateBigIdentityIdRecord(id)),
                TypeName = "SisoIdentityIds"
            };
        }

        private static SqlParameter CreateBigIdentityIdsTableParam(IEnumerable<long> ids)
        {
            return new SqlParameter("@ids", SqlDbType.Structured)
            {
                Value = ids.Select(CreateBigIdentityIdRecord),
                TypeName = "SisoIdentityIds"
            };
        }

        private static SqlDataRecord CreateBigIdentityIdRecord(long id)
        {
            var record = new SqlDataRecord(new SqlMetaData("Id", SqlDbType.BigInt));

            record.SetInt64(0, id);

            return record;
        }

        private static SqlParameter CreateStringIdsTableParam(IEnumerable<string> ids)
        {
            return new SqlParameter("@ids", SqlDbType.Structured)
            {
                Value = ids.Select(CreateStringIdRecord),
                TypeName = "SisoStringIds"
            };
        }

        private static SqlDataRecord CreateStringIdRecord(string id)
        {
            var record = new SqlDataRecord(new SqlMetaData("Id", SqlDbType.NVarChar, 16));

            record.SetSqlString(0, id);

            return record;
        }

        private static SqlParameter CreateGuidIdsTableParam(IEnumerable<Guid> ids)
        {
            return new SqlParameter("@ids", SqlDbType.Structured)
            {
                Value = ids.Select(CreateGuidIdRecord),
                TypeName = "SisoGuidIds"
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