using System;

namespace SisoDb.Querying.Lambdas.Operators
{
	[Serializable]
    public class LessThanOperator : Operator
    {
        public LessThanOperator()
            : base("<")
        {
        }

		public override Types OperatorType
		{
			get { return Types.LessThan; }
		}
    }
}