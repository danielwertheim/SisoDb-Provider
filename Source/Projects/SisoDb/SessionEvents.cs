using System;
using System.Collections.Generic;
using SisoDb.EnsureThat;
using SisoDb.Structures;
using SisoDb.Structures.Schemas;

namespace SisoDb
{
    public class SessionEvents : ISessionEvents
    {
        protected readonly List<Action<IStructureSchema, IStructure, object>> OnInsertedHandlers;
        protected readonly List<Action<IStructureSchema, IStructure, object>> OnUpdatedHandlers;
        protected readonly List<Action<IStructureSchema, IStructureId>> OnDeletedHandlers;
        protected readonly List<Action<IStructureSchema, IQuery>> OnDeletedByQueryHandlers;

        public Action<IStructureSchema, IStructure, object> OnInserted 
        { 
            set 
            {
                Ensure.That(value, "OnInserted").IsNotNull();
                RegisterNewOnInsertedHandler(value);
            }
        }
        public Action<IStructureSchema, IStructure, object> OnUpdated
        {
            set
            {
                Ensure.That(value, "OnUpdated").IsNotNull();
                RegisterNewOnUpdatedHandler(value);
            }
        }

        public Action<IStructureSchema, IStructureId> OnDeleted
        {
            set
            {
                Ensure.That(value, "OnDeleted").IsNotNull();
                RegisterNewOnDeletedHandler(value);
            }
        }

        public Action<IStructureSchema, IQuery> OnDeletedByQuery
        {
            set
            {
                Ensure.That(value, "OnDeleted").IsNotNull();
                RegisterNewOnDeletedByQueryHandler(value);
            }
        }

        public SessionEvents()
        {
            OnInsertedHandlers = new List<Action<IStructureSchema, IStructure, object>>();
            OnUpdatedHandlers = new List<Action<IStructureSchema, IStructure, object>>();
            OnDeletedHandlers = new List<Action<IStructureSchema, IStructureId>>();
            OnDeletedByQueryHandlers = new List<Action<IStructureSchema, IQuery>>();
        }

        protected virtual void RegisterNewOnInsertedHandler(Action<IStructureSchema, IStructure, object> handler)
        {
            OnInsertedHandlers.Add(handler);
        }

        protected virtual void RegisterNewOnUpdatedHandler(Action<IStructureSchema, IStructure, object> handler)
        {
            OnUpdatedHandlers.Add(handler);
        }

        protected virtual void RegisterNewOnDeletedHandler(Action<IStructureSchema, IStructureId> handler)
        {
            OnDeletedHandlers.Add(handler);
        }

        protected virtual void RegisterNewOnDeletedByQueryHandler(Action<IStructureSchema, IQuery> handler)
        {
            OnDeletedByQueryHandlers.Add(handler);
        }

        public virtual void NotifyOnInserted(IStructureSchema schema, IStructure structure, object item)
        {
            foreach (var handler in OnInsertedHandlers)
                handler.Invoke(schema, structure, item);
        }

        public virtual void NotifyOnInserted(IStructureSchema schema, IStructure[] structures, object[] items)
        {
            for (var i = 0; i < structures.Length; i++)
                NotifyOnInserted(schema, structures[i], items[i]);
        }

        public virtual void NotifyOnUpdated(IStructureSchema schema, IStructure structure, object item)
        {
            foreach (var handler in OnUpdatedHandlers)
                handler.Invoke(schema, structure, item);
        }

        public virtual void NotifyOnUpdated(IStructureSchema schema, IStructure[] structures, object[] items)
        {
            for (var i = 0; i < structures.Length; i++)
                NotifyOnUpdated(schema, structures[i], items[i]);
        }

        public virtual void NotifyOnDeleted(IStructureSchema schema, IStructureId id)
        {
            foreach (var handler in OnDeletedHandlers)
                handler.Invoke(schema, id);
        }

        public virtual void NotifyOnDeleted(IStructureSchema schema, IStructureId[] ids)
        {
            foreach (var id in ids)
                NotifyOnDeleted(schema, id);
        }

        public virtual void NotifyOnDeleted(IStructureSchema schema, IQuery query)
        {
            foreach (var handler in OnDeletedByQueryHandlers)
                handler.Invoke(schema, query);
        }
    }
}