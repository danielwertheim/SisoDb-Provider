using System;

namespace SisoDb.Querying.Lambdas.Operators
{
	[Serializable]
    public class NotEqualOperator : Operator
    {
        public NotEqualOperator()
            : base("<>")
        {
        }

		public override Types OperatorType
		{
			get { return Types.NotEqual; }
		}
    }
}