using ApprovalTests;

namespace SisoDb.UnitTests
{
    public static class StringApprovals
    {
        public static void Verify(object obj)
        {
#if DEBUG
            Approvals.Verify(obj.ToString());
#endif
        }

        public static void Verify(string sql)
        {
#if DEBUG
            Approvals.Verify(sql);
#endif
        }
    }
}