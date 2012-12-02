using System;
using System.Collections.Generic;
using SisoDb.EnsureThat;
using SisoDb.Structures;
using SisoDb.Structures.Schemas;

namespace SisoDb
{
    public class SessionEvents : ISessionEvents
    {
        protected readonly List<Action<ISisoDatabase, Guid>> OnCommittedHandlers;
        protected readonly List<Action<ISisoDatabase, Guid>> OnRolledbackHandlers;
        protected readonly List<Action<ISession, IStructureSchema, IStructure, object>> OnInsertedHandlers;
        protected readonly List<Action<ISession, IStructureSchema, IStructure, object>> OnUpdatedHandlers;
        protected readonly List<Action<ISession, IStructureSchema, IStructureId>> OnDeletedHandlers;
        protected readonly List<Action<ISession, IStructureSchema, IQuery>> OnDeletedByQueryHandlers;

        public Action<ISisoDatabase, Guid> OnCommitted
        {
            set
            {
                Ensure.That(value, "OnCommitted").IsNotNull();
                RegisterNewOnCommittedHandler(value);
            }
        }

        public Action<ISisoDatabase, Guid> OnRolledback
        {
            set
            {
                Ensure.That(value, "OnRolledback").IsNotNull();
                RegisterNewOnRolledbackHandler(value);
            }
        }

        public Action<ISession, IStructureSchema, IStructure, object> OnInserted 
        { 
            set 
            {
                Ensure.That(value, "OnInserted").IsNotNull();
                RegisterNewOnInsertedHandler(value);
            }
        }
        public Action<ISession, IStructureSchema, IStructure, object> OnUpdated
        {
            set
            {
                Ensure.That(value, "OnUpdated").IsNotNull();
                RegisterNewOnUpdatedHandler(value);
            }
        }

        public Action<ISession, IStructureSchema, IStructureId> OnDeleted
        {
            set
            {
                Ensure.That(value, "OnDeleted").IsNotNull();
                RegisterNewOnDeletedHandler(value);
            }
        }

        public Action<ISession, IStructureSchema, IQuery> OnDeletedByQuery
        {
            set
            {
                Ensure.That(value, "OnDeletedByQuery").IsNotNull();
                RegisterNewOnDeletedByQueryHandler(value);
            }
        }

        public SessionEvents()
        {
            OnCommittedHandlers = new List<Action<ISisoDatabase, Guid>>();
            OnRolledbackHandlers = new List<Action<ISisoDatabase, Guid>>();
            OnInsertedHandlers = new List<Action<ISession, IStructureSchema, IStructure, object>>();
            OnUpdatedHandlers = new List<Action<ISession, IStructureSchema, IStructure, object>>();
            OnDeletedHandlers = new List<Action<ISession, IStructureSchema, IStructureId>>();
            OnDeletedByQueryHandlers = new List<Action<ISession, IStructureSchema, IQuery>>();
        }

        protected virtual void RegisterNewOnCommittedHandler(Action<ISisoDatabase, Guid> handler)
        {
            OnCommittedHandlers.Add(handler);
        }

        protected virtual void RegisterNewOnRolledbackHandler(Action<ISisoDatabase, Guid> handler)
        {
            OnRolledbackHandlers.Add(handler);
        }

        protected virtual void RegisterNewOnInsertedHandler(Action<ISession, IStructureSchema, IStructure, object> handler)
        {
            OnInsertedHandlers.Add(handler);
        }

        protected virtual void RegisterNewOnUpdatedHandler(Action<ISession, IStructureSchema, IStructure, object> handler)
        {
            OnUpdatedHandlers.Add(handler);
        }

        protected virtual void RegisterNewOnDeletedHandler(Action<ISession, IStructureSchema, IStructureId> handler)
        {
            OnDeletedHandlers.Add(handler);
        }

        protected virtual void RegisterNewOnDeletedByQueryHandler(Action<ISession, IStructureSchema, IQuery> handler)
        {
            OnDeletedByQueryHandlers.Add(handler);
        }

        public virtual void NotifyCommitted(ISisoDatabase db, Guid sessionId)
        {
            foreach (var handler in OnCommittedHandlers)
                handler.Invoke(db, sessionId);
        }

        public virtual void NotifyRolledback(ISisoDatabase db, Guid sessionId)
        {
            foreach (var handler in OnRolledbackHandlers)
                handler.Invoke(db, sessionId);
        }

        public virtual void NotifyInserted(ISession session, IStructureSchema schema, IStructure structure, object item)
        {
            foreach (var handler in OnInsertedHandlers)
                handler.Invoke(session, schema, structure, item);
        }

        public virtual void NotifyInserted(ISession session, IStructureSchema schema, IStructure[] structures, object[] items)
        {
            for (var i = 0; i < structures.Length; i++)
                NotifyInserted(session, schema, structures[i], items[i]);
        }

        public virtual void NotifyUpdated(ISession session, IStructureSchema schema, IStructure structure, object item)
        {
            foreach (var handler in OnUpdatedHandlers)
                handler.Invoke(session, schema, structure, item);
        }

        public virtual void NotifyUpdated(ISession session, IStructureSchema schema, IStructure[] structures, object[] items)
        {
            for (var i = 0; i < structures.Length; i++)
                NotifyUpdated(session, schema, structures[i], items[i]);
        }

        public virtual void NotifyDeleted(ISession session, IStructureSchema schema, IStructureId id)
        {
            foreach (var handler in OnDeletedHandlers)
                handler.Invoke(session, schema, id);
        }

        public virtual void NotifyDeleted(ISession session, IStructureSchema schema, IStructureId[] ids)
        {
            foreach (var id in ids)
                NotifyDeleted(session, schema, id);
        }

        public virtual void NotifyDeleted(ISession session, IStructureSchema schema, IQuery query)
        {
            foreach (var handler in OnDeletedByQueryHandlers)
                handler.Invoke(session, schema, query);
        }
    }
}