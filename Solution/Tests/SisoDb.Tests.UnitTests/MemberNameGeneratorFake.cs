namespace SisoDb.Tests.UnitTests
{
    public class MemberNameGeneratorFake : IMemberNameGenerator
    {
        public string Generate(string memberPath)
        {
            return memberPath;
        }
    }
}