using ApprovalTests;
using Newtonsoft.Json;
using SisoDb.Serialization;

namespace SisoDb.UnitTests
{
    public static class JsonApprovals
    {
        public static void VerifyAsJson<T>(T item) where T : class 
        {
            VerifyJson(JsonConvert.SerializeObject(item));
        }

        public static void VerifyJson(string json)
        {
#if DEBUG
            Approvals.Verify(new ApprovalTextWriter(json.Dump(), "txt"));
#endif
        }
    }
}
