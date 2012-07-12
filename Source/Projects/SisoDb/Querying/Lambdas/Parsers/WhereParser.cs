using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EnsureThat;
using NCore;
using NCore.Collections;
using NCore.Expressions;
using NCore.Reflections;
using PineCone.Structures.Schemas;
using SisoDb.Core.Expressions;
using SisoDb.Querying.Lambdas.Nodes;
using SisoDb.Querying.Lambdas.Operators;
using SisoDb.Resources;

namespace SisoDb.Querying.Lambdas.Parsers
{
    public class WhereParser : ExpressionVisitor, IWhereParser
    {
        protected static readonly HashSet<ExpressionType> SupportedEnumerableQxOperators;
        private readonly object _lock;
        protected readonly List<MemberExpression> VirtualPrefixMembers;
        protected INodesCollection Nodes;
        protected readonly IDataTypeConverter DataTypeConverter;

        protected bool IsFlatteningMembers
        {
            get { return VirtualPrefixMembers.Count > 0; }
        }

        static WhereParser()
        {
            SupportedEnumerableQxOperators = new HashSet<ExpressionType>
            {
                ExpressionType.Equal, ExpressionType.NotEqual,
                ExpressionType.OrElse, ExpressionType.AndAlso,
                ExpressionType.LessThan, ExpressionType.LessThanOrEqual,
                ExpressionType.GreaterThan, ExpressionType.GreaterThanOrEqual,
            };
        }

        public WhereParser(IDataTypeConverter dataTypeConverter)
        {
            Ensure.That(dataTypeConverter, "dataTypeConverter").IsNotNull();
            
            DataTypeConverter = dataTypeConverter;
            _lock = new object();
            VirtualPrefixMembers = new List<MemberExpression>();
        }

        public virtual IParsedLambda Parse(LambdaExpression e)
        {
            if (e.Body.NodeType == ExpressionType.MemberAccess && !e.Body.Type.IsBoolType())
                throw new SisoDbException(ExceptionMessages.WhereExpressionParser_NoMemberExpressions);

            lock (_lock)
            {
                Nodes = new NodesCollection();

                Visit(e);

                return CreateParsedLambda();
            }
        }

        protected virtual IParsedLambda CreateParsedLambda()
        {
            //PERF: This ugly processing of nodes for boht transformers in one loop is for performance. Perhaps move bach to older cleaner solution
            var nullableNodeTransformer = new NullableNodeTransformer();
            var boolNodeTransformer = new BoolNodeTransformer();

            var newNodes = new NodesCollection();
            var maxIndex = Nodes.Count;
            for (var i = 0; i < Nodes.Count; i++)
            {
                var nullableNodes = nullableNodeTransformer.Transform(maxIndex, ref i, Nodes[i], Nodes);
                var merged = nullableNodes.MergeWith(Nodes.Skip(i + 1)).ToArray();
                var maxIndexInner = nullableNodes.Length;
                for (var iInner = 0; iInner < maxIndexInner; iInner++)
                {
                    var boolNodes = boolNodeTransformer.Transform(maxIndexInner, ref iInner, merged[iInner], new NodesCollection(merged));
                    newNodes.AddNodes(boolNodes);
                }
            }

            return new ParsedLambda(newNodes.ToArray());
        }

        protected override Expression VisitNew(NewExpression node)
        {
            var args = node.Arguments.Cast<ConstantExpression>().Select(c => c.Value).ToArray();
            var value = node.Constructor.Invoke(args); //TODO: Fix
            var cex = Expression.Constant(value);

            return Visit(cex);
        }

        protected override Expression VisitUnary(UnaryExpression e)
        {
            switch (e.NodeType)
            {
                case ExpressionType.Not:
                    Nodes.AddNode(new OperatorNode(Operator.Create(e.NodeType)));
                    Visit(e.Operand);
                    break;
                case ExpressionType.Convert:
                    if (!(e.Type.IsNullablePrimitiveType() || e.Operand.Type.IsAnyEnumType()))
                        throw new NotSupportedException(ExceptionMessages.LambdaParser_VisitUnary_InvalidConvert);
                    Visit(e.Operand);
                    break;
                default:
                    throw new NotSupportedException(ExceptionMessages.LambdaParser_VisitUnary_NotSupported.Inject(e.NodeType));
            }

            return e;
        }

        protected override Expression VisitBinary(BinaryExpression e)
        {
            var isGroupExpression = e.Left is BinaryExpression || e.Right is BinaryExpression;

            if (isGroupExpression)
                Nodes.AddNode(new StartGroupNode());

            if (e.Left.NodeType == ExpressionType.Parameter)
                Visit(VirtualPrefixMembers.LastOrDefault());
            else
                Visit(e.Left);

            if (ExpressionUtils.ExpressionRepresentsNullValue(e.Right))
                Nodes.AddNode(new OperatorNode(Operator.IsOrIsNot(e.NodeType)));
            else
                Nodes.AddNode(new OperatorNode(Operator.Create(e.NodeType)));

            if (e.Left.NodeType == ExpressionType.Convert)
            {
                var convert = (UnaryExpression)e.Left;
                if (convert.Operand.Type.IsAnyEnumType())
                    Visit(Expression.Constant(Enum.Parse(convert.Operand.Type, e.Right.Evaluate().ToString()).ToString()));
                else
                    Visit(e.Right);
            }
            else
                Visit(e.Right);

            if (isGroupExpression)
                Nodes.AddNode(new EndGroupNode());

            return e;
        }

        protected override Expression VisitConstant(ConstantExpression e)
        {
            if (ExpressionUtils.IsNullConstant(e))
            {
                Nodes.AddNode(new NullNode());
                return e;
            }

            Nodes.AddNode(new ValueNode(e.Value));

            return e;
        }

        protected override Expression VisitMember(MemberExpression e)
        {
            var isMember = e.Expression != null && e.Expression.NodeType == ExpressionType.Parameter;
            var isNullableMember = e.Expression != null && e.Expression.NodeType == ExpressionType.MemberAccess
                && ((MemberExpression)e.Expression).Expression != null
                && ((MemberExpression)e.Expression).Expression.NodeType == ExpressionType.Parameter;

            if (isMember || isNullableMember)
            {
                Nodes.AddNode(CreateNewMemberNode(e));

                return e;
            }

            try
            {
                var value = e.Evaluate();
                var constantExpression = Expression.Constant(value);

                return VisitConstant(constantExpression);
            }
            catch
            {
                var memberNode = CreateNewMemberNode(e);
                Nodes.AddNode(memberNode);

                return e;
            }
        }

        protected virtual MemberNode CreateNewMemberNode(MemberExpression e)
        {
            var graphLine = new List<MemberExpression>();

            if (IsFlatteningMembers)
            {
                graphLine.AddRange(VirtualPrefixMembers);

                if (!graphLine.Last().Equals(e))
                    graphLine.Add(e);
            }
            else
                graphLine.Add(e);

            MemberNode previousNode = null;
            for (var c = 0; c < graphLine.Count; c++)
            {
                var memberExpression = graphLine[c];
                var isLast = c == (graphLine.Count - 1);
                var path = previousNode == null ? memberExpression.ToPath() : string.Format("{0}.{1}", previousNode.Path, memberExpression.ToPath());

                if (isLast && memberExpression.Type.IsEnumerableType())
                {
                    var elementType = memberExpression.Type.GetEnumerableElementType();
                    previousNode = new MemberNode(path, elementType, DataTypeConverter.Convert(elementType, path));
                }
                else
                    previousNode = new MemberNode(path, memberExpression.Type, DataTypeConverter.Convert(memberExpression.Type, path));
            }

            if (previousNode != null)
            {
                if (e.Type.IsNullablePrimitiveType())
                    return new NullableMemberNode(previousNode.Path, e.Type, DataTypeConverter.Convert(e.Type, previousNode.Path));

                if (e.Expression.Type.IsNullablePrimitiveType())
                    return new NullableMemberNode(previousNode.Path, e.Expression.Type, DataTypeConverter.Convert(e.Expression.Type, previousNode.Path));
            }

            return previousNode;
        }

        protected override Expression VisitMethodCall(MethodCallExpression e)
        {
            if (e.Method.DeclaringType == typeof(StringQueryExtensions))
                return VisitStringQxMethodCall(e);

            if (e.Method.DeclaringType.IsStringType())
                return VisitStringMethodCall(e);

            if (e.Method.DeclaringType == typeof(EnumerableQueryExtensions))
                return VisitEnumerableQxMethodCall(e);

            if (e.Method.DeclaringType == typeof(SingleValueTypeQueryExtensions))
                return VisitSingleValueTypeQxMethodCall(e);

            try
            {
                Visit(Expression.Constant(e.Evaluate()));
            }
            catch (Exception ex)
            {
                throw new NotSupportedException(
                    ExceptionMessages.LambdaParser_UnsupportedMethodCall.Inject(e.ToString()), ex);
            }

            return e;
        }

        protected virtual Expression VisitStringMethodCall(MethodCallExpression e)
        {
            var member = e.GetRightMostMember();

            var expressionIsNotForMember = member == null || member.Expression.NodeType == ExpressionType.Constant;
            if (expressionIsNotForMember)
                return Visit(Expression.Constant(e.Evaluate()));

            var methodName = e.Method.Name;
            switch (methodName)
            {
                case "Contains":
                    Nodes.AddNode(CreateNewMemberNode(member).ToStringContainsNode(e.Arguments[0].Evaluate().ToStringOrNull()));
                    break;
                case "StartsWith":
                    Nodes.AddNode(CreateNewMemberNode(member).ToStringStartsWithNode(e.Arguments[0].Evaluate().ToStringOrNull()));
                    break;
                case "EndsWith":
                    Nodes.AddNode(CreateNewMemberNode(member).ToStringEndsWithNode(e.Arguments[0].Evaluate().ToStringOrNull()));
                    break;
                case "ToLower":
                    Nodes.AddNode(CreateNewMemberNode(member).ToLowerNode());
                    break;
                case "ToUpper":
                    Nodes.AddNode(CreateNewMemberNode(member).ToUpperNode());
                    break;
            }

            return e;
        }

        protected virtual Expression VisitStringQxMethodCall(MethodCallExpression e)
        {
            var member = e.GetRightMostMember();

            var expressionIsNotForMember = member == null || member.Expression.NodeType == ExpressionType.Constant;
            if (expressionIsNotForMember)
                return Visit(Expression.Constant(e.Evaluate()));

            var methodName = e.Method.Name;

            switch (methodName)
            {
                case "QxContains":
                    Nodes.AddNode(CreateNewMemberNode(member).ToStringContainsNode(e.Arguments[1].Evaluate().ToStringOrNull()));
                    break;
                case "QxStartsWith":
                    Nodes.AddNode(CreateNewMemberNode(member).ToStringStartsWithNode(e.Arguments[1].Evaluate().ToStringOrNull()));
                    break;
                case "QxEndsWith":
                    Nodes.AddNode(CreateNewMemberNode(member).ToStringEndsWithNode(e.Arguments[1].Evaluate().ToStringOrNull()));
                    break;
                case "QxLike":
                    Nodes.AddNode(CreateNewMemberNode(member).ToLikeNode(e.Arguments[1].Evaluate().ToStringOrNull()));
                    break;
                case "QxToLower":
                    Nodes.AddNode(CreateNewMemberNode(member).ToLowerNode());
                    break;
                case "QxToUpper":
                    Nodes.AddNode(CreateNewMemberNode(member).ToUpperNode());
                    break;
                default:
                    throw new SisoDbNotSupportedException("String query extension (Qx) method '{0}', is not supported.".Inject(methodName));
            }

            return e;
        }

        protected virtual Expression VisitSingleValueTypeQxMethodCall(MethodCallExpression e)
        {
            var member = e.GetRightMostMember();
            var methodName = e.Method.Name;

            switch (methodName)
            {
                case "QxIn":
                    Nodes.AddNode(CreateNewMemberNode(member).ToInSetNode(e.Arguments[1].Evaluate() as object[]));
                    break;
                default:
                    throw new SisoDbNotSupportedException("Single value type query extension (Qx) method '{0}', is not supported.".Inject(methodName));
            }

            return e;
        }

        protected virtual Expression VisitEnumerableQxMethodCall(MethodCallExpression e)
        {
            var member = (MemberExpression)e.Arguments[0];
            var methodName = e.Method.Name;

            switch (methodName)
            {
                case "QxAny":
                    VirtualPrefixMembers.Add(member);
                    Visit(e.Arguments[1] as LambdaExpression);
                    VirtualPrefixMembers.RemoveAt(VirtualPrefixMembers.Count - 1);
                    break;
                case "QxIn":
                    VirtualPrefixMembers.Add(member);
                    Nodes.AddNode(CreateNewMemberNode(member).ToInSetNode(e.Arguments[1].Evaluate() as object[]));
                    VirtualPrefixMembers.RemoveAt(VirtualPrefixMembers.Count - 1);
                    break;
                default:
                    throw new SisoDbNotSupportedException("Enumerable query extension (Qx) method '{0}', is not supported.".Inject(methodName));
            }

            return e;
        }
    }
}