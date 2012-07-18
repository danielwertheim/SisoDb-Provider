using System;

namespace NCore
{
    public static class SysDateTime
    {
        public static Func<DateTime> NowFn;
        public static Func<DateTime> NowUtcFn;
 
        public static DateTime Now
        {
            get { return NowFn.Invoke(); }
        }

        public static DateTime NowUtc
        {
            get { return NowUtcFn.Invoke(); }
        }

        static SysDateTime()
        {
            Reset();
        }

        public static void Reset()
        {
            NowFn = () => DateTime.Now;
            NowUtcFn = () => DateTime.UtcNow;
        }
    }
}