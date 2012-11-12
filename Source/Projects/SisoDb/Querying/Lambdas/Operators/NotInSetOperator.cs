using System;

namespace SisoDb.Querying.Lambdas.Operators
{
    [Serializable]
    public class NotInSetOperator : Operator
    {
        public NotInSetOperator()
            : base("not in")
        {
        }

        public override Types OperatorType
        {
            get { return Types.InSet; }
        }
    }
}