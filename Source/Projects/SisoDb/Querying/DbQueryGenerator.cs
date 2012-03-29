using System;
using System.Collections.Generic;
using System.Linq;
using EnsureThat;
using SisoDb.Dac;
using SisoDb.DbSchema;
using SisoDb.Querying.Sql;
using SisoDb.Resources;

namespace SisoDb.Querying
{
    public abstract class DbQueryGenerator : IDbQueryGenerator
    {
        protected readonly ISqlStatements SqlStatements;
        protected readonly SqlExpressionBuilder SqlExpressionBuilder;

        protected DbQueryGenerator(ISqlStatements sqlStatements)
        {
            Ensure.That(sqlStatements, "sqlStatements").IsNotNull();

            SqlStatements = sqlStatements;
            SqlExpressionBuilder = new SqlExpressionBuilder();
        }

        public DbQuery GenerateQuery(IQuery query)
        {
            Ensure.That(query, "query").IsNotNull();

            return CreateSqlQuery(query);
        }

        public DbQuery GenerateQueryReturningStrutureIds(IQuery query)
        {
			Ensure.That(query, "query").IsNotNull();

			if (!query.HasWhere || (query.HasTakeNumOfStructures || query.HasIncludes || query.HasSortings || query.HasPaging))
                throw new ArgumentException(ExceptionMessages.DbQueryGenerator_GenerateQueryReturningStrutureIds);

			return CreateSqlQueryReturningStructureIds(query);
        }

		protected abstract DbQuery CreateSqlQuery(IQuery query);

		protected abstract DbQuery CreateSqlQueryReturningStructureIds(IQuery query);

		protected virtual string GenerateStartString(IQuery query, ISqlExpression sqlExpression)
        {
            return string.Empty;
        }

		protected virtual string GenerateEndString(IQuery query, ISqlExpression sqlExpression)
        {
            return string.Empty;
        }

		protected virtual string GenerateTakeString(IQuery query)
        {
			if (!query.HasTakeNumOfStructures || query.HasPaging)
                return string.Empty;

			return string.Format("top ({0})", query.TakeNumOfStructures);
        }

		protected abstract string GeneratePagingString(IQuery query, ISqlExpression sqlExpression);

		protected virtual string GenerateOrderByMembersString(IQuery queryCommand, ISqlExpression sqlExpression)
		{
			var sortings = sqlExpression.SortingMembers.Select(
				sorting => (sorting.MemberPath != IndexStorageSchema.Fields.StructureId.Name)
					? string.Format("min(mem{0}.[{1}]) mem{0}", sorting.Index, sorting.IndexStorageColumnName)
					: string.Empty).Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();

            return sortings.Length == 0
                ? string.Empty
				: string.Join(", ", sortings);
        }

		protected virtual string GenerateOrderByString(IQuery query, ISqlExpression sqlExpression)
		{
			var sortings = sqlExpression.SortingMembers.Select(
				sorting => (sorting.MemberPath != IndexStorageSchema.Fields.StructureId.Name)
				           	? string.Format("mem{0} {1}", sorting.Index, sorting.Direction)
							: string.Format("s.[{0}] {1}", IndexStorageSchema.Fields.StructureId.Name, sorting.Direction)).ToArray();

            return sortings.Length == 0
                ? string.Empty
                : string.Join(", ", sortings);
        }

		protected virtual string GenerateWhereAndSortingJoins(IQuery query, ISqlExpression sqlExpression)
        {
            var wheres = sqlExpression.WhereMembers.ToList();
            var sortings = sqlExpression.SortingMembers.ToList();

			var indexesTableNames = query.StructureSchema.GetIndexesTableNames();

            var joins = new List<string>(wheres.Count + sortings.Count);

            const string joinFormat = "left join [{0}] mem{1} on mem{1}.[StructureId] = s.[StructureId] and mem{1}.[MemberPath] = '{2}'";

            if (wheres.Count > 0)
            {
                joins.AddRange(wheres.Select(where =>
                    string.Format(joinFormat,
                    indexesTableNames.GetNameByType(where.DataType),
                    where.Index,
                    where.MemberPath)));
            }

            if (sortings.Count > 0)
            {
                joins.AddRange(sortings.Select(sorting =>
                    string.Format(joinFormat,
					indexesTableNames.GetNameByType(sorting.DataType),
                    sorting.Index,
                    sorting.MemberPath)));
            }

            return string.Join(" ", joins.Distinct());
        }

        protected virtual string GenerateWhereCriteriaString(ISqlExpression sqlExpression)
        {
            return sqlExpression.WhereCriteria.CriteriaString;
        }

        protected virtual string GenerateIncludesJoins(IQuery query, ISqlExpression sqlExpression)
        {
            var includes = sqlExpression.Includes.ToList();
			if (includes.Count == 0)
				return string.Empty;

        	var structureTableName = query.StructureSchema.GetStructureTableName();
        	var indexesTableNames = query.StructureSchema.GetIndexesTableNames();

			const string joinFormat = "left join (select si.[StructureId], cs.[Json] [{1}Json] from [{2}] s inner join [{3}] si on si.[StructureId] = s.[StructureId] and si.[MemberPath] = '{4}' left join [{5}] cs on cs.[StructureId] = si.[Value]) inc{0} on inc{0}.[StructureId] = s.[StructureId]";

			return string.Join(" ", includes.Select(include => 
				string.Format(joinFormat,
					include.Index,
					include.ObjectReferencePath,
					structureTableName,
					indexesTableNames.GetNameByType(include.DataType),
					include.MemberPathReference,
					include.TableName)));
        }

        protected virtual string GenerateIncludedJsonMembersString(ISqlExpression sqlExpression)
        {
            var includes = sqlExpression.Includes.ToList();

            return includes.Count == 0
                ? string.Empty
                : string.Join(", ", includes.Select(inc => string.Format("inc{0}.[{1}Json]", inc.Index, inc.ObjectReferencePath)));
        }
    }
}