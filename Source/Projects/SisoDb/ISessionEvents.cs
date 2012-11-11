using System;
using SisoDb.Structures;
using SisoDb.Structures.Schemas;

namespace SisoDb
{
    public interface ISessionEvents
    {
        /// <summary>
        /// Called when an item has been inserted.
        /// </summary>
        Action<IStructureSchema, IStructure, object> OnInserted { set; }
        /// <summary>
        /// Called when an item has been updated.
        /// </summary>
        Action<IStructureSchema, IStructure, object> OnUpdated { set; }
    }
}