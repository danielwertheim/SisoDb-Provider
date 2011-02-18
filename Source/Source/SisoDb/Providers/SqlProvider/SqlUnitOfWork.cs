using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SisoDb.Providers.SqlProvider.BulkInserts;
using SisoDb.Providers.SqlProvider.DbSchema;
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
        private readonly IIdentityGenerator _identityGenerator;
        private readonly IDbSchemaManager _dbSchemaManager;
        private readonly IStructureSchemas _structureSchemas;
        private readonly IStructureBuilder _structureBuilder;
        private readonly ISqlQueryGenerator _queryGenerator;
        private readonly IBatchDeserializer _batchDeserializer;
        private readonly IJsonSerializer _jsonSerializer;

        protected internal SqlUnitOfWork(
            ISqlDbClient dbClient, IIdentityGenerator identityGenerator,
            IDbSchemaManager dbSchemaManager,
            IStructureSchemas structureSchemas, IStructureBuilder structureBuilder,
            IJsonSerializer jsonSerializer, ISqlQueryGenerator queryGenerator)
        {
            _dbClient = dbClient;
            _identityGenerator = identityGenerator;
            _dbSchemaManager = dbSchemaManager;
            _structureSchemas = structureSchemas;
            _structureBuilder = structureBuilder;
            _jsonSerializer = jsonSerializer;
            _queryGenerator = queryGenerator;

            //TODO: To use or not to use?!? Cuts's time but increases memoryconsumption.
            _batchDeserializer = new ParallelJsonBatchDeserializer(_jsonSerializer);
            //_batchDeserializer = new SequentialJsonBatchDeserializer();
        }

        public void Dispose()
        {
            _dbClient.Dispose();
        }

        public void Commit()
        {
            _dbClient.Flush();
        }

        public void Insert<T>(T item) where T : class
        {
            InsertMany(new[] { item });
        }

        public void InsertMany<T>(IEnumerable<T> items) where T : class
        {
            var structureSchema = _structureSchemas.GetSchema<T>();
            UpsertStructureSet(structureSchema);

            var itemsList = items.ToList();
            if (itemsList.Count < 1)
                return;

            DoInsert(structureSchema, itemsList);
        }

        private void DoInsert<T>(IStructureSchema structureSchema, IList<T> items) where T : class
        {
            var hasIdentity = structureSchema.IdAccessor.IdType == IdTypes.Identity;
            var seed = hasIdentity ? (int?)_identityGenerator.CheckOutAndGetSeed(structureSchema, items.Count) : null;
            var structures = new IStructure[items.Count];
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
            var updatedStructure = _structureBuilder.CreateStructure(item, structureSchema);

            var existingItem = GetByIdAsJson<T>(updatedStructure.Id);
            if (string.IsNullOrWhiteSpace(existingItem))
                throw new SisoDbException(
                    ExceptionMessages.SqlUnitOfWork_NoItemExistsForUpdate.Inject(updatedStructure.TypeName, updatedStructure.Id));

            DeleteById<T>(updatedStructure.Id);

            Insert(item);
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

            var queryCommand = new QueryCommand<T>().Where(expression);
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

        public TOut GetByIdAs<TContract, TOut>(Guid id) where TContract : class where TOut : class 
        {
            return GetById<TContract, TOut>(StructureId.NewGuidId(id));
        }

        public TOut GetByIdAs<TContract, TOut>(int id) where TContract : class where TOut : class
        {
            return GetById<TContract, TOut>(StructureId.NewIdentityId(id));
        }

        private TOut GetById<TContract, TOut>(IStructureId structureId) where TContract : class where TOut : class
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
            var command = new GetCommand<T>();

            return _batchDeserializer.Deserialize<T>(GetAllAsJson<T>(command));
        }

        public IEnumerable<T> GetAll<T>(Action<IGetCommand<T>> commandInitializer) where T : class
        {
            var command = new GetCommand<T>();
            commandInitializer(command);

            return _batchDeserializer.Deserialize<T>(GetAllAsJson<T>(command));
        }

        public IEnumerable<TOut> GetAllAs<TContract, TOut>() where TContract : class where TOut : class
        {
            return _batchDeserializer.Deserialize<TOut>(GetAllAsJson<TContract>());
        }

        public IEnumerable<TOut> GetAllAs<TContract, TOut>(Action<IGetCommand<TContract>> commandInitializer) where TContract : class where TOut : class
        {
            var command = new GetCommand<TContract>();
            commandInitializer(command);

            return _batchDeserializer.Deserialize<TOut>(GetAllAsJson<TContract>(command));
        }

        public IEnumerable<string> GetAllAsJson<T>() where T : class
        {
            var command = new GetCommand<T>();

            return GetAllAsJson<T>(command);
        }

        public IEnumerable<string> GetAllAsJson<T>(Action<IGetCommand<T>> commandInitializer) where T : class
        {
            var command = new GetCommand<T>();
            commandInitializer(command);

            return GetAllAsJson<T>(command);
        }

        private IEnumerable<string> GetAllAsJson<T>(IGetCommand<T> getCommand) where T : class
        {
            getCommand.AssertNotNull("getCommand");

            var structureSchema = _structureSchemas.GetSchema<T>();
            UpsertStructureSet(structureSchema);

            string sql;
            if(getCommand.HasSortings)
            {
                var queryCommand = getCommand.HasSortings ? new QueryCommand<T>(getCommand.Sortings) : new QueryCommand<T>();
                var query = _queryGenerator.Generate(queryCommand, structureSchema);
                sql = query.Value;
            }
            else
                sql = _dbClient.SqlStrings.GetSql("GetAll").Inject(structureSchema.GetStructureTableName());

            using (var cmd = _dbClient.CreateCommand(CommandType.Text, sql))
            {
                using (var reader = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        yield return reader.GetString(0);
                    }
                }
            }
        }

        public IEnumerable<T> NamedQuery<T>(INamedQuery query) where T : class
        {
            return _batchDeserializer.Deserialize<T>(NamedQueryAsJson<T>(query));
        }

        public IEnumerable<TOut> NamedQueryAs<TContract, TOut>(INamedQuery query) where TContract : class where TOut : class 
        {
            return _batchDeserializer.Deserialize<TOut>(NamedQueryAsJson<TContract>(query));
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
                        yield return reader.GetString(0);
                    }
                }
            }
        }

        public IEnumerable<T> Where<T>(Expression<Func<T, bool>> expression) where T : class
        {
            var command = new QueryCommand<T>().Where(expression);

            return _batchDeserializer.Deserialize<T>(QueryAsJson<T>(command));
        }

        public IEnumerable<TOut> WhereAs<TContract, TOut>(Expression<Func<TContract, bool>> expression)
            where TContract : class
            where TOut : class
        {
            var command = new QueryCommand<TContract>().Where(expression);

            return _batchDeserializer.Deserialize<TOut>(QueryAsJson<TContract>(command));
        }

        public IEnumerable<string> WhereAsJson<T>(Expression<Func<T, bool>> expression) where T : class
        {
            var command = new QueryCommand<T>().Where(expression);

            return QueryAsJson<T>(command);
        }

        public IEnumerable<T> Query<T>(Action<IQueryCommand<T>> commandInitializer) where T : class
        {
            var command = new QueryCommand<T>();
            commandInitializer(command);

            return _batchDeserializer.Deserialize<T>(QueryAsJson<T>(command));
        }
        
        public IEnumerable<TOut> QueryAs<TContract, TOut>(Action<IQueryCommand<TContract>> commandInitializer) where TContract : class where TOut : class
        {
            var command = new QueryCommand<TContract>();
            commandInitializer(command);

            return _batchDeserializer.Deserialize<TOut>(QueryAsJson<TContract>(command));
        }

        public IEnumerable<string> QueryAsJson<T>(Action<IQueryCommand<T>> commandInitializer) where T : class
        {
            var command = new QueryCommand<T>();
            commandInitializer(command);

            return QueryAsJson<T>(command);
        }

        private IEnumerable<string> QueryAsJson<T>(IQueryCommand<T> queryCommand) where T : class
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
                        yield return reader.GetString(0);
                    }
                }
            }
        }

        private void UpsertStructureSet(IStructureSchema structureSchema)
        {
            _dbSchemaManager.UpsertStructureSet(structureSchema);
        }
    }
}