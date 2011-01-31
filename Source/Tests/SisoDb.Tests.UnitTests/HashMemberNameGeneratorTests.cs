using System;
using NUnit.Framework;
using SisoDb.Cryptography;
using SisoDb.TestUtils.TypeMockExtensions;
using TypeMock.ArrangeActAssert;

namespace SisoDb.Tests.UnitTests
{
    [TestFixture]
    public class HashMemberNameGeneratorTests
    {
        private const string FakedHash = "Hash";

        [Test]
        public void MaxMemberNameLength_DoesNotExceedSqlServersMaxOf128()
        {
            Assert.AreEqual(128, HashMemberNameGenerator.MaxMemberNameLength);
        }

        [Test, Isolated]
        public void MemberPathSubstringLength_WhenHashLengthIs8_CalculatedTo119_SinceDelimTakesOne()
        {
            var hashService = Isolate.Fake.Instance<IHashService>();
            Isolate.WhenCalled(() => hashService.GetHashLength()).WillReturn(8);

            var generator = new HashMemberNameGenerator(hashService);

            Assert.AreEqual(119, generator.MemberPathSubstringLength);
        }

        [Test, Isolated]
        public void Generate_WhenCalledWithNull_ThrowsArgumentNullException()
        {
            var fakedHashService = GetFakedHashService();
            var generator = new HashMemberNameGenerator(fakedHashService);

            Assert.Throws<ArgumentNullException>(() => generator.Generate(null));
        }

        [Test, Isolated]
        public void Generate_WhenCalledWithEmptyString_ThrowsArgumentNullException()
        {
            var fakedHashService = GetFakedHashService();
            var generator = new HashMemberNameGenerator(fakedHashService);

            Assert.Throws<ArgumentNullException>(() => generator.Generate(string.Empty));
        }

        [Test, Isolated]
        public void Generate_WhenPassedValueLengthIsLessThanMemberPathSubstringLength_CompleteValuePlusHashIsReturned()
        {
            var fakedHashService = GetFakedHashService();
            var generator = new HashMemberNameGenerator(fakedHashService);

            var inputValue = new string('a', generator.MemberPathSubstringLength - 1);
            var memberName = generator.Generate(inputValue);

            Assert.AreEqual(inputValue + "_" + FakedHash, memberName);
        }

        [Test, Isolated]
        public void Generate_WhenPassedValueLengthIsEqualToMemberPathSubstringLength_CompleteValuePlusHashIsReturned()
        {
            var fakedHashService = GetFakedHashService();
            var generator = new HashMemberNameGenerator(fakedHashService);

            var inputValue = new string('a', generator.MemberPathSubstringLength);
            var memberName = generator.Generate(inputValue);

            Assert.AreEqual(inputValue + "_" + FakedHash, memberName);
        }

        [Test, Isolated]
        public void Generate_WhenPassedValueLengthIsGreaterThanMemberPathSubstringLength_CompleteValuePlusHashIsReturned()
        {
            var fakedHashService = GetFakedHashService();
            var generator = new HashMemberNameGenerator(fakedHashService);

            var inputValue = new string('a', generator.MemberPathSubstringLength + 1);
            var memberName = generator.Generate(inputValue);

            var expectedPrefixFromInputValue = inputValue.Substring(0, inputValue.Length - 1);
            Assert.AreEqual(expectedPrefixFromInputValue + "_" + FakedHash, memberName);
        }

        private static IHashService GetFakedHashService()
        {
            var hashService = Isolate.Fake.Instance<IHashService>();
            Isolate.WhenCalled(() => hashService.GenerateHash(IsAny<string>.Item)).WillReturn(FakedHash);

            return hashService;
        }
    }
}