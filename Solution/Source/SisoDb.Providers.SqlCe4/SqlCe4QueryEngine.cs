using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using SisoDb.Core;
using SisoDb.Providers.DbSchema;
using SisoDb.Providers.SqlCe4.Dac;
using SisoDb.Querying;
using SisoDb.Querying.Sql;
using SisoDb.Serialization;
using SisoDb.Structures;
using SisoDb.Structures.Schemas;

namespace SisoDb.Providers.SqlCe4
{
    public class SqlCe4QueryEngine : IQueryEngine
    {
        protected readonly SqlCe4DbClient DbClient;
        protected readonly IDbSchemaManager DbSchemaManager;
        protected readonly IDbSchemaUpserter DbSchemaUpserter;
        protected readonly IStructureSchemas StructureSchemas;
        protected readonly ISqlQueryGenerator QueryGenerator;
        protected readonly IJsonBatchDeserializer JsonBatchDeserializer;
        protected readonly IJsonSerializer JsonSerializer;
        protected readonly ICommandBuilderFactory CommandBuilderFactory;

        protected internal SqlCe4QueryEngine(
            SqlCe4DbClient dbClient,
            IDbSchemaManager dbSchemaManager,
            IDbSchemaUpserter dbSchemaUpserter,
            IStructureSchemas structureSchemas,
            IJsonSerializer jsonSerializer,
            IJsonBatchDeserializer jsonBatchDeserializer,
            ISqlQueryGenerator queryGenerator,
            ICommandBuilderFactory commandBuilderFactory)
        {
            DbClient = dbClient.AssertNotNull("dbClient");
            DbSchemaManager = dbSchemaManager.AssertNotNull("dbSchemaManager");
            DbSchemaUpserter = dbSchemaUpserter.AssertNotNull("dbSchemaUpserter");
            StructureSchemas = structureSchemas.AssertNotNull("structureSchemas");
            JsonSerializer = jsonSerializer.AssertNotNull("jsonSerializer");
            JsonBatchDeserializer = jsonBatchDeserializer.AssertNotNull("jsonBatchDeserializer");
            QueryGenerator = queryGenerator.AssertNotNull("queryGenerator");
            CommandBuilderFactory = commandBuilderFactory.AssertNotNull("commandBuilderFactory");
        }
        public void Dispose()
        {
            DbClient.Dispose();
        }

        public int Count<T>() where T : class
        {
            var structureSchema = StructureSchemas.GetSchema(TypeFor<T>.Type);
            UpsertStructureSet(structureSchema);

            return DbClient.RowCount(structureSchema.GetStructureTableName());
        }

        public int Count<T>(Expression<Func<T, bool>> expression) where T : class
        {
            var structureSchema = StructureSchemas.GetSchema(TypeFor<T>.Type);
            UpsertStructureSet(structureSchema);

            var commandBuilder = CommandBuilderFactory.CreateQueryCommandBuilder<T>();
            var queryCommand = commandBuilder.Where(expression).Command;
            var whereSql = QueryGenerator.GenerateWhere(queryCommand);

            return DbClient.RowCountByQuery(structureSchema.GetIndexesTableName(), whereSql);
        }

        [DebuggerStepThrough]
        public T GetById<T>(Guid sisoId) where T : class
        {
            return GetById<T, T>(SisoId.NewGuidId(sisoId));
        }

        [DebuggerStepThrough]
        public T GetById<T>(int sisoId) where T : class
        {
            return GetById<T, T>(SisoId.NewIdentityId(sisoId));
        }

        [DebuggerStepThrough]
        public IEnumerable<T> GetByIds<T>(IEnumerable<int> ids) where T : class
        {
            return JsonBatchDeserializer.Deserialize<T>(
                GetByIdsAsJson<T>(ids.Select(i => (ValueType)i), IdTypes.Identity));
        }

        [DebuggerStepThrough]
        public IEnumerable<T> GetByIds<T>(IEnumerable<Guid> ids) where T : class
        {
            return JsonBatchDeserializer.Deserialize<T>(
                GetByIdsAsJson<T>(ids.Select(i => (ValueType)i), IdTypes.Guid));
        }

        [DebuggerStepThrough]
        public IEnumerable<T> GetByIdInterval<T>(int idFrom, int idTo) where T : class
        {
            return JsonBatchDeserializer.Deserialize<T>(
                GetJsonWhereIdIsBetween<T>(idFrom, idTo));
        }

        [DebuggerStepThrough]
        public IEnumerable<T> GetByIdInterval<T>(Guid idFrom, Guid idTo) where T : class
        {
            return JsonBatchDeserializer.Deserialize<T>(
                GetJsonWhereIdIsBetween<T>(idFrom, idTo));
        }

        private IEnumerable<string> GetJsonWhereIdIsBetween<T>(ValueType idFrom, ValueType idTo) where T : class
        {
            var structureSchema = StructureSchemas.GetSchema(TypeFor<T>.Type);
            UpsertStructureSet(structureSchema);

            return DbClient.GetJsonWhereIdIsBetween(idFrom, idTo, structureSchema.GetStructureTableName());
        }

        [DebuggerStepThrough]
        public TOut GetByIdAs<TContract, TOut>(Guid sisoId)
            where TContract : class
            where TOut : class
        {
            return GetById<TContract, TOut>(SisoId.NewGuidId(sisoId));
        }

        [DebuggerStepThrough]
        public TOut GetByIdAs<TContract, TOut>(int sisoId)
            where TContract : class
            where TOut : class
        {
            return GetById<TContract, TOut>(SisoId.NewIdentityId(sisoId));
        }

        public IEnumerable<TOut> GetByIdsAs<TContract, TOut>(IEnumerable<int> ids)
            where TContract : class
            where TOut : class
        {
            return JsonBatchDeserializer.Deserialize<TOut>(
                GetByIdsAsJson<TContract>(ids.Select(i => (ValueType)i), IdTypes.Identity));
        }

        public IEnumerable<TOut> GetByIdsAs<TContract, TOut>(IEnumerable<Guid> ids)
            where TContract : class
            where TOut : class
        {
            return JsonBatchDeserializer.Deserialize<TOut>(
                GetByIdsAsJson<TContract>(ids.Select(i => (ValueType)i), IdTypes.Guid));
        }

        private TOut GetById<TContract, TOut>(ISisoId sisoId)
            where TContract : class
            where TOut : class
        {
            return JsonSerializer.ToItemOrNull<TOut>(
                GetByIdAsJson<TContract>(sisoId));
        }

        [DebuggerStepThrough]
        public string GetByIdAsJson<T>(Guid sisoId) where T : class
        {
            return GetByIdAsJson<T>(SisoId.NewGuidId(sisoId));
        }

        [DebuggerStepThrough]
        public string GetByIdAsJson<T>(int sisoId) where T : class
        {
            return GetByIdAsJson<T>(SisoId.NewIdentityId(sisoId));
        }

        [DebuggerStepThrough]
        public IEnumerable<string> GetByIdsAsJson<T>(IEnumerable<int> ids) where T : class
        {
            return GetByIdsAsJson<T>(ids.Select(i => (ValueType)i), IdTypes.Identity);
        }

        [DebuggerStepThrough]
        public IEnumerable<string> GetByIdsAsJson<T>(IEnumerable<Guid> ids) where T : class
        {
            return GetByIdsAsJson<T>(ids.Select(i => (ValueType)i), IdTypes.Guid);
        }

        private string GetByIdAsJson<T>(ISisoId sisoId) where T : class
        {
            var structureSchema = StructureSchemas.GetSchema(TypeFor<T>.Type);
            UpsertStructureSet(structureSchema);

            return DbClient.GetJsonById(sisoId.Value, structureSchema.GetStructureTableName());
        }

        private IEnumerable<string> GetByIdsAsJson<T>(IEnumerable<ValueType> ids, IdTypes idType) where T : class
        {
            var structureSchema = StructureSchemas.GetSchema(TypeFor<T>.Type);
            UpsertStructureSet(structureSchema);

            return DbClient.GetJsonByIds(
                ids,
                idType,
                structureSchema.GetStructureTableName());
        }

        public IEnumerable<T> GetAll<T>() where T : class
        {
            var command = new GetCommand();

            return JsonBatchDeserializer.Deserialize<T>(
                GetAllAsJson<T>(command));
        }

        public IEnumerable<T> GetAll<T>(Action<IGetCommandBuilder<T>> commandInitializer) where T : class
        {
            var commandBuilder = CommandBuilderFactory.CreateGetCommandBuilder<T>();
            commandInitializer(commandBuilder);

            return JsonBatchDeserializer.Deserialize<T>(
                GetAllAsJson<T>(commandBuilder.Command));
        }

        public IEnumerable<TOut> GetAllAs<TContract, TOut>()
            where TContract : class
            where TOut : class
        {
            return JsonBatchDeserializer.Deserialize<TOut>(
                GetAllAsJson<TContract>());
        }

        public IEnumerable<TOut> GetAllAs<TContract, TOut>(Action<IGetCommandBuilder<TContract>> commandInitializer)
            where TContract : class
            where TOut : class
        {
            var commandBuilder = CommandBuilderFactory.CreateGetCommandBuilder<TContract>();
            commandInitializer(commandBuilder);

            return JsonBatchDeserializer.Deserialize<TOut>(
                GetAllAsJson<TContract>(commandBuilder.Command));
        }

        public IEnumerable<string> GetAllAsJson<T>() where T : class
        {
            var commandBuilder = CommandBuilderFactory.CreateGetCommandBuilder<T>();

            return GetAllAsJson<T>(commandBuilder.Command);
        }

        public IEnumerable<string> GetAllAsJson<T>(Action<IGetCommandBuilder<T>> commandInitializer) where T : class
        {
            var commandBuilder = CommandBuilderFactory.CreateGetCommandBuilder<T>();
            commandInitializer(commandBuilder);

            return GetAllAsJson<T>(commandBuilder.Command);
        }

        private IEnumerable<string> GetAllAsJson<T>(IGetCommand getCommand) where T : class
        {
            getCommand.AssertNotNull("getCommand");

            var structureSchema = StructureSchemas.GetSchema(TypeFor<T>.Type);
            UpsertStructureSet(structureSchema);

            string sql;
            if (getCommand.HasSortings || getCommand.HasIncludes)
            {
                var queryCommand = new QueryCommand(getCommand.Includes) { Sortings = getCommand.Sortings };
                var query = QueryGenerator.Generate(queryCommand, structureSchema);
                sql = query.Sql;
            }
            else
                sql = DbClient.SqlStatements.GetSql("GetAll").Inject(structureSchema.GetStructureTableName());

            using (var cmd = DbClient.CreateCommand(CommandType.Text, sql))
            {
                using (var reader = cmd.ExecuteReader(CommandBehavior.SingleResult | CommandBehavior.SequentialAccess))
                {
                    while (reader.Read())
                    {
                        yield return reader.FieldCount < 2 ? reader.GetString(0) : GetMergedJsonStructure(reader);
                    }
                    reader.Close();
                }
            }
        }

        public IEnumerable<T> NamedQuery<T>(INamedQuery query) where T : class
        {
            return JsonBatchDeserializer.Deserialize<T>(
                NamedQueryAsJson<T>(query));
        }

        public IEnumerable<TOut> NamedQueryAs<TContract, TOut>(INamedQuery query)
            where TContract : class
            where TOut : class
        {
            return JsonBatchDeserializer.Deserialize<TOut>(
                NamedQueryAsJson<TContract>(query));
        }

        public IEnumerable<string> NamedQueryAsJson<T>(INamedQuery query) where T : class
        {
            var structureSchema = StructureSchemas.GetSchema(TypeFor<T>.Type);
            UpsertStructureSet(structureSchema);

            using (var cmd = DbClient.CreateCommand(CommandType.StoredProcedure, query.Name, query.Parameters.ToArray()))
            {
                using (var reader = cmd.ExecuteReader(CommandBehavior.SingleResult | CommandBehavior.SequentialAccess))
                {
                    while (reader.Read())
                    {
                        yield return reader.FieldCount < 2 ? reader.GetString(0) : GetMergedJsonStructure(reader);
                    }
                    reader.Close();
                }
            }
        }

        public IEnumerable<T> Where<T>(Expression<Func<T, bool>> expression)
            where T : class
        {
            return Query<T>(q => q.Where(expression));
        }

        public IEnumerable<TOut> WhereAs<TContract, TOut>(Expression<Func<TContract, bool>> expression)
            where TContract : class
            where TOut : class
        {
            return QueryAs<TContract, TOut>(q => q.Where(expression));
        }

        public IEnumerable<string> WhereAsJson<T>(Expression<Func<T, bool>> expression)
            where T : class
        {
            return QueryAsJson<T>(q => q.Where(expression));
        }

        public IEnumerable<T> Query<T>(Action<IQueryCommandBuilder<T>> commandInitializer)
            where T : class
        {
            var commandBuilder = CommandBuilderFactory.CreateQueryCommandBuilder<T>();
            commandInitializer(commandBuilder);

            return JsonBatchDeserializer.Deserialize<T>(
                QueryAsJson<T>(commandBuilder.Command));
        }

        public IEnumerable<TOut> QueryAs<TContract, TOut>(Action<IQueryCommandBuilder<TContract>> commandInitializer)
            where TContract : class
            where TOut : class
        {
            var commandBuilder = CommandBuilderFactory.CreateQueryCommandBuilder<TContract>();
            commandInitializer(commandBuilder);

            return JsonBatchDeserializer.Deserialize<TOut>(
                QueryAsJson<TContract>(commandBuilder.Command));
        }

        public IEnumerable<string> QueryAsJson<T>(Action<IQueryCommandBuilder<T>> commandInitializer) where T : class
        {
            var commandBuilder = CommandBuilderFactory.CreateQueryCommandBuilder<T>();
            commandInitializer(commandBuilder);

            return QueryAsJson<T>(commandBuilder.Command);
        }

        private IEnumerable<string> QueryAsJson<T>(IQueryCommand queryCommand) where T : class
        {
            queryCommand.AssertNotNull("queryCommand");

            var structureSchema = StructureSchemas.GetSchema(TypeFor<T>.Type);
            UpsertStructureSet(structureSchema);

            var query = QueryGenerator.Generate(queryCommand, structureSchema);
            var parameters = query.Parameters.Select(p => new QueryParameter(p.Name, p.Value)).ToArray();

            using (var cmd = DbClient.CreateCommand(CommandType.Text, query.Sql, parameters))
            {
                using (var reader = cmd.ExecuteReader(CommandBehavior.SingleResult | CommandBehavior.SequentialAccess))
                {
                    while (reader.Read())
                    {
                        yield return reader.FieldCount < 2 ? reader.GetString(0) : GetMergedJsonStructure(reader);
                    }
                    reader.Close();
                }
            }
        }

        private static string GetMergedJsonStructure(IDataRecord dataRecord)
        {
            var sb = new StringBuilder();
            sb.Append(dataRecord.GetString(0));
            sb.Remove(sb.Length - 1, 1);
            sb.Append(",");

            foreach (var childJson in ReadChildJson(dataRecord))
                sb.Append(childJson);

            sb.Append("}");

            return sb.ToString();
        }

        private static IEnumerable<string> ReadChildJson(IDataRecord dataRecord)
        {
            var lastIndex = dataRecord.FieldCount - 1;
            for (var c = 1; c <= lastIndex; c++)
                yield return string.Format("\"{0}\":{1}{2}",
                                           dataRecord.GetName(c),
                                           dataRecord.GetString(c),
                                           (c < lastIndex) ? "," : "");
        }

        private void UpsertStructureSet(IStructureSchema structureSchema)
        {
            DbSchemaManager.UpsertStructureSet(structureSchema, DbSchemaUpserter);
        }
    }
}