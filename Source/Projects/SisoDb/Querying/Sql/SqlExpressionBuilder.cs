using System.Linq;
using EnsureThat;
using NCore;
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
                        expression.AddWhereMember(new SqlWhereMember(memNode.Path, "mem" + memberIndex));
                    
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
                    sortingNode.MemberPath,
                    "mem" + memberIndex,
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
                var parentMemberPath = includeNode.IdReferencePath;

                var includeIndex = expression.GetNextNewIncludeIndex();
                
                var columnDefinition = "cs{0}.Json".Inject(includeIndex);
                
                var jsonColumnDefinition = string.Format("min({0}) [{1}Json]", columnDefinition, includeNode.ObjectReferencePath);

                var join = string.Format("left join [{0}] cs{1} on cs{1}.[{2}] = si.[{3}] and si.[{4}] = '{5}'",
                    includeNode.ChildStructureName + "Structure",
                    includeIndex,
                    StructureStorageSchema.Fields.Id.Name,
                    IndexStorageSchema.GetValueSchemaFieldForType(includeNode.MemberType).Name,
                    IndexStorageSchema.Fields.MemberPath.Name,
                    parentMemberPath);

                expression.AddInclude(new SqlInclude(columnDefinition, jsonColumnDefinition, join));
            }
        }
    }
}