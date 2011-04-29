using System.Collections.ObjectModel;
using SisoDb.Querying.Lambdas.Nodes;

namespace SisoDb.Querying.Lambdas
{
    public interface IParsedLambda
    {
        ReadOnlyCollection<INode> Nodes { get; }

        IParsedLambda MergeAsNew(IParsedLambda other);
    }
}