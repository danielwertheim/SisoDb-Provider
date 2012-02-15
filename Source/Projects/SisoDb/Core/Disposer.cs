using System;
using System.Collections.Generic;
using System.Linq;

namespace SisoDb.Core
{
    public static class Disposer
    {
        public static bool TryDispose(IDisposable disposable)
        {
            if (disposable == null)
                return true;

            try
            {
                disposable.Dispose();
            }
            catch
            {
                return false;
            }

            return true;
        }

        public static bool TryDispose(IDisposable[] disposables)
        {
            foreach (var disposable in disposables.Where(d => d != null))
            {
                if (!TryDispose(disposable))
                    return false;
            }

            return true;
        }

        public static void Dispose(IDisposable[] disposables)
        {
            var exceptions = new List<Exception>();

            foreach (var disposable in disposables.Where(d => d != null))
            {
                try
                {
                    disposable.Dispose();
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }

            if (exceptions.Any())
                throw new AggregateException("Error while disposing batch of disposables. Inspect inner exceptions for details.", exceptions);
        }
    }
}