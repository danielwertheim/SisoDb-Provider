using System.Linq;
using EnsureThat;
using SisoDb.DbSchema;
using SisoDb.Querying.Lambdas;
using SisoDb.Querying.Lambdas.Nodes;

namespace SisoDb.Querying.Sql
{
    public class SqlExpressionBuilder
    {
        public ISqlExpression Process(IQueryCommand queryCommand)
        {
            Ensure.That(queryCommand, "queryCommand").IsNotNull();

            var expression = new SqlExpression();

            if(queryCommand.HasWhere)
                ProcessWheres(queryCommand.Where, expression);

            if (queryCommand.HasSortings)
                ProcessSortings(queryCommand.Sortings, expression);

            if (queryCommand.HasIncludes)
            {
                var mergedIncludeLambda = GetMergedIncludeLambda(queryCommand);
                ProcessIncludes(mergedIncludeLambda, expression);
            }

            return expression;
        }

        protected virtual IParsedLambda GetMergedIncludeLambda(IQueryCommand queryCommand)
        {
            if (!queryCommand.HasIncludes)
                return null;

            IParsedLambda mergedIncludes = null;
            foreach (var include in queryCommand.Includes)
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

            foreach (var node in wheresLambda.Nodes)
            {
                if (node is MemberNode)
                {
                    var memNode = (MemberNode) node;

                    var memberIndex = expression.GetExistingOrNewMemberIndexFor(memNode.Path);

                    if (!expression.ContainsWhereMemberFor(memNode.Path))
                        expression.AddWhereMember(new SqlWhereMember(memberIndex, memNode.Path, "mem" + memberIndex));
                    
                    builder.AddMember(memNode, memberIndex);
                }
                else if (node is OperatorNode)
                    builder.AddOp((OperatorNode)node);
                else if (node is ValueNode)
                    builder.AddValue((ValueNode)node);
                else if (node is NullNode)
                    builder.AddNullValue((NullNode)node);
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
            if (sortingsLambda == null || sortingsLambda.Nodes.Count == 0)
                return;

            foreach (var sortingNode in sortingsLambda.Nodes.OfType<SortingNode>())
            {
                var valueField = IndexStorageSchema.GetValueSchemaFieldForType(sortingNode.MemberType);

                if(expression.ContainsSortingMemberFor(sortingNode.MemberPath))
                    continue;

                var memberIndex = expression.GetExistingOrNewMemberIndexFor(sortingNode.MemberPath);

                expression.AddSortingMember(new SqlSortingMember(
                    memberIndex,
                    sortingNode.MemberPath,
                    valueField.Name,
                    sortingNode.Direction.ToString()));
            }
        }

        protected virtual void ProcessIncludes(IParsedLambda includesLambda, SqlExpression expression)
        {
            if (includesLambda == null || includesLambda.Nodes.Count == 0)
                return;

            foreach (var includeNode in includesLambda.Nodes.OfType<IncludeNode>())
            {
                expression.AddInclude(new SqlInclude(
                    expression.GetNextNewIncludeIndex(),
                    includeNode.ChildStructureName + "Structure",
                    IndexStorageSchema.GetValueSchemaFieldForType(includeNode.MemberType).Name,
                    includeNode.IdReferencePath,
                    includeNode.ObjectReferencePath));
            }
        }
    }
}