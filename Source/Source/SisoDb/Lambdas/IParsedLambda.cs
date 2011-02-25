using System.Collections.ObjectModel;
using SisoDb.Lambdas.Nodes;

namespace SisoDb.Lambdas
{
    public interface IParsedLambda
    {
        ReadOnlyCollection<INode> Nodes { get; }

        IParsedLambda MergeAsNew(IParsedLambda other);
    }
}