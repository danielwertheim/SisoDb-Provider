using System;

namespace SisoDb.Querying.Lambdas.Operators
{
	[Serializable]
    public class LessThanOrEqualOperator : Operator
    {
        public LessThanOrEqualOperator()
            : base("<=")
        {
        }

		public override Types OperatorType
		{
			get { return Types.LessThanOrEqual; }
		}
    }
}