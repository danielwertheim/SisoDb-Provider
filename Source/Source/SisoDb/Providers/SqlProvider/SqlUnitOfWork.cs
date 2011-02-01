using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SisoDb.Lambdas;
using SisoDb.Lambdas.Processors;
using SisoDb.Providers.SqlProvider.BulkInserts;
using SisoDb.Providers.SqlProvider.DbSchema;
using SisoDb.Querying;
using SisoDb.Resources;
using SisoDb.Serialization;
using SisoDb.Structures;
using SisoDb.Structures.Schemas;

namespace SisoDb.Providers.SqlProvider
{
    internal class SqlUnitOfWork : IUnitOfWork
    {
        private readonly SqlDbClient _dbClient;
        private readonly IIdentityGenerator _identityGenerator;
        private readonly IDbSchemaManager _dbSchemaManager;
        private readonly IStructureSchemas _structureSchemas;
        private readonly IStructureBuilder _structureBuilder;
        private readonly ISqlQueryGenerator _sqlQueryGenerator;
        private readonly IBatchDeserializer _batchDeserializer;

        public SqlUnitOfWork(
            SqlDbClient dbClient, IIdentityGenerator identityGenerator,
            IDbSchemaManager dbSchemaManager,
            IStructureSchemas structureSchemas, IStructureBuilder structureBuilder)
        {
            _dbClient = dbClient;
            _identityGenerator = identityGenerator;
            _dbSchemaManager = dbSchemaManager;
            _structureSchemas = structureSchemas;
            _structureBuilder = structureBuilder;
            _sqlQueryGenerator = new SqlQueryGenerator(
                new LambdaParser(),
                new ParsedLambdaSqlProcessor(SisoDbEnvironment.MemberNameGenerator));

            //TODO: To use or not to use?!? Cuts's time but increases memoryconsumption.
            _batchDeserializer = new ParallelJsonBatchDeserializer();
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
            var structures = new IStructure[items.Count]; //TODO: Use Queue and Parallel task and batch to keep down memory consumption.
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

        public bool DeleteById<T>(Guid id) where T : class
        {
            return DeleteById<T>(StructureId.NewGuidId(id));
        }

        public bool DeleteById<T>(int id) where T : class
        {
            return DeleteById<T>(StructureId.NewIdentityId(id));
        }

        private bool DeleteById<T>(IStructureId structureId) where T : class
        {
            var structureSchema = _structureSchemas.GetSchema<T>();
            UpsertStructureSet(structureSchema);

            var affectedRowCount = _dbClient.DeleteById(
                structureId.Value,
                structureSchema.GetStructureTableName(),
                structureSchema.GetIndexesTableName(),
                structureSchema.GetUniquesTableName());

            return affectedRowCount > 0;
        }

        public int Count<T>() where T : class
        {
            var structureSchema = _structureSchemas.GetSchema<T>();
            UpsertStructureSet(structureSchema);

            return _dbClient.RowCount(structureSchema.GetStructureTableName());
        }

        public T GetById<T>(Guid id) where T : class
        {
            return GetById<T>(StructureId.NewGuidId(id));
        }

        public T GetById<T>(int id) where T : class
        {
            return GetById<T>(StructureId.NewIdentityId(id));
        }

        private T GetById<T>(IStructureId structureId) where T : class
        {
            var json = GetByIdAsJson<T>(structureId);

            return JsonSerialization.ToItemOrNull<T>(json);
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
            return _batchDeserializer.Deserialize<T>(GetAllAsJson<T>());
            //return GetAllAsJson<T>().Select(JsonSerialization.ToItemOrNull<T>);
        }

        public IEnumerable<string> GetAllAsJson<T>() where T : class
        {
            var structureSchema = _structureSchemas.GetSchema<T>();
            UpsertStructureSet(structureSchema);

            var sql = _dbClient.SqlStrings.GetSql("GetAll").Inject(structureSchema.GetStructureTableName());
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
            //return NamedQueryAsJson<T>(query).Select(JsonSerialization.ToItemOrNull<T>);
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

        public IEnumerable<T> Query<T>(Expression<Func<T, bool>> expression) where T : class
        {
            return _batchDeserializer.Deserialize<T>(QueryAsJson<T>(expression));
            //return QueryAsJson(expression).Select(JsonSerialization.ToItemOrNull<T>);
        }

        public IEnumerable<string> QueryAsJson<T>(Expression<Func<T, bool>> expression) where T : class
        {
            var structureSchema = _structureSchemas.GetSchema<T>();
            UpsertStructureSet(structureSchema);

            var query = _sqlQueryGenerator.Generate(expression, structureSchema);
            var parameters = query.Parameters.Select(p => new QueryParameter(p.Name, p.Value)).ToArray();

            using (var cmd = _dbClient.CreateCommand(CommandType.Text, query.Sql, parameters))
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