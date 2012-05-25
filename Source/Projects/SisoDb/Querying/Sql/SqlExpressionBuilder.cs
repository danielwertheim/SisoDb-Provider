using System.Linq;
using System.Linq.Expressions;
using EnsureThat;
using NCore.Collections;
using SisoDb.DbSchema;
using SisoDb.Querying.Lambdas;
using SisoDb.Querying.Lambdas.Nodes;
using NCore.Reflections;
using SisoDb.Querying.Lambdas.Operators;

namespace SisoDb.Querying.Sql
{
    public class SqlExpressionBuilder : ISqlExpressionBuilder
    {
        public virtual ISqlExpression Process(IQuery query)
        {
            Ensure.That(query, "query").IsNotNull();

            var expression = new SqlExpression();

            if(query.HasWhere)
                ProcessWheres(query.Where, expression);

            if (query.HasSortings)
                ProcessSortings(query.Sortings, expression);

            if (query.HasIncludes)
            {
                var mergedIncludeLambda = GetMergedIncludeLambda(query);
                ProcessIncludes(mergedIncludeLambda, expression);
            }

            return expression;
        }

        protected virtual IParsedLambda GetMergedIncludeLambda(IQuery query)
        {
            if (!query.HasIncludes)
                return null;

            IParsedLambda mergedIncludes = null;
            foreach (var include in query.Includes)
            {
                mergedIncludes = mergedIncludes == null 
                    ? include 
                    : mergedIncludes.MergeAsNew(include);
            }

            return mergedIncludes;
        }

        protected virtual void ProcessWheres(IParsedLambda wheresLambda, SqlExpression expression)
        {
            var builder = new WhereCriteriaBuilder();

        	for (int i = 0; i < wheresLambda.Nodes.Length; i++)
        	{
        		var node = wheresLambda.Nodes[i];
        		if (node is MemberNode)
        		{
        			var memNode = (MemberNode) node;
					var memberIndex = expression.GetExistingOrNewMemberIndexFor(memNode.Path);
					if (!expression.ContainsWhereMemberFor(memNode.Path))
						expression.AddWhereMember(new SqlWhereMember(
                            memberIndex,
                            memNode.Path, 
                            string.Concat("mem", memberIndex), 
                            memNode.DataType,
                            memNode.DataTypeCode));

        			if (memNode.DataType.IsAnyBoolType())
        			{
        				var leftNode = wheresLambda.Nodes.PeekLeft(i);
						var rightNode = wheresLambda.Nodes.PeekRight(i);

						if(!(leftNode is OperatorNode) && !(rightNode is OperatorNode))
						{
							builder.AddMember(memNode, memberIndex);
							builder.AddOp(new OperatorNode(Operator.Create(ExpressionType.Equal)));
							builder.AddValue(new ValueNode(true));
							continue;
						}
        			}

        			builder.AddMember(memNode, memberIndex);
        		}
        		else if (node is OperatorNode)
        			builder.AddOp((OperatorNode) node);
        		else if (node is ValueNode)
        			builder.AddValue((ValueNode) node);
        		else if (node is NullNode)
        			builder.AddNullValue((NullNode) node);
        		else
        			builder.Sql.Append(node);

        		builder.Flush();
        	}

        	var whereCriteria = builder.Sql.Length > 0 
                ? new SqlWhereCriteria(builder.Sql.ToString(), builder.Params.ToArray())
                : SqlWhereCriteria.Empty();

            expression.SetWhereCriteria(whereCriteria);
        }

        protected virtual void ProcessSortings(IParsedLambda sortingsLambda, SqlExpression expression)
        {
            if (sortingsLambda == null || sortingsLambda.Nodes.Length == 0)
                return;

            foreach (var sortingNode in sortingsLambda.Nodes.OfType<SortingNode>())
            {
                var valueField = IndexStorageSchema.Fields.Value;

                if(expression.ContainsSortingMemberFor(sortingNode.MemberPath))
                    continue;

                var memberIndex = expression.GetExistingOrNewMemberIndexFor(sortingNode.MemberPath);

                expression.AddSortingMember(new SqlSortingMember(
                    memberIndex,
                    sortingNode.MemberPath,
                    string.Concat("mem", memberIndex),
                    valueField.Name,
                    sortingNode.Direction.ToString(), 
					sortingNode.DataType,
                    sortingNode.DataTypeCode));
            }
        }

        protected virtual void ProcessIncludes(IParsedLambda includesLambda, SqlExpression expression)
        {
            if (includesLambda == null || includesLambda.Nodes.Length == 0)
                return;

            foreach (var includeNode in includesLambda.Nodes.OfType<IncludeNode>())
            {
                var nextNewIncludeIndex = expression.GetNextNewIncludeIndex();

                expression.AddInclude(new SqlInclude(
                    nextNewIncludeIndex,
                    includeNode.ReferencedStructureName,
                    string.Concat("inc", nextNewIncludeIndex),
                    IndexStorageSchema.Fields.Value.Name,
                    includeNode.IdReferencePath,
                    includeNode.ObjectReferencePath,
					includeNode.DataType,
                    includeNode.DataTypeCode));
            }
        }
    }
}