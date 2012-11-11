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

        public Action<IStructureSchema, IStructure, object> OnInserted 
        { 
            set 
            {
                Ensure.That(value, "OnInserted").IsNotNull();
                RegisterNewOnInsertedAction(value);
            }
        }
        public Action<IStructureSchema, IStructure, object> OnUpdated
        {
            set
            {
                Ensure.That(value, "OnUpdated").IsNotNull();
                RegisterNewOnUpdatedAction(value);
            }
        }

        public SessionEvents()
        {
            OnInsertedHandlers = new List<Action<IStructureSchema, IStructure, object>>();
            OnUpdatedHandlers = new List<Action<IStructureSchema, IStructure, object>>();
        }

        protected virtual void RegisterNewOnInsertedAction(Action<IStructureSchema, IStructure, object> handler)
        {
            OnInsertedHandlers.Add(handler);
        }

        protected virtual void RegisterNewOnUpdatedAction(Action<IStructureSchema, IStructure, object> handler)
        {
            OnUpdatedHandlers.Add(handler);
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
    }
}