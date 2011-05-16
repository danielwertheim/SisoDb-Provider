using NUnit.Framework;
using SisoDb.Querying.Lambdas.Nodes;
using SisoDb.Reflections;

namespace SisoDb.Tests.UnitTests.Querying.Lambdas.Nodes
{
    [TestFixture]
    public class MemberNodeTests : UnitTestBase
    {
        [Test]
        public void CTor_WhenMemberNameIsSisoId_MemberNameIsTranslated()
        {
            var memberExpression = Reflect<MyClass>.MemberFrom(x => x.SisoId);

            var node = new MemberNode(null, memberExpression);

            Assert.AreEqual("SisoId", node.Name);
        }

        [Test]
        public void CTor_WhenMemberNameIsSisoId_MemberPathIsTranslated()
        {
            var memberExpression = Reflect<MyClass>.MemberFrom(x => x.SisoId);

            var node = new MemberNode(null, memberExpression);

            Assert.AreEqual("SisoId", node.Path);
        }

        [Test]
        public void CTor_WhenNestedMemberNameIsSisoId_MemberNameIsTranslated()
        {
            var parentExpression = Reflect<MyClass>.MemberFrom(x => x.Child);
            var memberExpression = Reflect<MyClass>.MemberFrom(x => x.Child.SisoId);

            var parentNode = new MemberNode(null, parentExpression);
            var node = new MemberNode(parentNode, memberExpression);

            Assert.AreEqual("SisoId", node.Name);
        }

        [Test]
        public void CTor_WhenNestedMemberNameIsSisoId_MemberPathIsTranslated()
        {
            var memberExpression = Reflect<MyClass>.MemberFrom(x => x.Child.SisoId);

            var node = new MemberNode(null, memberExpression);

            Assert.AreEqual("Child.SisoId", node.Path);
        }

        [Test]
        public void CTor_WhenMemberNameStartsWithId_MemberNameIsTranslated()
        {
            var memberExpression = Reflect<MyClass>.MemberFrom(x => x.IdTmp);

            var node = new MemberNode(null, memberExpression);

            Assert.AreEqual("IdTmp", node.Name);
        }

        [Test]
        public void CTor_WhenNestedMemberNameStartsWithId_MemberNameIsTranslated()
        {
            var parentExpression = Reflect<MyClass>.MemberFrom(x => x.Child);
            var memberExpression = Reflect<MyClass>.MemberFrom(x => x.Child.IdTmp);

            var parentNode = new MemberNode(null, parentExpression);
            var node = new MemberNode(parentNode, memberExpression);

            Assert.AreEqual("IdTmp", node.Name);
        }

        [Test]
        public void CTor_WhenNestedMemberNameStartsWithId_MemberPathIsTranslated()
        {
            var memberExpression = Reflect<MyClass>.MemberFrom(x => x.Child.IdTmp);

            var node = new MemberNode(null, memberExpression);

            Assert.AreEqual("Child.IdTmp", node.Path);
        }

        [Test]
        public void CTor_WhenMemberNameEndsWithId_MemberNameIsTranslated()
        {
            var memberExpression = Reflect<MyClass>.MemberFrom(x => x.TmpId);

            var node = new MemberNode(null, memberExpression);

            Assert.AreEqual("TmpId", node.Name);
        }

        [Test]
        public void CTor_WhenNestedMemberNameEndsWithId_MemberNameIsTranslated()
        {
            var parentExpression = Reflect<MyClass>.MemberFrom(x => x.Child);
            var memberExpression = Reflect<MyClass>.MemberFrom(x => x.Child.TmpId);

            var parentNode = new MemberNode(null, parentExpression);
            var node = new MemberNode(parentNode, memberExpression);

            Assert.AreEqual("TmpId", node.Name);
        }

        [Test]
        public void CTor_WhenNestedMemberNameEndsWithId_MemberPathIsTranslated()
        {
            var memberExpression = Reflect<MyClass>.MemberFrom(x => x.Child.TmpId);

            var node = new MemberNode(null, memberExpression);

            Assert.AreEqual("Child.TmpId", node.Path);
        }

        private class MyClass
        {
            public int SisoId { get; set; }

            public int TmpId { get; set; }

            public int IdTmp { get; set; }

            public MyChildClass Child { get; set; }
        }

        private class MyChildClass
        {
            public int SisoId { get; set; }

            public int TmpId { get; set; }

            public int IdTmp { get; set; }
        }
    }
}