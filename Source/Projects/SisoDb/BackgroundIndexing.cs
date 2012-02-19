using System;

namespace SisoDb
{
    [Serializable]
    public enum BackgroundIndexing
    {
        /// <summary>
        /// Default. Indexes are inserted along with structures and uniques.
        /// </summary>
        Off = 0,
        /// <summary>
        /// Indexes are queued and processed in the background. This means
        /// that structures and uniques will be inserted directly, whilst
        /// indexes will be inserted later. There will be eventual concistency
        /// between them.
        /// </summary>
        On = 1
    }
}