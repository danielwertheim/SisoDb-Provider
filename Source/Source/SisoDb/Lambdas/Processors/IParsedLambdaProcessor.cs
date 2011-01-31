namespace SisoDb.Lambdas.Processors
{
    internal interface IParsedLambdaProcessor
    {
        void Process(IParsedLambda lambda);
    }

    internal interface IParsedLambdaProcessor<out T>
    {
        T Process(IParsedLambda lambda);
    }
}