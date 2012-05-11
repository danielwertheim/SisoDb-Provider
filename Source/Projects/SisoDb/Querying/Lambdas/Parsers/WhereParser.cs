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
	    protected readonly List<MemberExpression> VirtualPrefixMembers;
	    protected INodesCollection Nodes;

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

        public WhereParser()
        {
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
			var nodes =
				new BoolNodesTransformer().Transform(
				new NullableNodesTransformer().Transform(Nodes));

			return new ParsedLambda(nodes.ToArray());
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

            if (ExpressionUtils.IsNullConstant(e.Right))
                Nodes.AddNode(new OperatorNode(Operator.IsOrIsNot(e.NodeType)));
            else
                Nodes.AddNode(new OperatorNode(Operator.Create(e.NodeType)));

			if(e.Left.NodeType == ExpressionType.Convert)
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
				&& ((MemberExpression) e.Expression).Expression != null 
				&& ((MemberExpression) e.Expression).Expression.NodeType == ExpressionType.Parameter;
			
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

                if(!graphLine.Last().Equals(e))
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

            if (e.Method.DeclaringType.IsStringType())
                return VisitStringMethodCall(e);
            
            if (e.Method.DeclaringType == typeof(EnumerableQueryExtensions))
                return VisitEnumerableQxMethodCall(e);

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
            var methodName = e.Method.Name;
            
            switch (methodName)
            {
                case "StartsWith":
					var startsWithValue = e.Arguments[0].Evaluate().ToStringOrNull();
					Nodes.AddNode(CreateNewMemberNode(member).ToStartsWithNode());
                    Nodes.AddNode(new OperatorNode(Operator.Like()));
                    Visit(Expression.Constant(string.Concat(startsWithValue, "%")));
					break;
                case "EndsWith":
					var endsWithValue = e.Arguments[0].Evaluate().ToStringOrNull();
					Nodes.AddNode(CreateNewMemberNode(member).ToEndsWithNode());
                    Nodes.AddNode(new OperatorNode(Operator.Like()));
                    Visit(Expression.Constant(string.Concat("%", endsWithValue)));
                    break;
                case "ToLower":
                    Nodes.AddNode(CreateNewMemberNode(member).ToLowerNode());
                    break;
                case "ToUpper":
                    Nodes.AddNode(CreateNewMemberNode(member).ToUpperNode());
                    break;
                case "Contains":
                    Visit(member);
                    Nodes.AddNode(new OperatorNode(Operator.Like()));

                    var containsValue = e.Arguments[0].Evaluate().ToStringOrNull();
                    var constant = Expression.Constant("%{0}%".Inject(containsValue).Replace("%%", "%"));
                    Visit(constant);
                    break;
            }

            return e;
        }

		protected virtual Expression VisitStringQxMethodCall(MethodCallExpression e)
		{
			var member = e.GetRightMostMember();
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
					var argValue = e.Arguments[1].Evaluate().ToStringOrNull();
                    var newValue = string.Format("{0}{1}{2}", usePrefix ? "%" : "", argValue, useSuffix ? "%" : "");
                    var constant = Expression.Constant(newValue);

					if(methodName == "QxStartsWith")
						Nodes.AddNode(CreateNewMemberNode(member).ToStartsWithNode());
					else if(methodName == "QxEndsWith")
						Nodes.AddNode(CreateNewMemberNode(member).ToEndsWithNode());
					else
						Visit(member);

                    Nodes.AddNode(new OperatorNode(Operator.Like()));
                    Visit(constant);
                    break;
                case "QxToLower":
					Nodes.AddNode(CreateNewMemberNode(member).ToLowerNode());
                    break;
                case "QxToUpper":
					Nodes.AddNode(CreateNewMemberNode(member).ToUpperNode());
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
                    VirtualPrefixMembers.Add(member);
                    Visit(lambda);
                    VirtualPrefixMembers.RemoveAt(VirtualPrefixMembers.Count - 1);
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