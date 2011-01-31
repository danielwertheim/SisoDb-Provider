using System.Collections.Generic;
using System.Linq.Expressions;
using SisoDb.Lambdas.Nodes;

namespace SisoDb.Lambdas
{
    internal interface IParsedLambda
    {
        LambdaExpression Expression { get; }
        IList<INode> Nodes { get; }
    }
}