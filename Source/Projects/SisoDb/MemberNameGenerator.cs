using EnsureThat;

namespace SisoDb
{
    public class MemberNameGenerator : IMemberNameGenerator
    {
        public string Generate(string memberPath)
        {
            Ensure.That(memberPath, "memberPath").IsNotNullOrWhiteSpace();

            return memberPath;
        }
    }
}