using System;

namespace SisoDb.Querying.Lambdas.Operators
{
	[Serializable]
    public class GreaterThanOrEqualOperator : Operator
    {
        public GreaterThanOrEqualOperator()
            : base(">=")
        {
        }

		public override Types OperatorType
		{
			get { return Types.GreaterThanOrEqual; }
		}
    }
}