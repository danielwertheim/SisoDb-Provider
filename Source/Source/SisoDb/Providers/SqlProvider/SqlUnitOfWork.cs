using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using SisoDb.Core;
using SisoDb.Providers.DbSchema;
using SisoDb.Providers.SqlProvider.BulkInserts;
using SisoDb.Querying;
using SisoDb.Querying.Sql;
using SisoDb.Resources;
using SisoDb.Serialization;
using SisoDb.Structures;
using SisoDb.Structures.Schemas;

namespace SisoDb.Providers.SqlProvider
{
    public class SqlUnitOfWork : IUnitOfWork
    {
        private readonly ISqlDbClient _dbClient;
        private readonly ISqlDbClient _dbClientNonTransactional;
        private readonly IIdentityGenerator _identityGenerator;
        private readonly IDbSchemaManager _dbSchemaManager;
        private readonly IDbSchemaUpserter _dbSchemaUpserter;
        private readonly IStructureSchemas _structureSchemas;
        private readonly IStructureBuilder _structureBuilder;
        private readonly ISqlQueryGenerator _queryGenerator;
        private readonly IBatchDeserializer _batchDeserializer;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly ICommandBuilderFactory _commandBuilderFactory;

        protected internal SqlUnitOfWork(
            ISqlDbClient dbClient, ISqlDbClient dbClientNonTransactional,
            IIdentityGenerator identityGenerator,
            IDbSchemaManager dbSchemaManager, IDbSchemaUpserter dbSchemaUpserter,
            IStructureSchemas structureSchemas, IStructureBuilder structureBuilder,
            IJsonSerializer jsonSerializer, ISqlQueryGenerator queryGenerator,
            ICommandBuilderFactory commandBuilderFactory)
        {
            _dbClient = dbClient.AssertNotNull("dbClient");
            _dbClientNonTransactional = dbClientNonTransactional.AssertNotNull("dbClientNonTransactional");
            _identityGenerator = identityGenerator.AssertNotNull("identityGenerator");
            _dbSchemaManager = dbSchemaManager.AssertNotNull("dbSchemaManager");
            _dbSchemaUpserter = dbSchemaUpserter.AssertNotNull("dbSchemaUpserter");
            _structureSchemas = structureSchemas.AssertNotNull("structureSchemas");
            _structureBuilder = structureBuilder.AssertNotNull("structureBuilder");
            _jsonSerializer = jsonSerializer.AssertNotNull("jsonSerializer");
            _queryGenerator = queryGenerator.AssertNotNull("queryGenerator");
            _commandBuilderFactory = commandBuilderFactory.AssertNotNull("commandBuilderFactory");

            _batchDeserializer = new ParallelJsonBatchDeserializer(_jsonSerializer);
        }

        public void Dispose()
        {
            _dbClient.Dispose();
            _dbClientNonTransactional.Dispose();
        }

        public void Commit()
        {
            _dbClient.Flush();
        }

        [DebuggerStepThrough]
        public void Insert<T>(T item) where T : class
        {
            InsertMany(new[] { item });
        }

        public void InsertJson<T>(string json) where T : class
        {
            Insert(_jsonSerializer.ToItemOrNull<T>(json));
        }

        public void InsertMany<T>(IList<T> items) where T : class
        {
            var structureSchema = _structureSchemas.GetSchema(TypeFor<T>.Type);
            UpsertStructureSet(structureSchema);

            DoInsert(structureSchema, items);
        }

        public void InsertManyJson<T>(IList<string> json) where T : class
        {
            InsertMany(_batchDeserializer.Deserialize<T>(json).ToList());
        }

        private void DoInsert<T>(IStructureSchema structureSchema, IEnumerable<T> items) where T : class 
        {
            if (items.Count() < 1)
                return;

            var hasIdentity = structureSchema.IdAccessor.IdType == IdTypes.Identity;
            var seed = hasIdentity ? (int?)_identityGenerator.CheckOutAndGetSeed(structureSchema, items.Count()) : null;

            var converter = new StructureConverter(structureSchema, _structureBuilder, 1000, seed);
            var bulkInserter = new SqlBulkInserter(_dbClient);
            foreach (var structures in converter.Convert(items))
                bulkInserter.Insert(structureSchema, structures);
        }

        public void Update<T>(T item) where T : class
        {
            var structureSchema = _structureSchemas.GetSchema(TypeFor<T>.Type);
            UpsertStructureSet(structureSchema);

            var updatedStructure = _structureBuilder.CreateStructure(item, structureSchema);

            var existingItem = GetByIdAsJson<T>(updatedStructure.Id);
            if (string.IsNullOrWhiteSpace(existingItem))
                throw new SisoDbException(
                    ExceptionMessages.SqlUnitOfWork_NoItemExistsForUpdate.Inject(updatedStructure.Name, updatedStructure.Id));

            DeleteById<T>(updatedStructure.Id);

            var bulkInserter = new SqlBulkInserter(_dbClient);
            bulkInserter.Insert(structureSchema, new[] { updatedStructure });
        }

        [DebuggerStepThrough]
        public void DeleteById<T>(Guid sisoId) where T : class
        {
            DeleteById<T>(SisoId.NewGuidId(sisoId));
        }

        [DebuggerStepThrough]
        public void DeleteById<T>(int sisoId) where T : class
        {
            DeleteById<T>(SisoId.NewIdentityId(sisoId));
        }

        private void DeleteById<T>(ISisoId sisoId) where T : class
        {
            var structureSchema = _structureSchemas.GetSchema(TypeFor<T>.Type);
            UpsertStructureSet(structureSchema);

            _dbClient.DeleteById(
                sisoId.Value,
                structureSchema.GetStructureTableName(),
                structureSchema.GetIndexesTableName(),
                structureSchema.GetUniquesTableName());
        }

        [DebuggerStepThrough]
        public void DeleteByIds<T>(IEnumerable<int> ids) where T : class
        {
            DeleteByIds<T>(ids.Select(id => (ValueType)id), IdTypes.Identity);
        }

        [DebuggerStepThrough]
        public void DeleteByIds<T>(IEnumerable<Guid> ids) where T : class
        {
            DeleteByIds<T>(ids.Select(id => (ValueType)id), IdTypes.Guid);
        }

        [DebuggerStepThrough]
        public void DeleteByIdInterval<T>(int idFrom, int idTo) where T : class
        {
            DeleteWhereIdIsBetween<T>(idFrom, idTo);
        }

        [DebuggerStepThrough]
        public void DeleteByIdInterval<T>(Guid idFrom, Guid idTo) where T : class
        {
            DeleteWhereIdIsBetween<T>(idFrom, idTo);
        }

        private void DeleteWhereIdIsBetween<T>(ValueType idFrom, ValueType idTo) where T : class
        {
            var structureSchema = _structureSchemas.GetSchema(TypeFor<T>.Type);
            UpsertStructureSet(structureSchema);

            _dbClient.DeleteWhereIdIsBetween(
                idFrom, idTo, 
                structureSchema.GetStructureTableName(), 
                structureSchema.GetIndexesTableName(), 
                structureSchema.GetUniquesTableName());
        }

        private void DeleteByIds<T>(IEnumerable<ValueType> ids, IdTypes idType) where T : class
        {
            var structureSchema = _structureSchemas.GetSchema(TypeFor<T>.Type);
            UpsertStructureSet(structureSchema);

            _dbClient.DeleteByIds(
                ids,
                idType,
                structureSchema.GetStructureTableName(),
                structureSchema.GetIndexesTableName(),
                structureSchema.GetUniquesTableName());
        }

        public void DeleteByQuery<T>(Expression<Func<T, bool>> expression) where T : class
        {
            expression.AssertNotNull("expression");

            var structureSchema = _structureSchemas.GetSchema(TypeFor<T>.Type);
            UpsertStructureSet(structureSchema);

            var commandBuilder = _commandBuilderFactory.CreateQueryCommandBuilder<T>();
            var queryCommand = commandBuilder.Where(expression).Command;
            var sql = _queryGenerator.GenerateWhere(queryCommand);
            _dbClient.DeleteByQuery(sql,
                structureSchema.IdAccessor.DataType,
                structureSchema.GetStructureTableName(),
                structureSchema.GetIndexesTableName(),
                structureSchema.GetUniquesTableName());
        }

        public int Count<T>() where T : class
        {
            var structureSchema = _structureSchemas.GetSchema(TypeFor<T>.Type);
            UpsertStructureSet(structureSchema);

            return _dbClient.RowCount(structureSchema.GetStructureTableName());
        }

        public int Count<T>(Expression<Func<T, bool>> expression) where T : class
        {
            var structureSchema = _structureSchemas.GetSchema(TypeFor<T>.Type);
            UpsertStructureSet(structureSchema);

            var commandBuilder = _commandBuilderFactory.CreateQueryCommandBuilder<T>();
            var queryCommand = commandBuilder.Where(expression).Command;
            var whereSql = _queryGenerator.GenerateWhere(queryCommand);
            
            return _dbClient.RowCountByQuery(structureSchema.GetIndexesTableName(), whereSql);
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
            return _batchDeserializer.Deserialize<T>(
                GetByIdsAsJson<T>(ids.Select(i => (ValueType)i), IdTypes.Identity));
        }

        [DebuggerStepThrough]
        public IEnumerable<T> GetByIds<T>(IEnumerable<Guid> ids) where T : class
        {
            return _batchDeserializer.Deserialize<T>(
                GetByIdsAsJson<T>(ids.Select(i => (ValueType)i), IdTypes.Guid));
        }

        [DebuggerStepThrough]
        public IEnumerable<T> GetByIdInterval<T>(int idFrom, int idTo) where T : class
        {
            return _batchDeserializer.Deserialize<T>(
                GetJsonWhereIdIsBetween<T>(idFrom, idTo));
        }

        [DebuggerStepThrough]
        public IEnumerable<T> GetByIdInterval<T>(Guid idFrom, Guid idTo) where T : class
        {
            return _batchDeserializer.Deserialize<T>(
                GetJsonWhereIdIsBetween<T>(idFrom, idTo));
        }

        private IEnumerable<string> GetJsonWhereIdIsBetween<T>(ValueType idFrom, ValueType idTo) where T : class
        {
            var structureSchema = _structureSchemas.GetSchema(TypeFor<T>.Type);
            UpsertStructureSet(structureSchema);

            return _dbClient.GetJsonWhereIdIsBetween(idFrom, idTo, structureSchema.GetStructureTableName());
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

        public IEnumerable<TOut> GetByIdsAs<TContract, TOut>(IEnumerable<int> ids) where TContract : class where TOut : class
        {
            return _batchDeserializer.Deserialize<TOut>(
                GetByIdsAsJson<TContract>(ids.Select(i => (ValueType)i), IdTypes.Identity));
        }

        public IEnumerable<TOut> GetByIdsAs<TContract, TOut>(IEnumerable<Guid> ids) where TContract : class where TOut : class
        {
            return _batchDeserializer.Deserialize<TOut>(
                 GetByIdsAsJson<TContract>(ids.Select(i => (ValueType)i), IdTypes.Guid));
        }

        private TOut GetById<TContract, TOut>(ISisoId sisoId)
            where TContract : class
            where TOut : class
        {
            return _jsonSerializer.ToItemOrNull<TOut>(
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
            var structureSchema = _structureSchemas.GetSchema(TypeFor<T>.Type);
            UpsertStructureSet(structureSchema);

            return _dbClient.GetJsonById(sisoId.Value, structureSchema.GetStructureTableName());
        }

        private IEnumerable<string> GetByIdsAsJson<T>(IEnumerable<ValueType> ids, IdTypes idType) where T : class
        {
            var structureSchema = _structureSchemas.GetSchema(TypeFor<T>.Type);
            UpsertStructureSet(structureSchema);

            return _dbClient.GetJsonByIds(
                ids,
                idType,
                structureSchema.GetStructureTableName());
        }

        public IEnumerable<T> GetAll<T>() where T : class
        {
            var command = new GetCommand();

            return _batchDeserializer.Deserialize<T>(
                GetAllAsJson<T>(command));
        }

        public IEnumerable<T> GetAll<T>(Action<IGetCommandBuilder<T>> commandInitializer) where T : class
        {
            var commandBuilder = _commandBuilderFactory.CreateGetCommandBuilder<T>();
            commandInitializer(commandBuilder);

            return _batchDeserializer.Deserialize<T>(
                GetAllAsJson<T>(commandBuilder.Command));
        }

        public IEnumerable<TOut> GetAllAs<TContract, TOut>()
            where TContract : class
            where TOut : class
        {
            return _batchDeserializer.Deserialize<TOut>(
                GetAllAsJson<TContract>());
        }

        public IEnumerable<TOut> GetAllAs<TContract, TOut>(Action<IGetCommandBuilder<TContract>> commandInitializer)
            where TContract : class
            where TOut : class
        {
            var commandBuilder = _commandBuilderFactory.CreateGetCommandBuilder<TContract>();
            commandInitializer(commandBuilder);

            return _batchDeserializer.Deserialize<TOut>(
                GetAllAsJson<TContract>(commandBuilder.Command));
        }

        public IEnumerable<string> GetAllAsJson<T>() where T : class
        {
            var commandBuilder = _commandBuilderFactory.CreateGetCommandBuilder<T>();

            return GetAllAsJson<T>(commandBuilder.Command);
        }

        public IEnumerable<string> GetAllAsJson<T>(Action<IGetCommandBuilder<T>> commandInitializer) where T : class
        {
            var commandBuilder = _commandBuilderFactory.CreateGetCommandBuilder<T>();
            commandInitializer(commandBuilder);

            return GetAllAsJson<T>(commandBuilder.Command);
        }

        private IEnumerable<string> GetAllAsJson<T>(IGetCommand getCommand) where T : class
        {
            getCommand.AssertNotNull("getCommand");

            var structureSchema = _structureSchemas.GetSchema(TypeFor<T>.Type);
            UpsertStructureSet(structureSchema);

            string sql;
            if (getCommand.HasSortings || getCommand.HasIncludes)
            {
                var queryCommand = new QueryCommand(getCommand.Includes) { Sortings = getCommand.Sortings };
                var query = _queryGenerator.Generate(queryCommand, structureSchema);
                sql = query.Sql;
            }
            else
                sql = _dbClient.SqlStringsRepository.GetSql("GetAll").Inject(structureSchema.GetStructureTableName());

            using (var cmd = _dbClient.CreateCommand(CommandType.Text, sql))
            {
                using (var reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
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
            return _batchDeserializer.Deserialize<T>(
                NamedQueryAsJson<T>(query));
        }

        public IEnumerable<TOut> NamedQueryAs<TContract, TOut>(INamedQuery query)
            where TContract : class
            where TOut : class
        {
            return _batchDeserializer.Deserialize<TOut>(
                NamedQueryAsJson<TContract>(query));
        }

        public IEnumerable<string> NamedQueryAsJson<T>(INamedQuery query) where T : class
        {
            var structureSchema = _structureSchemas.GetSchema(TypeFor<T>.Type);
            UpsertStructureSet(structureSchema);

            using (var cmd = _dbClient.CreateCommand(CommandType.StoredProcedure, query.Name, query.Parameters.ToArray()))
            {
                using (var reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
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
            var commandBuilder = _commandBuilderFactory.CreateQueryCommandBuilder<T>();
            commandInitializer(commandBuilder);

            return _batchDeserializer.Deserialize<T>(
                QueryAsJson<T>(commandBuilder.Command));
        }

        public IEnumerable<TOut> QueryAs<TContract, TOut>(Action<IQueryCommandBuilder<TContract>> commandInitializer)
            where TContract : class
            where TOut : class
        {
            var commandBuilder = _commandBuilderFactory.CreateQueryCommandBuilder<TContract>();
            commandInitializer(commandBuilder);

            return _batchDeserializer.Deserialize<TOut>(
                QueryAsJson<TContract>(commandBuilder.Command));
        }

        public IEnumerable<string> QueryAsJson<T>(Action<IQueryCommandBuilder<T>> commandInitializer) where T : class
        {
            var commandBuilder = _commandBuilderFactory.CreateQueryCommandBuilder<T>();
            commandInitializer(commandBuilder);

            return QueryAsJson<T>(commandBuilder.Command);
        }

        private IEnumerable<string> QueryAsJson<T>(IQueryCommand queryCommand) where T : class
        {
            queryCommand.AssertNotNull("queryCommand");

            var structureSchema = _structureSchemas.GetSchema(TypeFor<T>.Type);
            UpsertStructureSet(structureSchema);

            var query = _queryGenerator.Generate(queryCommand, structureSchema);
            var parameters = query.Parameters.Select(p => new QueryParameter(p.Name, p.Value)).ToArray();

            using (var cmd = _dbClient.CreateCommand(CommandType.Text, query.Sql, parameters))
            {
                using (var reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
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
            _dbSchemaManager.UpsertStructureSet(structureSchema, _dbSchemaUpserter);
        }
    }
}