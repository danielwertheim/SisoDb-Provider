namespace SisoDb.Tests.UnitTests
{
    internal class MemberNameGeneratorFake : IMemberNameGenerator
    {
        public string Generate(string memberPath)
        {
            return memberPath;
        }
    }
}