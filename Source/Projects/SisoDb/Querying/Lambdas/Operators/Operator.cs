using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SisoDb.Core;
using SisoDb.Resources;

namespace SisoDb.Querying.Lambdas.Operators
{
    [Serializable]
    public class Operator
    {
        private static readonly Dictionary<ExpressionType, Func<Operator>> OperatorMap;

        private readonly string _value;

        static Operator()
        {
            OperatorMap = new Dictionary<ExpressionType, Func<Operator>>
                              {
                                  {ExpressionType.And, () => new AndOperator()},
                                  {ExpressionType.AndAlso, () => new AndOperator()},
                                  {ExpressionType.Or, () => new OrOperator()},
                                  {ExpressionType.OrElse, () => new OrOperator()},
                                  {ExpressionType.Equal, () => new EqualOperator()},
                                  {ExpressionType.Not, () => new NotOperator()},
                                  {ExpressionType.NotEqual, () => new NotEqualOperator()},
                                  {ExpressionType.LessThan, () => new LessThanOperator()},
                                  {ExpressionType.LessThanOrEqual, () => new LessThanOrEqualOperator()},
                                  {ExpressionType.GreaterThan, () => new GreaterThanOperator()},
                                  {ExpressionType.GreaterThanOrEqual, () => new GreaterThanOrEqualOperator()},
                              };
        }

        protected Operator(string value)
        {
            _value = value.Trim();
        }

        public static Operator Create(ExpressionType expressionType)
        {
            if (!OperatorMap.ContainsKey(expressionType))
                throw new NotSupportedException(
                    ExceptionMessages.LambdaOperator_OperatorTypeIsNotSupported
                    .Inject(expressionType));

            return OperatorMap[expressionType].Invoke();
        }

        public static LikeOperator Like()
        {
            return new LikeOperator();
        }

        public static Operator IsOrIsNot(ExpressionType expressionType)
        {
            if (expressionType == ExpressionType.Equal)
                return Is();

            if (expressionType == ExpressionType.NotEqual)
                return IsNot();

            throw new NotSupportedException(
                ExceptionMessages.LambdaOperator_IsOrIsNot_NotSupported.Inject(expressionType));
        }

        public static IsOperator Is()
        {
            return new IsOperator();
        }

        public static IsNotOperator IsNot()
        {
            return new IsNotOperator();
        }

        public override string ToString()
        {
            return _value;
        }
    }
}