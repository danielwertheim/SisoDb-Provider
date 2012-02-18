using System;

namespace SisoDb
{
    public interface ISisoDbTransaction : IDisposable
    {
        bool Failed { get; }
        void MarkAsFailed();
    }
}