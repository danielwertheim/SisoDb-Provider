using PineCone.Structures.Schemas;

namespace SisoDb.Querying.Lambdas.Converters
{
    public interface IParsedLambdaConverter<out T>
    {
        T Convert(IStructureSchema structureSchema, IParsedLambda lambda);
    }
}