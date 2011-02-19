using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using SisoDb.Lambdas.Nodes;
using SisoDb.Lambdas.Operators;
using SisoDb.Querying;
using SisoDb.Reflections;
using SisoDb.Resources;

namespace SisoDb.Lambdas
{
    public class SelectorParser : ExpressionVisitor, ISelectorParser
    {
        private readonly object _lock;
        private readonly IExpressionEvaluator _expressionEvaluator;
        private readonly Stack<MemberExpression> _virtualPrefixMembers;
        private NodesContainer _nodesContainer;

        /// <summary>
        /// Due to SisoDb is flattening object hierarchies items
        /// that are enumerable will be denormalized and store
        /// many values as string in one field. This means when
        /// using enumerables in a lambda the sub-members needs
        /// to use the complete hierarchy of the members in the
        /// lambda.
        /// </summary>
        private bool IsFlatteningMembers
        {
            get { return _virtualPrefixMembers.Count > 0; }
        }

        public SelectorParser()
        {
            _lock = new object();
            _expressionEvaluator = new LambdaExpressionEvaluator();
            _virtualPrefixMembers = new Stack<MemberExpression>();
        }

        public IParsedLambda Parse<T>(Expression<Func<T, bool>> e) where T : class 
        {
            if (e.Body.NodeType == ExpressionType.MemberAccess)
                throw new SisoDbException(ExceptionMessages.LambdaParser_NoMemberExpressions);

            lock (_lock)
            {
                _nodesContainer = new NodesContainer();

                Visit(e);

                return new ParsedLambda(_nodesContainer);
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
                    if (!e.Type.IsNullableValueType())
                        throw new NotSupportedException(ExceptionMessages.LambdaParser_VisitUnary_InvalidConvert);
                    Visit(e.Operand);
                    break;
                default:
                    throw new NotSupportedException(
                        ExceptionMessages.LambdaParser_VisitUnary_NotSupported
                        .Inject(e.NodeType));
            }

            return e;
        }

        protected override Expression VisitBinary(BinaryExpression e)
        {
            var isGroup = e.Left is BinaryExpression || e.Right is BinaryExpression;

            if (isGroup)
                _nodesContainer.AddNode(new StartGroupNode());

            if (e.Left.NodeType == ExpressionType.Parameter)
                Visit(_virtualPrefixMembers.Peek());
            else
                Visit(e.Left);

            if (Expressions.IsNullConstant(e.Right))
                _nodesContainer.AddNode(new OperatorNode(Operator.IsOrIsNot(e.NodeType)));
            else
                _nodesContainer.AddNode(!isGroup && IsFlatteningMembers
                    ? new OperatorNode(Operator.Like())
                    : new OperatorNode(Operator.Create(e.NodeType)));

            Visit(e.Right);

            if (isGroup)
                _nodesContainer.AddNode(new EndGroupNode());

            return e;
        }

        protected override Expression VisitConstant(ConstantExpression e)
        {
            if (Expressions.IsNullConstant(e))
            {
                _nodesContainer.AddNode(new NullValueNode());
                return e;
            }

            var value = e.Value;

            if (IsFlatteningMembers)
            {
                if (!(value is string))
                    value = SisoDbEnvironment.Formatting.StringConverter.AsString(value);

                value = "%<${0}$>%".Inject(value);
            }

            _nodesContainer.AddNode(new ValueNode(value));

            return e;
        }

        protected override Expression VisitMember(MemberExpression e)
        {
            ConstantExpression constantExpression = null;

            try
            {
                var value = _expressionEvaluator.Evaluate(e);
                constantExpression = Expression.Constant(value);
            }
            catch
            {
            }

            if(constantExpression != null)
                return Visit(constantExpression);

            var memberNode = CreateNewMemberNode(e);
            _nodesContainer.AddNode(memberNode);

            return e;
        }

        private MemberNode CreateNewMemberNode(MemberExpression e)
        {
            var graphLine = new List<MemberExpression>(e.ExtractGraphLineFirstToLast());
            MemberNode previous = null;

            if (IsFlatteningMembers)
                graphLine.InsertRange(0, _virtualPrefixMembers.Reverse().Where(vir => !graphLine.Any(gl => gl.Equals(vir))));

            foreach (var memberExpression in graphLine)
            {
                var node = new MemberNode(previous, IsFlatteningMembers, memberExpression);
                previous = node;
            }

            if(previous.Expression.Type.IsEnumerableBytesType())
                throw new NotSupportedException(ExceptionMessages.LambdaParser_MemberIsBytes.Inject(previous.Path));

            return previous;
        }

        protected override Expression VisitMethodCall(MethodCallExpression e)
        {
            if (e.Method.DeclaringType == typeof(StringQueryExtensions))
                return VisitStringQueryExtensionMethodCall(e);
            
            if (e.Method.DeclaringType == typeof(EnumerableQueryExtensions))
                return VisitEnumerableQueryExtensionMethodCall(e);

            try
            {
                var value = _expressionEvaluator.Evaluate(e);
                var constant = Expression.Constant(value);
                Visit(constant);
            }
            catch (Exception ex)
            {
                throw new NotSupportedException(
                    ExceptionMessages.LambdaParser_UnsupportedMethodCall
                    .Inject(e.ToString()), ex);
            }

            return e;
        }

        private Expression VisitStringQueryExtensionMethodCall(MethodCallExpression e)
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

        private Expression VisitEnumerableQueryExtensionMethodCall(MethodCallExpression e)
        {
            var member = (MemberExpression)e.Arguments[0];
            var methodName = e.Method.Name;

            switch (methodName)
            {
                case "QxAny":
                    _virtualPrefixMembers.Push(member);
                    Visit(e.Arguments[1]);
                    _virtualPrefixMembers.Pop();
                    break;
            }

            return e;
        }
    }
}