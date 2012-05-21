using NUnit.Framework;
using SisoDb.Querying.Lambdas.Nodes;

namespace SisoDb.UnitTests.Querying.Lambdas.Nodes
{
    [TestFixture]
    public class SortingNodeTests : UnitTestBase
    {
        [Test]
        public void Ctor_WhenTypeIsSpecified_MemberTypeIsAssigned()
        {
            var node1 = new SortingNode("GOOFY", typeof (int));
            var node2 = new SortingNode("GOOFY", typeof(string));

            Assert.AreEqual(typeof(int), node1.DataType);
            Assert.AreEqual(typeof(string), node2.DataType);
        }

        [Test]
        public void Ctor_WhenMemberPathIsStructureId_MemberNameIsTranslated()
        {
            var node = new SortingNode("StructureId", typeof(int));

            Assert.AreEqual("StructureId", node.MemberPath);
        }

        [Test]
        public void Ctor_WhenMemberPathIsId_MemberNameIsTranslated()
        {
            var node = new SortingNode("Id", typeof(int));

            Assert.AreEqual("Id", node.MemberPath);
        }

        [Test]
        public void Ctor_WhenMemberPathStartsWithId_MemberNameIsTranslated()
        {
            var node = new SortingNode("IdTmp", typeof(int));

            Assert.AreEqual("IdTmp", node.MemberPath);
        }

        [Test]
        public void Ctor_WhenMemberPathEndsWithId_MemberNameIsTranslated()
        {
            var node = new SortingNode("TmpId", typeof(int));

            Assert.AreEqual("TmpId", node.MemberPath);
        }

        [Test]
        public void Ctor_WhenNestedMemberPathIsId_MemberNameIsTranslated()
        {
            var node = new SortingNode("Nested.Id", typeof(int));

            Assert.AreEqual("Nested.Id", node.MemberPath);
        }

        [Test]
        public void CTor_WhenNestedMemberPathStartsWithId_MemberNameIsTranslated()
        {
            var node = new SortingNode("Nested.IdTmp", typeof(int));

            Assert.AreEqual("Nested.IdTmp", node.MemberPath);
        }

        [Test]
        public void CTor_WhenNestedMemberPathEndsWithId_MemberNameIsTranslated()
        {
            var node = new SortingNode("Nested.TmpId", typeof(int));

            Assert.AreEqual("Nested.TmpId", node.MemberPath);
        }
    }
}