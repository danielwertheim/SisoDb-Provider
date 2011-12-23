using System;

namespace SisoDb.Querying.Lambdas.Operators
{
	[Serializable]
    public class LikeOperator : Operator
    {
        public LikeOperator()
            : base("like")
        {
        }

		public override Types OperatorType
		{
			get { return Types.Like; }
		}
    }
}