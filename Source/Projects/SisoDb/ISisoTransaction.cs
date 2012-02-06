using System;

namespace SisoDb
{
    public interface ISisoTransaction : IDisposable
    {
        bool Failed { get; set; }
        void MarkAsFailed();
        void Try(Action action);
        T Try<T>(Func<T> action);
    }
}