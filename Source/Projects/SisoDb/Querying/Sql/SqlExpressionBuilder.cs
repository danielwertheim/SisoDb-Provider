using System.Linq;
using System.Linq.Expressions;
using EnsureThat;
using NCore;
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
                OnProcessWheres(query.Where, expression);

            if (query.HasSortings)
                OnProcessSortings(query.Sortings, expression);

            if (query.HasIncludes)
            {
                var mergedIncludeLambda = GetMergedIncludeLambda(query);
                OnProcessIncludes(mergedIncludeLambda, expression);
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

        protected virtual void OnProcessWheres(IParsedLambda lambda, SqlExpression expression)
        {
            var builder = new WhereCriteriaBuilder();

        	for (var i = 0; i < lambda.Nodes.Length; i++)
        	{
                if(i > 0)
                    builder.Flush();

        		var node = lambda.Nodes[i];
                if(node is MemberNode)
                {
                    OnProcessWhereMemberNode(lambda, expression, builder, i, (MemberNode)node);
                    continue;
                }

        		if (node is OperatorNode)
        			builder.AddOp((OperatorNode) node);
        		else if (node is ValueNode)
        			builder.AddValue((ValueNode) node);
        		else if (node is NullNode)
        			builder.AddNullValue((NullNode) node);
        		else
        			builder.Sql.Append(node);
        	}

            builder.Flush();

        	var whereCriteria = builder.Sql.Length > 0 
                ? new SqlWhereCriteria(builder.Sql.ToString(), builder.Params.ToArray())
                : SqlWhereCriteria.Empty();

            expression.SetWhereCriteria(whereCriteria);
        }

        protected virtual void OnProcessWhereMemberNode(IParsedLambda lambda, SqlExpression expression, WhereCriteriaBuilder builder, int nodeIndex, MemberNode memberNode)
        {
            var memberIndex = expression.GetExistingOrNewMemberIndexFor(memberNode.Path);
            if (!expression.ContainsWhereMemberFor(memberNode.Path))
                expression.AddWhereMember(new SqlWhereMember(
                    memberIndex,
                    memberNode.Path,
                    memberNode.DataType,
                    memberNode.DataTypeCode));

            if (memberNode is InSetMemberNode)
            {
                OnProcessWhereInSetMemberNode(builder, (InSetMemberNode)memberNode, memberIndex);
                return;
            }

            if (memberNode is LikeMemberNode)
            {
                OnProcessWhereLikeMemberNode(builder, (LikeMemberNode)memberNode, memberIndex);
                return;
            }

            if (memberNode is StringContainsMemberNode)
            {
                OnProcessWhereStringContainsMemberNode(builder, (StringContainsMemberNode)memberNode, memberIndex);
                return;
            }

            if (memberNode is StringEndsWithMemberNode)
            {
                OnProcessWhereStringEndsWithMemberNode(builder, (StringEndsWithMemberNode)memberNode, memberIndex);
                return;
            }

            if(memberNode is StringStartsWithMemberNode)
            {
                OnProccessWhereStringStartsWithMemberNode(builder, (StringStartsWithMemberNode)memberNode, memberIndex);
                return;
            }

            if (memberNode.DataType.IsAnyBoolType())
            {
                var leftNode = lambda.Nodes.PeekLeft(nodeIndex);
                var rightNode = lambda.Nodes.PeekRight(nodeIndex);
                var isImplicitBoolNode = !(leftNode is OperatorNode) && !(rightNode is OperatorNode);

                if (isImplicitBoolNode)
                {
                    OnProcessWhereImplicitBoolMemberNode(builder, memberNode, memberIndex);
                    return;
                }
            }

            builder.AddMember(memberNode, memberIndex);
        }

        protected virtual void OnProcessWhereInSetMemberNode(WhereCriteriaBuilder builder, InSetMemberNode memberNode, int memberIndex)
        {
            builder.AddMember(memberNode, memberIndex);
            builder.AddOp(new OperatorNode(Operator.Like()));
            //builder.AddValue(new ValueNode(string.Concat("%", memberNode.Value, "%").Replace("%%", "%")));
        }

        protected virtual void OnProcessWhereLikeMemberNode(WhereCriteriaBuilder builder, LikeMemberNode memberNode, int memberIndex)
        {
            builder.AddMember(memberNode, memberIndex);
            builder.AddOp(new OperatorNode(Operator.Like()));
            builder.AddValue(new ValueNode(memberNode.Value.ToStringOrNull() ?? "%"));
        }

        protected virtual void OnProcessWhereStringContainsMemberNode(WhereCriteriaBuilder builder, StringContainsMemberNode memberNode, int memberIndex)
        {
            builder.AddMember(memberNode, memberIndex);
            builder.AddOp(new OperatorNode(Operator.Like()));
            builder.AddValue(new ValueNode(string.Concat("%", memberNode.Value, "%").Replace("%%", "%")));
        }

        protected virtual void OnProcessWhereStringEndsWithMemberNode(WhereCriteriaBuilder builder, StringEndsWithMemberNode memberNode, int memberIndex)
        {
            builder.AddMember(memberNode, memberIndex);
            builder.AddOp(new OperatorNode(Operator.Like()));
            builder.AddValue(new ValueNode(string.Concat("%", memberNode.Value)));
        }

        protected virtual void OnProccessWhereStringStartsWithMemberNode(WhereCriteriaBuilder builder, StringStartsWithMemberNode memberNode, int memberIndex)
        {
            builder.AddMember(memberNode, memberIndex);
            builder.AddOp(new OperatorNode(Operator.Like()));
            builder.AddValue(new ValueNode(string.Concat(memberNode.Value, "%")));
        }

        protected virtual void OnProcessWhereImplicitBoolMemberNode(WhereCriteriaBuilder builder, MemberNode memberNode, int memberIndex)
        {
            builder.AddMember(memberNode, memberIndex);
            builder.AddOp(new OperatorNode(Operator.Equal()));
            builder.AddValue(new ValueNode(true));
        }

        protected virtual void OnProcessSortings(IParsedLambda sortingsLambda, SqlExpression expression)
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

        protected virtual void OnProcessIncludes(IParsedLambda includesLambda, SqlExpression expression)
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