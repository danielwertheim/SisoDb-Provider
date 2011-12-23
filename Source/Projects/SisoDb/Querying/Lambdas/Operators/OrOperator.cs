using System;

namespace SisoDb.Querying.Lambdas.Operators
{
	[Serializable]
    public class OrOperator : Operator
    {
        public OrOperator()
            : base("or")
        {
        }

		public override Types OperatorType
		{
			get { return Types.Or; }
		}
    }
}