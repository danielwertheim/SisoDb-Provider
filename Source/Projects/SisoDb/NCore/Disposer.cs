using System;

namespace NCore
{
    public static class Disposer
    {
        public static Exception TryDispose(IDisposable disposable)
        {
            if (disposable == null)
                return null;

            try
            {
                disposable.Dispose();
            }
            catch (Exception ex)
            {
                return ex;
            }

            return null;
        }
    }
}