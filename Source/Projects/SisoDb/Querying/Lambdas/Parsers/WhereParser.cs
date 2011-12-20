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
    	protected static readonly HashSet<ExpressionType> SupportedEnumerableQxOperators;
        private readonly object _lock;
        private readonly Stack<MemberExpression> _virtualPrefixMembers;
        private INodesCollection _nodes;

        protected bool IsFlatteningMembers
        {
            get { return _virtualPrefixMembers.Count > 0; }
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

        public WhereParser()
        {
            _lock = new object();
            _virtualPrefixMembers = new Stack<MemberExpression>();
        }

        public IParsedLambda Parse(LambdaExpression e)
        {
            if (e.Body.NodeType == ExpressionType.MemberAccess && e.Body.Type != typeof(bool))
                throw new SisoDbException(ExceptionMessages.WhereExpressionParser_NoMemberExpressions);

            lock (_lock)
            {
                _nodes = new NodesCollection();

                Visit(e);

            	return CreateParsedLambda();
            }
        }

		protected IParsedLambda CreateParsedLambda()
		{
			var nodes =
				new BoolNodesTransformer().Transform(
				new NullableNodesTransformer().Transform(_nodes));

			return new ParsedLambda(nodes.ToArray());
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
                    _nodes.AddNode(new OperatorNode(Operator.Create(e.NodeType)));
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
                _nodes.AddNode(new StartGroupNode());

            if (e.Left.NodeType == ExpressionType.Parameter)
                Visit(_virtualPrefixMembers.Peek());
            else
                Visit(e.Left);

            if (ExpressionUtils.IsNullConstant(e.Right))
                _nodes.AddNode(new OperatorNode(Operator.IsOrIsNot(e.NodeType)));
            else
                _nodes.AddNode(new OperatorNode(Operator.Create(e.NodeType)));

            Visit(e.Right);

            if (isGroupExpression)
                _nodes.AddNode(new EndGroupNode());

            return e;
        }

        protected override Expression VisitConstant(ConstantExpression e)
        {
            if (ExpressionUtils.IsNullConstant(e))
            {
                _nodes.AddNode(new NullNode());
                return e;
            }

            _nodes.AddNode(new ValueNode(e.Value));

            return e;
        }

        protected override Expression VisitMember(MemberExpression e)
        {
			//if (e.Expression.NodeType == ExpressionType.Parameter)
			//{
			//    var memberNode = CreateNewMemberNode(e);
			//    _nodes.AddNode(memberNode);

			//    return e;
			//}

            try
            {
                var value = e.Evaluate();

                var constantExpression = Expression.Constant(value);

                return Visit(constantExpression);
            }
            catch
            {
                var memberNode = CreateNewMemberNode(e);
                _nodes.AddNode(memberNode);

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

			if (previousNode != null)
			{
				if (e.Type.IsNullablePrimitiveType())
					return new NullableMemberNode(previousNode.Path, e.Type);

				if (e.Expression.Type.IsNullablePrimitiveType())
					return new NullableMemberNode(previousNode.Path, e.Expression.Type);
			}

        	return previousNode;
        }

        protected override Expression VisitMethodCall(MethodCallExpression e)
        {
            if (e.Method.DeclaringType == typeof(StringQueryExtensions))
                return VisitStringQxMethodCall(e);

            if (e.Method.DeclaringType == typeof(string))
                return VisitStringMethodCall(e);
            
            if (e.Method.DeclaringType == typeof(EnumerableQueryExtensions))
                return VisitEnumerableQxMethodCall(e);

            try
            {
                var value = e.Evaluate();
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

		protected virtual Expression VisitStringMethodCall(MethodCallExpression e)
        {
            var member = (MemberExpression)e.Object;
            var methodName = e.Method.Name;
            
            switch (methodName)
            {
                case "StartsWith":
                case "EndsWith":
                    var useSuffix = methodName == "StartsWith";
                    var usePrefix = methodName == "EndsWith";
                    var argValue = ((ConstantExpression)e.Arguments[0]).Evaluate().ToStringOrNull();
                    var newValue = string.Format("{0}{1}{2}", usePrefix ? "%" : "", argValue, useSuffix ? "%" : "");

                    Visit(member);
                    _nodes.AddNode(new OperatorNode(Operator.Like()));
                    Visit(Expression.Constant(newValue));
                    break;
                case "ToLower":
                    _nodes.AddNode(CreateNewMemberNode(member).ToLowerNode());
                    break;
                case "ToUpper":
                    _nodes.AddNode(CreateNewMemberNode(member).ToUpperNode());
                    break;
            }

            return e;
        }

		protected virtual Expression VisitStringQxMethodCall(MethodCallExpression e)
        {
            var member = (MemberExpression)e.Arguments[0];
            var methodName = e.Method.Name;

            switch (methodName)
            {
                case "StartsWith":
                case "EndsWith":
                case "QxLike":
                case "QxStartsWith":
                case "QxEndsWith":
                case "QxContains":
                    var useSuffix = methodName != "QxLike" && (methodName == "QxStartsWith" || methodName == "QxContains");
                    var usePrefix = methodName != "QxLike" && (methodName == "QxEndsWith" || methodName == "QxContains");
                    var argValue = ((ConstantExpression)e.Arguments[1]).Evaluate().ToStringOrNull();
                    var newValue = string.Format("{0}{1}{2}", usePrefix ? "%" : "", argValue, useSuffix ? "%" : "");
                    var constant = Expression.Constant(newValue);

                    Visit(member);
                    _nodes.AddNode(new OperatorNode(Operator.Like()));
                    Visit(constant);
                    break;
                case "QxToLower":
					_nodes.AddNode(CreateNewMemberNode(member).ToLowerNode());
                    break;
                case "QxToUpper":
					_nodes.AddNode(CreateNewMemberNode(member).ToUpperNode());
                    break;
            }

            return e;
        }

		protected virtual Expression VisitEnumerableQxMethodCall(MethodCallExpression e)
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

		protected virtual void EnsureSupportedEnumerableQxOperator(LambdaExpression e)
        {
            var isSupportedMethodCall = e.Body.NodeType == ExpressionType.Call && ((MethodCallExpression)e.Body).Method.Name == "QxAny";
            
            var operatorIsSupported = isSupportedMethodCall || SupportedEnumerableQxOperators.Contains(e.Body.NodeType);

            if(!operatorIsSupported)
                throw new SisoDbException(ExceptionMessages.WhereParser_QxEnumerables_OperatorNotSupported.Inject(e.Body.NodeType));
        }
    }
}