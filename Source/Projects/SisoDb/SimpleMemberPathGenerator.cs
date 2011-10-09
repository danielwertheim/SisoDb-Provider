using EnsureThat;

namespace SisoDb
{
    public class SimpleMemberPathGenerator : IMemberPathGenerator
    {
        public string Generate(string memberPath)
        {
            Ensure.That(memberPath, "memberPath").IsNotNullOrWhiteSpace();

            return memberPath;
        }
    }
}