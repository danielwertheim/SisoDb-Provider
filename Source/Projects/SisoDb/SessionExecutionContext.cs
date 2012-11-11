using System;
using SisoDb.EnsureThat;
using SisoDb.NCore;
using SisoDb.Resources;

namespace SisoDb
{
    /// <summary>
    /// All operations within <see cref="DbSession"/> needs to be
    /// wrapped in a try, so that exceptions can be catched and
    /// the session could be marked as failed, so that automatic
    /// commits or rollbacks can be performed.
    /// </summary>
    public class SessionExecutionContext
    {
        protected readonly ISession Session;

        public SessionExecutionContext(ISession session)
        {
            Ensure.That(session, "session").IsNotNull();

            Session = session;
        }

        public virtual void Try(Action action)
        {
            if(Session.Status.IsAborted()) return;
            EnsureNotAlreadyFailed();

            try
            {
                action.Invoke();
            }
            catch
            {
                Session.MarkAsFailed();
                throw;
            }
        }

        public virtual T Try<T>(Func<T> action)
        {
            if (Session.Status.IsAborted()) return default(T);
            EnsureNotAlreadyFailed();

            try
            {
                return action.Invoke();
            }
            catch
            {
                Session.MarkAsFailed();
                throw;
            }
        }

        protected virtual void EnsureNotAlreadyFailed()
        {
            if (Session.Status.IsFailed())
                throw new SisoDbException(ExceptionMessages.Session_AlreadyFailed.Inject(Session.Id, Session.Db.Name));
        }
    }
}