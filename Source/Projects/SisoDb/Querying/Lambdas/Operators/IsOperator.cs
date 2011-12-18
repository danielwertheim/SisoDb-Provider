using System;

namespace SisoDb.Querying.Lambdas.Operators
{
	[Serializable]
    public class IsOperator : Operator
    {
        public IsOperator()
            : base("is")
        {
        }

		public override Types OperatorType
		{
			get { return Types.Is; }
		}
    }
}