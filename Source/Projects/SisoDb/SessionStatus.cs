using System;

namespace SisoDb
{
    [Serializable]
    public enum SessionStatus
    {
        /// <summary>
        /// Session is active and healthy.
        /// </summary>
        Active,
        /// <summary>
        /// Session has been aborted but has not yet been disposed.
        /// </summary>
        Aborted,
        /// <summary>
        /// Session has failed but has not yet been disposed.
        /// </summary>
        Failed,
        /// <summary>
        /// Session without failures has been disposed.
        /// </summary>
        Disposed,
        /// <summary>
        /// Aborted session has been disposed.
        /// </summary>
        DisposedAfterAbort,
        /// <summary>
        /// Failed session has been disposed.
        /// </summary>
        DisposedWithFailure
    }

    public static class SessionStatusExtensions
    {
        public static bool IsAborted(this SessionStatus status)
        {
            return status == SessionStatus.Aborted || status == SessionStatus.DisposedAfterAbort;
        }

        public static bool IsFailed(this SessionStatus status)
        {
            return status == SessionStatus.Failed || status == SessionStatus.DisposedWithFailure;
        }

        public static bool IsDisposed(this SessionStatus status)
        {
            return status == SessionStatus.Disposed || status == SessionStatus.DisposedAfterAbort ||  status == SessionStatus.DisposedWithFailure;
        }
    }
}