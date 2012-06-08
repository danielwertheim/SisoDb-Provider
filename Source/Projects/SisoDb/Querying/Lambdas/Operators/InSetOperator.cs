using System;

namespace SisoDb.Querying.Lambdas.Operators
{
    [Serializable]
    public class InSetOperator : Operator
    {
        public InSetOperator()
            : base("in")
        {
        }

        public override Types OperatorType
        {
            get { return Types.InSet; }
        }
    }
}