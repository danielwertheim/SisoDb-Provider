using System;
using SisoDb.Cryptography;

namespace SisoDb
{
    internal class HashMemberNameGenerator : IMemberNameGenerator
    {
        internal const int MaxMemberNameLength = 128;
        internal readonly int MemberPathSubstringLength;

        private readonly IHashService _hashService;

        public HashMemberNameGenerator(IHashService hashService)
        {
            _hashService = hashService;

            const int delimLength = 1;
            MemberPathSubstringLength = MaxMemberNameLength - (_hashService.GetHashLength() + delimLength);
        }

        public string Generate(string memberPath)
        {
            if(memberPath.IsNullOrWhiteSpace())
                throw  new ArgumentNullException("memberPath");

            var hash = _hashService.GenerateHash(memberPath);
            var memberPathSubstring = memberPath.Length < MemberPathSubstringLength ? 
                memberPath : memberPath.Substring(0, MemberPathSubstringLength);

            return memberPathSubstring + "_" + hash;
        }
    }
}