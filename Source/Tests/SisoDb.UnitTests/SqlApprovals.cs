using ApprovalTests;

namespace SisoDb.UnitTests
{
    public static class SqlApprovals
    {
        public static void Verify(string sql)
        {
#if DEBUG
            Approvals.Verify(sql);
#endif
        }
    }
}