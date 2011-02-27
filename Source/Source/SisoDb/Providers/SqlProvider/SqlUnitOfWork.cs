using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using SisoDb.Providers.Shared;
using SisoDb.Providers.Shared.DbSchema;
using SisoDb.Providers.SqlProvider.BulkInserts;
using SisoDb.Querying;
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

            //TODO: To use or not to use?!? Cuts's time but increases memoryconsumption.
            _batchDeserializer = new ParallelJsonBatchDeserializer(_jsonSerializer);
            //_batchDeserializer = new SequentialJsonBatchDeserializer();
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

        public void Insert<T>(T item) where T : class
        {
            InsertMany(new[] { item });
        }

        public void InsertMany<T>(IList<T> items) where T : class
        {
            var structureSchema = _structureSchemas.GetSchema<T>();
            UpsertStructureSet(structureSchema);

            DoInsert(structureSchema, items);
        }

        private void DoInsert<T>(IStructureSchema structureSchema, IList<T> items) where T : class
        {
            var numOfItems = items.Count;
            if(numOfItems < 1)
                return;

            var hasIdentity = structureSchema.IdAccessor.IdType == IdTypes.Identity;
            var seed = hasIdentity ? (int?)_identityGenerator.CheckOutAndGetSeed(structureSchema, numOfItems) : null;
            var structures = new IStructure[numOfItems];
            Action<int> itteration = c =>
                                         {
                                             var item = items[c];

                                             if (seed.HasValue)
                                             {
                                                 var id = seed.Value + c;
                                                 structureSchema.IdAccessor.SetValue(item, id);
                                             }

                                             var structure = _structureBuilder.CreateStructure(item, structureSchema);
                                             structures[c] = structure;
                                         };

            if (structures.Length > 10)
                Parallel.For(0, structures.Length, itteration);
            else
                for (var c = 0; c < structures.Length; c++)
                    itteration(c);

            var bulkInserter = new SqlBulkInserter(_dbClient);
            bulkInserter.Insert(structureSchema, structures);
        }

        public void Update<T>(T item) where T : class
        {
            var structureSchema = _structureSchemas.GetSchema<T>();
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

        public void DeleteById<T>(Guid id) where T : class
        {
            DeleteById<T>(StructureId.NewGuidId(id));
        }

        public void DeleteById<T>(int id) where T : class
        {
            DeleteById<T>(StructureId.NewIdentityId(id));
        }

        private void DeleteById<T>(IStructureId structureId) where T : class
        {
            var structureSchema = _structureSchemas.GetSchema<T>();
            UpsertStructureSet(structureSchema);

            _dbClient.DeleteById(
                structureId.Value,
                structureSchema.GetStructureTableName(),
                structureSchema.GetIndexesTableName(),
                structureSchema.GetUniquesTableName());
        }

        public void DeleteByQuery<T>(Expression<Func<T, bool>> expression) where T : class
        {
            expression.AssertNotNull("expression");

            var structureSchema = _structureSchemas.GetSchema<T>();
            UpsertStructureSet(structureSchema);

            var commandBuilder = _commandBuilderFactory.CreateQueryCommandBuilder<T>();
            var queryCommand = commandBuilder.Where(expression).Command;
            var sql = _queryGenerator.GenerateWhere(queryCommand, structureSchema);
            _dbClient.DeleteByQuery(sql,
                structureSchema.IdAccessor.DataType,
                structureSchema.GetStructureTableName(),
                structureSchema.GetIndexesTableName(),
                structureSchema.GetUniquesTableName());
        }

        public int Count<T>() where T : class
        {
            var structureSchema = _structureSchemas.GetSchema<T>();
            UpsertStructureSet(structureSchema);

            return _dbClient.RowCount(structureSchema.GetStructureTableName());
        }

        public T GetById<T>(Guid id) where T : class
        {
            return GetById<T, T>(StructureId.NewGuidId(id));
        }

        public T GetById<T>(int id) where T : class
        {
            return GetById<T, T>(StructureId.NewIdentityId(id));
        }

        public TOut GetByIdAs<TContract, TOut>(Guid id)
            where TContract : class
            where TOut : class
        {
            return GetById<TContract, TOut>(StructureId.NewGuidId(id));
        }

        public TOut GetByIdAs<TContract, TOut>(int id)
            where TContract : class
            where TOut : class
        {
            return GetById<TContract, TOut>(StructureId.NewIdentityId(id));
        }

        private TOut GetById<TContract, TOut>(IStructureId structureId)
            where TContract : class
            where TOut : class
        {
            var json = GetByIdAsJson<TContract>(structureId);

            return _jsonSerializer.ToItemOrNull<TOut>(json);
        }

        public string GetByIdAsJson<T>(Guid id) where T : class
        {
            return GetByIdAsJson<T>(StructureId.NewGuidId(id));
        }

        public string GetByIdAsJson<T>(int id) where T : class
        {
            return GetByIdAsJson<T>(StructureId.NewIdentityId(id));
        }

        private string GetByIdAsJson<T>(IStructureId structureId) where T : class
        {
            var structureSchema = _structureSchemas.GetSchema<T>();
            UpsertStructureSet(structureSchema);

            return _dbClient.GetJsonById(structureId.Value, structureSchema.GetStructureTableName());
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

            var structureSchema = _structureSchemas.GetSchema<T>();
            UpsertStructureSet(structureSchema);

            string sql;
            if (getCommand.HasSortings || getCommand.HasIncludes)
            {
                var queryCommand = new QueryCommand(getCommand.Includes) { Sortings = getCommand.Sortings };
                var query = _queryGenerator.Generate(queryCommand, structureSchema);
                sql = query.Value;
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
            var structureSchema = _structureSchemas.GetSchema<T>();
            UpsertStructureSet(structureSchema);

            using (var cmd = _dbClient.CreateCommand(CommandType.StoredProcedure, query.Name, query.Parameters.ToArray()))
            {
                using (var reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        yield return reader.FieldCount < 2 ? reader.GetString(0) : GetMergedJsonStructure(reader);
                    }
                }
            }
        }

        public IEnumerable<T> Where<T>(Expression<Func<T, bool>> expression) 
            where T : class
        {
            return Query<T>(q => q.Where(expression));
        }

        public IEnumerable<TOut> WhereAs<TContract, TOut>(Expression<Func<TContract, bool>> expression) 
            where TContract : class where TOut : class 
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

            var structureSchema = _structureSchemas.GetSchema<T>();
            UpsertStructureSet(structureSchema);

            var query = _queryGenerator.Generate(queryCommand, structureSchema);
            var parameters = query.Parameters.Select(p => new QueryParameter(p.Name, p.Value)).ToArray();

            using (var cmd = _dbClient.CreateCommand(CommandType.Text, query.Value, parameters))
            {
                using (var reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        yield return reader.FieldCount < 2 ? reader.GetString(0) : GetMergedJsonStructure(reader);
                    }
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