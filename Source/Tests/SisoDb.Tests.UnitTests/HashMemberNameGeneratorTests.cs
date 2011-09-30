using System;
using Moq;
using NUnit.Framework;
using SisoDb.Cryptography;

namespace SisoDb.Tests.UnitTests
{
    [TestFixture]
    public class HashMemberNameGeneratorTests : UnitTestBase
    {
        private const string FakedHash = "Hash";

        [Test]
        public void Generate_WhenPassedValueIsSisoId_NoHashIsAppended()
        {
            var fakedHashService = GetFakedHashService();
            var generator = new HashMemberNameGenerator(fakedHashService);

            var inputValue = "SisoId";
            var memberName = generator.Generate(inputValue);

            Assert.AreEqual(inputValue, memberName);
        }

        [Test]
        public void MaxMemberNameLength_DoesNotExceedSqlServersMaxOf128()
        {
            Assert.AreEqual(128, HashMemberNameGenerator.MaxMemberNameLength);
        }

        [Test]
        public void MemberPathSubstringLength_WhenHashLengthIs8_CalculatedTo119_SinceDelimTakesOne()
        {
            var fakedHashService = new Mock<IHashService>();
            fakedHashService.Setup(x => x.GetHashLength()).Returns(8);

            var generator = new HashMemberNameGenerator(fakedHashService.Object);

            Assert.AreEqual(119, generator.MemberPathSubstringLength);
        }

        [Test]
        public void Generate_WhenCalledWithNull_ThrowsArgumentNullException()
        {
            var fakedHashService = GetFakedHashService();
            var generator = new HashMemberNameGenerator(fakedHashService);

            Assert.Throws<ArgumentNullException>(() => generator.Generate(null));
        }

        [Test]
        public void Generate_WhenCalledWithEmptyString_ThrowsArgumentNullException()
        {
            var fakedHashService = GetFakedHashService();
            var generator = new HashMemberNameGenerator(fakedHashService);

            Assert.Throws<ArgumentNullException>(() => generator.Generate(string.Empty));
        }

        [Test]
        public void Generate_WhenPassedValueLengthIsLessThanMemberPathSubstringLength_CompleteValuePlusHashIsReturned()
        {
            var fakedHashService = GetFakedHashService();
            var generator = new HashMemberNameGenerator(fakedHashService);

            var inputValue = new string('a', generator.MemberPathSubstringLength - 1);
            var memberName = generator.Generate(inputValue);

            Assert.AreEqual(inputValue + "_" + FakedHash, memberName);
        }

        [Test]
        public void Generate_WhenPassedValueLengthIsEqualToMemberPathSubstringLength_CompleteValuePlusHashIsReturned()
        {
            var fakedHashService = GetFakedHashService();
            var generator = new HashMemberNameGenerator(fakedHashService);

            var inputValue = new string('a', generator.MemberPathSubstringLength);
            var memberName = generator.Generate(inputValue);

            Assert.AreEqual(inputValue + "_" + FakedHash, memberName);
        }

        [Test]
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
            var fakedHashService = new Mock<IHashService>();
            fakedHashService.Setup(x => x.GenerateHash(It.IsAny<string>())).Returns(FakedHash);

            return fakedHashService.Object;
        }
    }
}