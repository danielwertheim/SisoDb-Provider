using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using EnsureThat;
using NCore;
using PineCone.Structures;
using PineCone.Structures.Schemas;
using SisoDb.Dac;
using SisoDb.Querying.Sql;
using SisoDb.Structures;

namespace SisoDb.Sql2008.Dac
{
    public class Sql2008DbClient : DbClientBase
    {
        public Sql2008DbClient(ISisoConnectionInfo connectionInfo, bool transactional) 
            : base(connectionInfo, transactional, () => new SqlConnection(connectionInfo.ConnectionString.PlainString))
        {
        }

        public override IDbBulkCopy GetBulkCopy(bool keepIdentities)
        {
            var options = keepIdentities ? SqlBulkCopyOptions.KeepIdentity : SqlBulkCopyOptions.Default;

            return new Sql2008DbBulkCopy((SqlConnection)Connection, options, (SqlTransaction)Transaction);
        }

        public override void Drop(IStructureSchema structureSchema)
        {
            var sql = SqlStatements.GetSql("DropStructureTables").Inject(
                structureSchema.GetIndexesTableName(),
                structureSchema.GetUniquesTableName(),
                structureSchema.GetStructureTableName());

            using (var cmd = CreateCommand(sql, new DacParameter("entityHash", structureSchema.Hash)))
            {
                cmd.ExecuteNonQuery();
            }
        }

        public override void RebuildIndexes(IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            var sql = SqlStatements.GetSql("RebuildIndexes").Inject(
                structureSchema.GetStructureTableName(),
                structureSchema.GetIndexesTableName(),
                structureSchema.GetUniquesTableName());

            using (var cmd = CreateCommand(sql))
            {
                cmd.ExecuteNonQuery();
            }
        }

        public override void DeleteById(ValueType structureId, IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            var sql = SqlStatements.GetSql("DeleteById").Inject(
                structureSchema.GetStructureTableName());

            using (var cmd = CreateCommand(sql, new DacParameter("id", structureId)))
            {
                cmd.ExecuteNonQuery();
            }
        }

        public override void DeleteByIds(IEnumerable<ValueType> ids, StructureIdTypes idType, IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            var sql = SqlStatements.GetSql("DeleteByIds").Inject(
                structureSchema.GetStructureTableName());

            using (var cmd = CreateCommand(sql))
            {
                cmd.Parameters.Add(Sql2008IdsTableParam.CreateIdsTableParam(idType, ids));
                cmd.ExecuteNonQuery();
            }
        }

        public override void DeleteByQuery(SqlQuery query, Type idType, IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            var sql = SqlStatements.GetSql("DeleteByQuery").Inject(
                structureSchema.GetStructureTableName(),
                structureSchema.GetIndexesTableName(),
                query.Sql);

            using (var cmd = CreateCommand(sql, query.Parameters.ToArray()))
            {
                cmd.ExecuteNonQuery();
            }
        }

        public override void DeleteWhereIdIsBetween(ValueType structureIdFrom, ValueType structureIdTo, IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            var sql = SqlStatements.GetSql("DeleteWhereIdIsBetween").Inject(
                structureSchema.GetStructureTableName());

            using (var cmd = CreateCommand(sql, new DacParameter("idFrom", structureIdFrom), new DacParameter("idTo", structureIdTo)))
            {
                cmd.ExecuteNonQuery();
            }
        }

        public override bool TableExists(string name)
        {
            Ensure.That(name, "name").IsNotNullOrWhiteSpace();

            var sql = SqlStatements.GetSql("TableExists");
            var value = ExecuteScalar<string>(sql, new DacParameter("tableName", name));

            return !string.IsNullOrWhiteSpace(value);
        }

        public override int RowCount(IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            var sql = SqlStatements.GetSql("RowCount").Inject(structureSchema.GetStructureTableName());

            return ExecuteScalar<int>(sql);
        }

        public override int RowCountByQuery(IStructureSchema structureSchema, SqlQuery query)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            var sql = SqlStatements.GetSql("RowCountByQuery").Inject(
                structureSchema.GetIndexesTableName(),
                query.Sql);

            return ExecuteScalar<int>(sql, query.Parameters.ToArray());
        }

        public override long CheckOutAndGetNextIdentity(string entityHash, int numOfIds)
        {
            Ensure.That(entityHash, "entityHash").IsNotNullOrWhiteSpace();

            var sql = SqlStatements.GetSql("Sys_Identities_CheckOutAndGetNextIdentity");

            return ExecuteScalar<long>(sql,
                new DacParameter("entityHash", entityHash),
                new DacParameter("numOfIds", numOfIds));
        }

        public override IEnumerable<string> GetJson(IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            var sql = SqlStatements.GetSql("GetAllById").Inject(structureSchema.GetStructureTableName());

            using (var cmd = CreateCommand(sql))
            {
                using (var reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        yield return reader.GetString(0);
                    }
                    reader.Close();
                }
            }
        }

        public override string GetJsonById(ValueType structureId, IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            var sql = SqlStatements.GetSql("GetById").Inject(structureSchema.GetStructureTableName());

            return ExecuteScalar<string>(sql, new DacParameter("id", structureId));
        }

        public override IEnumerable<string> GetJsonByIds(IEnumerable<ValueType> ids, StructureIdTypes idType, IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            var sql = SqlStatements.GetSql("GetByIds").Inject(structureSchema.GetStructureTableName());

            using (var cmd = CreateCommand(sql))
            {
                cmd.Parameters.Add(Sql2008IdsTableParam.CreateIdsTableParam(idType, ids));

                using (var reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        yield return reader.GetString(0);
                    }
                    reader.Close();
                }
            }
        }

        public override IEnumerable<string> GetJsonWhereIdIsBetween(ValueType structureIdFrom, ValueType structureIdTo, IStructureSchema structureSchema)
        {
            Ensure.That(structureSchema, "structureSchema").IsNotNull();

            var sql = SqlStatements.GetSql("GetJsonWhereIdIsBetween").Inject(structureSchema.GetStructureTableName());

            using (var cmd = CreateCommand(sql, new DacParameter("idFrom", structureIdFrom), new DacParameter("idTo", structureIdTo)))
            {
                using (var reader = cmd.ExecuteReader(CommandBehavior.SingleResult | CommandBehavior.SequentialAccess))
                {
                    while (reader.Read())
                    {
                        yield return reader.GetString(0);
                    }
                    reader.Close();
                }
            }
        }
    }
}