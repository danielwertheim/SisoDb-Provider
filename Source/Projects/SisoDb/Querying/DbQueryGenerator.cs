using System;
using System.Collections.Generic;
using EnsureThat;
using NCore;
using SisoDb.Dac;
using SisoDb.DbSchema;
using SisoDb.Querying.Lambdas;
using SisoDb.Querying.Lambdas.Converters.Sql;
using SisoDb.Querying.Sql;
using SisoDb.Resources;
using SisoDb.Structures;

namespace SisoDb.Querying
{
    public abstract class DbQueryGenerator : IDbQueryGenerator
    {
        private readonly ILambdaToSqlWhereConverter _whereConverter;
        private readonly ILambdaToSqlSortingConverter _sortingConverter;
        private readonly ILambdaToSqlIncludeConverter _includeConverter;

        protected DbQueryGenerator(
            ILambdaToSqlWhereConverter whereConverter,
            ILambdaToSqlSortingConverter sortingConverter,
            ILambdaToSqlIncludeConverter includeConverter)
        {
            Ensure.That(whereConverter, "whereConverter").IsNotNull();
            Ensure.That(sortingConverter, "sortingConverter").IsNotNull();
            Ensure.That(includeConverter, "includeConverter").IsNotNull();

            _whereConverter = whereConverter;
            _sortingConverter = sortingConverter;
            _includeConverter = includeConverter;
        }

        public SqlQuery GenerateQuery(IQueryCommand queryCommand)
        {
            Ensure.That(queryCommand, "queryCommand").IsNotNull();

            var includes = GenerateIncludes(queryCommand);
            var whereSql = _whereConverter.Convert(queryCommand.StructureSchema, queryCommand.Where);
            var sortings = _sortingConverter.Convert(queryCommand.StructureSchema, queryCommand.Sortings);

            return queryCommand.HasPaging
                ? CreateSqlCommandInfoForPaging(queryCommand, includes, whereSql, sortings)
                : CreateSqlCommandInfo(queryCommand, includes, whereSql, sortings);
        }

        public SqlQuery GenerateWhereQuery(IQueryCommand queryCommand)
        {
            Ensure.That(queryCommand, "queryCommand").IsNotNull();

            if (!queryCommand.HasWhere)
                throw new ArgumentException(ExceptionMessages.SqlQueryGenerator_GenerateWhere);

            var where = _whereConverter.Convert(queryCommand.StructureSchema, queryCommand.Where);
            var queryParameters = where.Parameters;

            return new SqlQuery(where.Sql, queryParameters);
        }

        protected virtual SqlQuery CreateSqlCommandInfo(IQueryCommand queryCommand, IList<SqlInclude> includes, SqlWhere whereSql, IList<SqlSorting> sortings)
        {
            var sql = string.Format("select {0}s.Json{1} from [{2}] as s inner join [{3}] as si on si.[StructureId] = s.[StructureId]{4}{5} group by s.[StructureId], s.[Json] order by {6};",
                GenerateTakeString(queryCommand).AppendWith(" "),
                SqlInclude.ToColumnDefinitionString(includes).PrependWith(", "),
                queryCommand.StructureSchema.GetStructureTableName(), 
                queryCommand.StructureSchema.GetIndexesTableName(),
                SqlInclude.ToJoinString(includes).PrependWith(" "),
                whereSql.Sql.PrependWith(" where "),
                SqlSorting.ToSortingString(sortings, "min({0})"));

            return new SqlQuery(sql, whereSql.Parameters);
        }

        protected virtual SqlQuery CreateSqlCommandInfoForPaging(IQueryCommand queryCommand, IList<SqlInclude> includes, SqlWhere whereSql, IList<SqlSorting> sortings)
        {
            const string sqlFormat = "with pagedRs as ({0}){1};";

            var innerSelect = string.Format("select {0}s.Json{1}, row_number() over (order by {2}) RowNum from [{3}] as s inner join [{4}] as si on si.[StructureId] = s.[StructureId]{5}{6} group by s.[StructureId], s.[Json]",
                GenerateTakeString(queryCommand).AppendWith(" "),
                SqlInclude.ToColumnDefinitionString(includes).PrependWith(", "),
                SqlSorting.ToSortingString(sortings, "min({0})"),
                queryCommand.StructureSchema.GetStructureTableName(),
                queryCommand.StructureSchema.GetIndexesTableName(),
                SqlInclude.ToJoinString(includes).PrependWith(" "),
                whereSql.Sql.PrependWith(" where "));

            var outerSelect = string.Format("select pagedRs.Json{0} from pagedRs where pagedRs.RowNum between @pagingFrom and @pagingTo", 
                SqlInclude.ToColumnDefinitionString(includes).PrependWith(", "));
            
            var sql = string.Format(sqlFormat, innerSelect, outerSelect);

            var takeFromRowNum = (queryCommand.Paging.PageIndex * queryCommand.Paging.PageSize) + 1;
            var takeToRowNum = (takeFromRowNum + queryCommand.Paging.PageSize) - 1;
            var queryParams = new List<IDacParameter>(whereSql.Parameters)
            {
                new DacParameter("@pagingFrom", takeFromRowNum),
                new DacParameter("@pagingTo", takeToRowNum)
            };

            return new SqlQuery(sql, queryParams);
        }

        protected virtual string GenerateTakeString(IQueryCommand queryCommand)
        {
            if (!queryCommand.HasTakeNumOfStructures)
                return string.Empty;

            return string.Format("top({0})", queryCommand.TakeNumOfStructures);
        }

        private IList<SqlInclude> GenerateIncludes(IQueryCommand queryCommand)
        {
            if (!queryCommand.HasIncludes)
                return new List<SqlInclude>();

            IParsedLambda mergedIncludes = null;
            foreach (var include in queryCommand.Includes)
            {
                if (mergedIncludes == null)
                    mergedIncludes = include;
                else
                    mergedIncludes = mergedIncludes.MergeAsNew(include);
            }

            return _includeConverter.Convert(queryCommand.StructureSchema, mergedIncludes);
        }
    }
}