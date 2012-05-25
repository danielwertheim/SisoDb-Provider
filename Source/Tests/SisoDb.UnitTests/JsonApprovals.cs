using ApprovalTests;
using SisoDb.Serialization;

namespace SisoDb.UnitTests
{
    public static class JsonApprovals
    {
        public static void VerifyAsJson<T>(T item)
        {
            VerifyJson(item.ToJson());
        }

        public static void VerifyJson(string json)
        {
#if DEBUG
            Approvals.Verify(new ApprovalTextWriter(json.Dump(), "txt"));
#endif
        }
    }
}
