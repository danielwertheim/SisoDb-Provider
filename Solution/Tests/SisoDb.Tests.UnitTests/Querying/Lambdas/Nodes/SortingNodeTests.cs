using NUnit.Framework;
using SisoDb.Querying.Lambdas.Nodes;

namespace SisoDb.Tests.UnitTests.Querying.Lambdas.Nodes
{
    [TestFixture]
    public class SortingNodeTests : UnitTestBase
    {
        [Test]
        public void CTor_WhenMemberPathIsSisoId_MemberNameIsTranslated()
        {
            var node = new SortingNode("SisoId");

            Assert.AreEqual("SisoId", node.MemberPath);
        }

        [Test]
        public void CTor_WhenMemberPathIsId_MemberNameIsTranslated()
        {
            var node = new SortingNode("Id");

            Assert.AreEqual("Id", node.MemberPath);
        }

        [Test]
        public void CTor_WhenMemberPathStartsWithId_MemberNameIsTranslated()
        {
            var node = new SortingNode("IdTmp");

            Assert.AreEqual("IdTmp", node.MemberPath);
        }

        [Test]
        public void CTor_WhenMemberPathEndsWithId_MemberNameIsTranslated()
        {
            var node = new SortingNode("TmpId");

            Assert.AreEqual("TmpId", node.MemberPath);
        }

        [Test]
        public void CTor_WhenNestedMemberPathIsId_MemberNameIsTranslated()
        {
            var node = new SortingNode("Nested.Id");

            Assert.AreEqual("Nested.Id", node.MemberPath);
        }

        [Test]
        public void CTor_WhenNestedMemberPathStartsWithId_MemberNameIsTranslated()
        {
            var node = new SortingNode("Nested.IdTmp");

            Assert.AreEqual("Nested.IdTmp", node.MemberPath);
        }

        [Test]
        public void CTor_WhenNestedMemberPathEndsWithId_MemberNameIsTranslated()
        {
            var node = new SortingNode("Nested.TmpId");

            Assert.AreEqual("Nested.TmpId", node.MemberPath);
        }
    }
}