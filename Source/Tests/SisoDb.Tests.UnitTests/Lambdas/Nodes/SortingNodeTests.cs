using NUnit.Framework;
using SisoDb.Lambdas.Nodes;

namespace SisoDb.Tests.UnitTests.Lambdas.Nodes
{
    [TestFixture]
    public class SortingNodeTests
    {
        [Test]
        public void CTor_WhenMemberNameIsId_TranslatedToStructureId()
        {
            var node = new SortingNode("Id");

            Assert.AreEqual("StructureId", node.Name);
        }

        [Test]
        public void CTor_WhenMemberNameStartsWithId_MemberNameIsNotTranslated()
        {
            var node = new SortingNode("IdTmp");

            Assert.AreEqual("IdTmp", node.Name);
        }

        [Test]
        public void CTor_WhenMemberNameEndsWithId_MemberNameIsNotTranslated()
        {
            var node = new SortingNode("TmpId");

            Assert.AreEqual("TmpId", node.Name);
        }
    }
}