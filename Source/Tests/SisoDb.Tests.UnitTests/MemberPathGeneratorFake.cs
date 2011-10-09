namespace SisoDb.Tests.UnitTests
{
    public class MemberPathGeneratorFake : IMemberPathGenerator
    {
        public string Generate(string memberPath)
        {
            return memberPath;
        }
    }
}