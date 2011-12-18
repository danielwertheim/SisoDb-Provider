using System;

namespace SisoDb.Querying.Lambdas.Operators
{
	[Serializable]
    public class GreaterThanOperator : Operator
    {
        public GreaterThanOperator()
            : base(">")
        {
        }

		public override Types OperatorType
		{
			get { return Types.GreaterThan; }
		}
    }
}