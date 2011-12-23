using System;

namespace SisoDb.Querying.Lambdas.Operators
{
	[Serializable]
    public class EqualOperator : Operator
    {
        public EqualOperator()
            : base("=")
        {
        }

		public override Types OperatorType
		{
			get { return Types.Equal; }
		}
    }
}