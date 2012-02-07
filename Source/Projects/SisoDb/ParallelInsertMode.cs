using System;

namespace SisoDb
{
    [Serializable]
    public enum ParallelInsertMode
    {
        /// <summary>
        /// Default. Within the same insert operation, NO parallel inserts are made.
        /// </summary>
        None = 0,
        /// <summary>
        /// Within the same insert operation, Structures and Indexes are inserted in parallel (two threads).
        /// </summary>
        Simple = 1,
        /// <summary>
        /// Within the same insert operation, Structures and Indexes are inserted in parallel and indexes gets one connection per type of Index. Integers, fractals....
        /// </summary>
        Full = 2
    }
}