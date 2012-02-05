using System;

namespace SisoDb.Dac
{
    public interface ISisoDbTransaction : IDisposable
    {
        bool Failed { get; set; }
        void MarkAsFailed();
        void Try(Action action);
        T Try<T>(Func<T> action);
    }
}