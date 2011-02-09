using System.Collections.ObjectModel;
using SisoDb.Lambdas.Nodes;

namespace SisoDb.Lambdas
{
    internal interface IParsedLambda
    {
        ReadOnlyCollection<INode> Nodes { get; }
    }
}