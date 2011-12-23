using System;

namespace SisoDb.Querying.Lambdas.Operators
{
	[Serializable]
    public class NotOperator : Operator
    {
        public NotOperator() : base("not")
        {
        }

		public override Types OperatorType
		{
			get { return Types.Not; }
		}
    }
}