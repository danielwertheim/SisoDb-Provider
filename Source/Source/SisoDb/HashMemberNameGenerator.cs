using SisoDb.Cryptography;

namespace SisoDb
{
    public class HashMemberNameGenerator : IMemberNameGenerator
    {
        public const int MaxMemberNameLength = 128;
        public readonly int MemberPathSubstringLength;

        private readonly IHashService _hashService;

        public HashMemberNameGenerator(IHashService hashService)
        {
            _hashService = hashService;

            const int delimLength = 1;
            MemberPathSubstringLength = MaxMemberNameLength - (_hashService.GetHashLength() + delimLength);
        }

        public string Generate(string memberPath)
        {
            memberPath.AssertNotNullOrWhiteSpace("memberPath");

            if (memberPath == "StructureId")
                return memberPath;

            var hash = _hashService.GenerateHash(memberPath);
            var memberPathSubstring = memberPath.Length < MemberPathSubstringLength ? 
                memberPath : memberPath.Substring(0, MemberPathSubstringLength);

            return memberPathSubstring + "_" + hash;
        }
    }
}