using PineCone.Structures.Schemas;

namespace SisoDb.Querying.Lambdas.Processors
{
    public interface IParsedLambdaConverter<out T>
    {
        T Convert(IStructureSchema structureSchema, IParsedLambda lambda);
    }
}