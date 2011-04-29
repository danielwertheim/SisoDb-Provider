namespace SisoDb.Querying.Lambdas.Processors
{
    public interface IParsedLambdaProcessor
    {
        void Process(IParsedLambda lambda);
    }

    public interface IParsedLambdaProcessor<out T>
    {
        T Process(IParsedLambda lambda);
    }
}