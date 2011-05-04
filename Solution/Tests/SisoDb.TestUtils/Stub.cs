using System;
using Moq;

namespace SisoDb.TestUtils
{
    public static class Stub
    {
        public static T This<T>(params Action<Mock<T>>[] setups) where T : class
        {
            var stub = new Mock<T>();

            foreach (var setup in setups)
                setup(stub);

            return stub.Object;
        }
    }
}