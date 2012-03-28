using SisoDb.Querying.Lambdas.Nodes;

namespace SisoDb.Querying.Lambdas
{
    public interface IParsedLambda
    {
        INode[] Nodes { get; }

        IParsedLambda MergeAsNew(IParsedLambda other);

        string AsString();
    }
}