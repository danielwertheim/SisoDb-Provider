using System;

namespace SisoDb.Querying.Lambdas.Operators
{
	[Serializable]
    public class AndOperator : Operator
    {
        public AndOperator()
            : base("and")
        {
        }

    	public override Types OperatorType
    	{
			get { return Types.And; }
    	}
    }
}