using System;

namespace SisoDb.Querying.Lambdas.Operators
{
	[Serializable]
    public class IsNotOperator : Operator
    {
        public IsNotOperator()
            : base("is not")
        {
        }

		public override Types OperatorType
		{
			get { return Types.IsNot; }
		}
    }
}