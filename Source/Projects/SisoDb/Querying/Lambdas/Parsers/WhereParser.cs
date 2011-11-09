using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NCore;
using NCore.Expressions;
using NCore.Reflections;
using SisoDb.Core.Expressions;
using SisoDb.Querying.Lambdas.Nodes;
using SisoDb.Querying.Lambdas.Operators;
using SisoDb.Resources;

namespace SisoDb.Querying.Lambdas.Parsers
{
    public class WhereParser : ExpressionVisitor, IWhereParser
    {
        private static readonly HashSet<ExpressionType> SupportedEnumerableQxOperators;
        private readonly object _lock;
        private readonly IExpressionEvaluator _expressionEvaluator;
        private readonly Stack<MemberExpression> _virtualPrefixMembers;
        private NodesContainer _nodesContainer;

        private bool IsFlatteningMembers
        {
            get { return _virtualPrefixMembers.Count > 0; }
        }

        static WhereParser()
        {
            SupportedEnumerableQxOperators = new HashSet<ExpressionType>
            {
                ExpressionType.Equal, ExpressionType.NotEqual,
                ExpressionType.OrElse, ExpressionType.AndAlso
            };
        }

        public WhereParser()
        {
            _lock = new object();
            _expressionEvaluator = new LambdaExpressionEvaluator();
            _virtualPrefixMembers = new Stack<MemberExpression>();
        }

        public IParsedLambda Parse(LambdaExpression e)
        {
            if (e.Body.NodeType == ExpressionType.MemberAccess)
                throw new SisoDbException(ExceptionMessages.LambdaParser_NoMemberExpressions);

            lock (_lock)
            {
                _nodesContainer = new NodesContainer();

                Visit(e);

                return new ParsedLambda(_nodesContainer.ToArray());
            }
        }

        protected override Expression VisitNew(NewExpression node)
        {
            var args = node.Arguments.Cast<ConstantExpression>().Select(c => c.Value).ToArray();
            var value = node.Constructor.Invoke(args);
            var cex = Expression.Constant(value);

            return Visit(cex);
        }

        protected override Expression VisitUnary(UnaryExpression e)
        {
            switch (e.NodeType)
            {
                case ExpressionType.Not:
                    _nodesContainer.AddNode(new OperatorNode(Operator.Create(e.NodeType)));
                    Visit(e.Operand);
                    break;
                case ExpressionType.Convert:
                    if (!e.Type.IsNullablePrimitiveType())
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
                _nodesContainer.AddNode(new StartGroupNode());

            if (e.Left.NodeType == ExpressionType.Parameter)
                Visit(_virtualPrefixMembers.Peek());
            else
                Visit(e.Left);

            if (ExpressionUtils.IsNullConstant(e.Right))
                _nodesContainer.AddNode(new OperatorNode(Operator.IsOrIsNot(e.NodeType)));
            else
                _nodesContainer.AddNode(new OperatorNode(Operator.Create(e.NodeType)));

            Visit(e.Right);

            if (isGroupExpression)
                _nodesContainer.AddNode(new EndGroupNode());

            return e;
        }

        protected override Expression VisitConstant(ConstantExpression e)
        {
            if (ExpressionUtils.IsNullConstant(e))
            {
                _nodesContainer.AddNode(new NullNode());
                return e;
            }

            _nodesContainer.AddNode(new ValueNode(e.Value));

            return e;
        }

        protected override Expression VisitMember(MemberExpression e)
        {
            try
            {
                var value = _expressionEvaluator.Evaluate(e);

                var constantExpression = Expression.Constant(value);

                return Visit(constantExpression);
            }
            catch
            {
                var memberNode = CreateNewMemberNode(e);
                _nodesContainer.AddNode(memberNode);

                return e;
            }
        }

        private MemberNode CreateNewMemberNode(MemberExpression e)
        {
            var graphLine = new List<MemberExpression> {e};
            
            if (IsFlatteningMembers)
                graphLine.InsertRange(0, _virtualPrefixMembers.Reverse().Where(vir => !graphLine.Any(gl => gl.Equals(vir))));

            if (graphLine.Count < 1)
                return null;

            MemberNode previousNode = null;
            for (var c = 0; c < graphLine.Count; c++)
            {
                var memberExpression = graphLine[c];
                var isLast = c == (graphLine.Count - 1);
                var path = previousNode == null ? memberExpression.ToPath() : string.Format("{0}.{1}", previousNode.Path, memberExpression.ToPath());

                if (isLast && memberExpression.Type.IsEnumerableType())
                    previousNode = new MemberNode(path, memberExpression.Type.GetEnumerableElementType());
                else
                    previousNode = new MemberNode(path, memberExpression.Type);
            }

            return previousNode;
        }

        protected override Expression VisitMethodCall(MethodCallExpression e)
        {
            if (e.Method.DeclaringType == typeof(StringQueryExtensions))
                return VisitStringQxMethodCall(e);
            
            if (e.Method.DeclaringType == typeof(EnumerableQueryExtensions))
                return VisitEnumerableQxMethodCall(e);

            try
            {
                var value = _expressionEvaluator.Evaluate(e);
                var constant = Expression.Constant(value);
                Visit(constant);
            }
            catch (Exception ex)
            {
                throw new NotSupportedException(
                    ExceptionMessages.LambdaParser_UnsupportedMethodCall.Inject(e.ToString()), ex);
            }

            return e;
        }

        private Expression VisitStringQxMethodCall(MethodCallExpression e)
        {
            var member = e.Arguments[0];
            var methodName = e.Method.Name;

            switch (methodName)
            {
                case "QxLike":
                case "QxStartsWith":
                case "QxEndsWith":
                case "QxContains":
                    var useSuffix = methodName != "QxLike" && (methodName == "QxStartsWith" || methodName == "QxContains");
                    var usePrefix = methodName != "QxLike" && (methodName == "QxEndsWith" || methodName == "QxContains");
                    var argValue = ((ConstantExpression)e.Arguments[1]).Value;
                    var newValue = string.Format("{0}{1}{2}", usePrefix ? "%" : "", argValue, useSuffix ? "%" : "");
                    var constant = Expression.Constant(newValue);
                    Visit(member);
                    _nodesContainer.AddNode(new OperatorNode(Operator.Like()));
                    Visit(constant);
                    break;
            }

            return e;
        }

        private Expression VisitEnumerableQxMethodCall(MethodCallExpression e)
        {
            var member = (MemberExpression)e.Arguments[0];
            var methodName = e.Method.Name;
            var lambda = e.Arguments[1] as LambdaExpression;

            switch (methodName)
            {
                case "QxAny":
                    EnsureSupportedEnumerableQxOperator(lambda);
                    _virtualPrefixMembers.Push(member);
                    Visit(lambda);
                    _virtualPrefixMembers.Pop();
                    break;
            }

            return e;
        }

        private static void EnsureSupportedEnumerableQxOperator(LambdaExpression e)
        {
            var isSupportedMethodCall = e.Body.NodeType == ExpressionType.Call && ((MethodCallExpression)e.Body).Method.Name == "QxAny";
            
            var operatorIsSupported = isSupportedMethodCall || SupportedEnumerableQxOperators.Contains(e.Body.NodeType);

            if(!operatorIsSupported)
                throw new SisoDbException(ExceptionMessages.WhereParser_QxEnumerables_OperatorNotSupported.Inject(e.Body.NodeType));
        }
    }
}