using System;
using SisoDb.EnsureThat;
using SisoDb.NCore;
using SisoDb.Resources;

namespace SisoDb
{
    public class SessionExecutionContext : ISessionExecutionContext
    {
        public ISession Session { get; private set; }

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