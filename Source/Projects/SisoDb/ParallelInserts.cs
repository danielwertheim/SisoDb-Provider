using System;

namespace SisoDb
{
    [Serializable]
    public enum ParallelInserts
    {
        /// <summary>
        /// Default. Within the same insert operation, NO parallel inserts are made.
        /// </summary>
        Off = 0,
        /// <summary>
        /// Within the same insert operation multiple threads and connections are
        /// used to insert in parallel.
        /// </summary>
        /// <remarks>Requires DTC Service to be started.</remarks>
        On = 1
    }
}