using System;

namespace SisoDb.Dynamic
{
    public static class SisoDynamicRuntime
    {
        private static readonly IDynamicLambdaBuilder LambdaBuilder;

        public static readonly Func<IDynamicLambdaBuilder> LambdaBuilderResolver;

        static SisoDynamicRuntime()
        {
            LambdaBuilder = new DynamicLambdaBuilder();
            LambdaBuilderResolver = () => LambdaBuilder;
        }
    }
}