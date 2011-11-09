using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using EnsureThat;
using NCore;
using PineCone.Structures.Schemas;
using SisoDb.Dac;
using SisoDb.DbSchema;
using SisoDb.Providers;
using SisoDb.Querying;
using SisoDb.Serialization;
using SisoDb.Structures;

namespace SisoDb
{
    public abstract class DbQueryEngine : IQueryEngine
    {
        protected ISisoProviderFactory ProviderFactory { get; private set; }

        protected IDbClient DbClient { get; private set; }
        protected IDbClient DbClientNonTrans { get; private set; }
        protected IDbSchemaManager DbSchemaManager { get; private set; }
        protected IDbSchemaUpserter DbSchemaUpserter { get; private set; }
        protected IStructureSchemas StructureSchemas { get; private set; }
        protected IDbQueryGenerator QueryGenerator { get; private set; }
        protected IJsonSerializer JsonSerializer { get; private set; }

        protected DbQueryEngine(
            ISisoConnectionInfo connectionInfo,
            bool transactional,
            IDbSchemaManager dbSchemaManager,
            IStructureSchemas structureSchemas,
            IJsonSerializer jsonSerializer)
        {
            Ensure.That(connectionInfo, "connectionInfo").IsNotNull();
            Ensure.That(dbSchemaManager, "dbSchemaManager").IsNotNull();
            Ensure.That(structureSchemas, "structureSchemas").IsNotNull();
            Ensure.That(jsonSerializer, "jsonSerializer").IsNotNull();

            ProviderFactory = SisoEnvironment.ProviderFactories.Get(connectionInfo.ProviderType);
            DbClient = ProviderFactory.GetDbClient(connectionInfo, transactional);
            DbClientNonTrans = !DbClient.IsTransactional ? DbClient : ProviderFactory.GetDbClient(connectionInfo, false);
            DbSchemaUpserter = ProviderFactory.GetDbSchemaUpserter(DbClientNonTrans);
            QueryGenerator = ProviderFactory.GetDbQueryGenerator();
            DbSchemaManager = dbSchemaManager;
            StructureSchemas = structureSchemas;
            JsonSerializer = jsonSerializer;
        }

        public void Dispose()
        {
            if (ReferenceEquals(DbClient, DbClientNonTrans))
            {
                (DbClient ?? DbClientNonTrans).Dispose();
                DbClient = null;
                DbClientNonTrans = null;

                return;
            }

            if (DbClientNonTrans != null)
            {
                DbClientNonTrans.Dispose();
                DbClientNonTrans = null;
            }

            if (DbClient != null)
            {
                DbClient.Dispose();
                DbClient = null;
            }
        }

        protected void UpsertStructureSet(IStructureSchema structureSchema)
        {
            DbSchemaManager.UpsertStructureSet(structureSchema, DbSchemaUpserter);
        }

        public virtual int Count<T>() where T : class
        {
            var structureSchema = StructureSchemas.GetSchema(TypeFor<T>.Type);

            UpsertStructureSet(structureSchema);

            return DbClient.RowCount(structureSchema);
        }

        public virtual int Count<T>(Expression<Func<T, bool>> expression) where T : class
        {
            var structureSchema = StructureSchemas.GetSchema(TypeFor<T>.Type);

            UpsertStructureSet(structureSchema);

            var commandBuilder = ProviderFactory.CreateQueryCommandBuilder<T>(structureSchema);
            var queryCommand = commandBuilder.Where(expression).Command;
            var whereSql = QueryGenerator.GenerateQueryReturningStrutureIds(queryCommand);

            return DbClient.RowCountByQuery(structureSchema, whereSql);
        }

        public virtual T GetById<T>(ValueType id) where T : class
        {
            return JsonSerializer.Deserialize<T>(
                GetByIdAsJson<T>(id));
        }

        public virtual IEnumerable<T> GetByIds<T>(params ValueType[] ids) where T : class
        {
            return JsonSerializer.DeserializeMany<T>(GetByIdsAsJson<T>(ids));
        }

        public virtual IEnumerable<T> GetByIdInterval<T>(ValueType idFrom, ValueType idTo) where T : class
        {
            var structureSchema = StructureSchemas.GetSchema(TypeFor<T>.Type);

            UpsertStructureSet(structureSchema);

            return JsonSerializer.DeserializeMany<T>(DbClient.GetJsonWhereIdIsBetween(idFrom, idTo, structureSchema));
        }

        public virtual TOut GetByIdAs<TContract, TOut>(ValueType id)
            where TContract : class
            where TOut : class
        {
            return JsonSerializer.Deserialize<TOut>(
                GetByIdAsJson<TContract>(id));
        }

        public virtual IEnumerable<TOut> GetByIdsAs<TContract, TOut>(params ValueType[] ids)
            where TContract : class
            where TOut : class
        {
            return JsonSerializer.DeserializeMany<TOut>(GetByIdsAsJson<TContract>(ids.Select(i => i).ToArray()));
        }

        public virtual string GetByIdAsJson<T>(ValueType id) where T : class
        {
            var structureSchema = StructureSchemas.GetSchema(TypeFor<T>.Type);

            UpsertStructureSet(structureSchema);

            return DbClient.GetJsonById(id, structureSchema);
        }

        public virtual IEnumerable<string> GetByIdsAsJson<T>(params ValueType[] ids) where T : class
        {
            var structureSchema = StructureSchemas.GetSchema(TypeFor<T>.Type);

            UpsertStructureSet(structureSchema);

            return DbClient.GetJsonByIds(ids, structureSchema.IdAccessor.IdType, structureSchema);
        }

        public virtual IEnumerable<T> GetAll<T>() where T : class
        {
            var command = new GetCommand();

            return JsonSerializer.DeserializeMany<T>(GetAllAsJson<T>(command));
        }

        public virtual IEnumerable<T> GetAll<T>(Action<IGetCommandBuilder<T>> commandInitializer) where T : class
        {
            var commandBuilder = ProviderFactory.CreateGetCommandBuilder<T>();
            commandInitializer(commandBuilder);

            return JsonSerializer.DeserializeMany<T>(GetAllAsJson<T>(commandBuilder.Command));
        }

        public virtual IEnumerable<TOut> GetAllAs<TContract, TOut>()
            where TContract : class
            where TOut : class
        {
            return JsonSerializer.DeserializeMany<TOut>(GetAllAsJson<TContract>());
        }

        public virtual IEnumerable<TOut> GetAllAs<TContract, TOut>(Action<IGetCommandBuilder<TContract>> commandInitializer)
            where TContract : class
            where TOut : class
        {
            var commandBuilder = ProviderFactory.CreateGetCommandBuilder<TContract>();
            commandInitializer(commandBuilder);

            return JsonSerializer.DeserializeMany<TOut>(GetAllAsJson<TContract>(commandBuilder.Command));
        }

        public virtual IEnumerable<string> GetAllAsJson<T>() where T : class
        {
            var commandBuilder = ProviderFactory.CreateGetCommandBuilder<T>();

            return GetAllAsJson<T>(commandBuilder.Command);
        }

        public virtual IEnumerable<string> GetAllAsJson<T>(Action<IGetCommandBuilder<T>> commandInitializer) where T : class
        {
            var commandBuilder = ProviderFactory.CreateGetCommandBuilder<T>();
            commandInitializer(commandBuilder);

            return GetAllAsJson<T>(commandBuilder.Command);
        }

        public virtual IEnumerable<T> NamedQuery<T>(INamedQuery query) where T : class
        {
            return JsonSerializer.DeserializeMany<T>(NamedQueryAsJson<T>(query));
        }

        public virtual IEnumerable<TOut> NamedQueryAs<TContract, TOut>(INamedQuery query)
            where TContract : class
            where TOut : class
        {
            return JsonSerializer.DeserializeMany<TOut>(NamedQueryAsJson<TContract>(query));
        }

        public virtual IEnumerable<string> NamedQueryAsJson<T>(INamedQuery query) where T : class
        {
            var structureSchema = StructureSchemas.GetSchema(TypeFor<T>.Type);

            UpsertStructureSet(structureSchema);

            return ConsumeReader(query.Name, true, query.Parameters.ToArray());
        }

        public virtual IEnumerable<T> Where<T>(Expression<Func<T, bool>> expression)
            where T : class
        {
            return Query<T>(q => q.Where(expression));
        }

        public virtual IEnumerable<TOut> WhereAs<TContract, TOut>(Expression<Func<TContract, bool>> expression)
            where TContract : class
            where TOut : class
        {
            return QueryAs<TContract, TOut>(q => q.Where(expression));
        }

        public virtual IEnumerable<string> WhereAsJson<T>(Expression<Func<T, bool>> expression)
            where T : class
        {
            return QueryAsJson<T>(q => q.Where(expression));
        }

        public virtual IEnumerable<T> Query<T>(Action<IQueryCommandBuilder<T>> commandInitializer)
            where T : class
        {
            var structureSchema = StructureSchemas.GetSchema(TypeFor<T>.Type);
            UpsertStructureSet(structureSchema);

            var commandBuilder = ProviderFactory.CreateQueryCommandBuilder<T>(structureSchema);
            commandInitializer(commandBuilder);

            return JsonSerializer.DeserializeMany<T>(QueryAsJson(commandBuilder.Command));
        }

        public virtual IEnumerable<TOut> QueryAs<TContract, TOut>(Action<IQueryCommandBuilder<TContract>> commandInitializer)
            where TContract : class
            where TOut : class
        {
            var structureSchema = StructureSchemas.GetSchema(TypeFor<TContract>.Type);
            UpsertStructureSet(structureSchema);

            var commandBuilder = ProviderFactory.CreateQueryCommandBuilder<TContract>(structureSchema);
            commandInitializer(commandBuilder);

            return JsonSerializer.DeserializeMany<TOut>(QueryAsJson(commandBuilder.Command));
        }

        public virtual IEnumerable<string> QueryAsJson<T>(Action<IQueryCommandBuilder<T>> commandInitializer) where T : class
        {
            var structureSchema = StructureSchemas.GetSchema(TypeFor<T>.Type);
            UpsertStructureSet(structureSchema);

            var commandBuilder = ProviderFactory.CreateQueryCommandBuilder<T>(structureSchema);
            commandInitializer(commandBuilder);

            return QueryAsJson(commandBuilder.Command);
        }

        private IEnumerable<string> GetAllAsJson<T>(IGetCommand getCommand) where T : class
        {
            Ensure.That(getCommand, "getCommand").IsNotNull();

            var structureSchema = StructureSchemas.GetSchema(TypeFor<T>.Type);

            UpsertStructureSet(structureSchema);

            string sql;
            if (getCommand.HasSortings || getCommand.HasIncludes)
            {
                var queryCommand = new QueryCommand(structureSchema)
                {
                    Includes = getCommand.Includes,
                    Sortings = getCommand.Sortings
                };
                var query = QueryGenerator.GenerateQuery(queryCommand);
                sql = query.Sql;
            }
            else
                sql = DbClient.SqlStatements.GetSql("GetAll").Inject(structureSchema.GetStructureTableName());

            return ConsumeReader(sql, false);
        }

        private IEnumerable<string> QueryAsJson(IQueryCommand queryCommand)
        {
            Ensure.That(queryCommand, "queryCommand").IsNotNull();

            var query = QueryGenerator.GenerateQuery(queryCommand);
            var parameters = query.Parameters.Select(p => new DacParameter(p.Name, p.Value)).ToArray();

            return ConsumeReader(query.Sql, false, parameters);
        }

        private IEnumerable<string> ConsumeReader(string sql, bool isStoredProcedure, params IDacParameter[] parameters)
        {
            using (var cmd = !isStoredProcedure ? DbClient.CreateCommand(sql, parameters) : DbClient.CreateSpCommand(sql, parameters))
            {
                using (var reader = cmd.ExecuteReader(CommandBehavior.SingleResult | CommandBehavior.SequentialAccess))
                {
                    Func<IDataRecord, IDictionary<int, string>, string> read;
                    IDictionary<int, string> additionalJsonFields = null;

                    if (reader.FieldCount == 1)
                        read = (dr, af) => dr.GetString(0);
                    else
                    {
                        additionalJsonFields = GetAdditionalJsonFields(reader);
                        read = GetMergedJsonStructure;
                    }

                    while (reader.Read())
                    {
                        yield return read.Invoke(reader, additionalJsonFields);
                    }

                    reader.Close();
                }
            }
        }

        private IDictionary<int, string> GetAdditionalJsonFields(IDataRecord dataRecord)
        {
            var indices = new Dictionary<int, string>();
            for(var i = 1; i < dataRecord.FieldCount; i++)
            {
                var name = dataRecord.GetName(i); 
                if(name.Contains(StructureStorageSchema.Fields.Json.Name))
                    indices.Add(i, name);
                else
                    break;
            }
            return indices;
        }

        private static string GetMergedJsonStructure(IDataRecord dataRecord, IDictionary<int, string> additionalJsonFields)
        {
            var sb = new StringBuilder();
            sb.Append(dataRecord.GetString(0));
            sb = sb.Remove(sb.Length - 1, 1);
            
            foreach (var childJson in ReadChildJson(dataRecord, additionalJsonFields))
            {
                sb.Append(",");
                sb.Append(childJson);
            }

            sb.Append("}");

            return sb.ToString();
        }

        private static IEnumerable<string> ReadChildJson(IDataRecord dataRecord, IDictionary<int, string> additionalJsonFields)
        {
            return additionalJsonFields.Select(additionalJsonField => 
                string.Format("\"{0}\":{1}",
                additionalJsonField.Value.Replace(StructureStorageSchema.Fields.Json.Name, string.Empty),
                dataRecord.GetString(additionalJsonField.Key)));
        }
    }
}