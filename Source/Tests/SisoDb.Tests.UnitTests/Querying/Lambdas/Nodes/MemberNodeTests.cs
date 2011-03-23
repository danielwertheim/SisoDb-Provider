using NUnit.Framework;
using SisoDb.Querying.Lambdas.Nodes;
using SisoDb.Reflections;

namespace SisoDb.Tests.UnitTests.Querying.Lambdas.Nodes
{
    [TestFixture]
    public class MemberNodeTests
    {
        [Test]
        public void CTor_WhenMemberNameIsId_MemberNameIsNotTranslated()
        {
            var memberExpression = Reflect<MyClass>.MemberFrom(x => x.Id);

            var node = new MemberNode(null, false, memberExpression);

            Assert.AreEqual("Id", node.Name);
        }

        [Test]
        public void CTor_WhenMemberNameIsId_MemberPathIsNotTranslated()
        {
            var memberExpression = Reflect<MyClass>.MemberFrom(x => x.Id);

            var node = new MemberNode(null, false, memberExpression);

            Assert.AreEqual("Id", node.Path);
        }

        [Test]
        public void CTor_WhenNestedMemberNameIsId_MemberNameIsNotTranslated()
        {
            var parentExpression = Reflect<MyClass>.MemberFrom(x => x.Child);
            var memberExpression = Reflect<MyClass>.MemberFrom(x => x.Child.Id);

            var parentNode = new MemberNode(null, false, parentExpression);
            var node = new MemberNode(parentNode, false, memberExpression);

            Assert.AreEqual("Id", node.Name);
        }

        [Test]
        public void CTor_WhenNestedMemberNameIsId_MemberPathIsNotTranslated()
        {
            var parentExpression = Reflect<MyClass>.MemberFrom(x => x.Child);
            var memberExpression = Reflect<MyClass>.MemberFrom(x => x.Child.Id);

            var parentNode = new MemberNode(null, false, parentExpression);
            var node = new MemberNode(parentNode, false, memberExpression);

            Assert.AreEqual("Child.Id", node.Path);
        }

        [Test]
        public void CTor_WhenMemberNameStartsWithId_MemberNameIsNotTranslated()
        {
            var memberExpression = Reflect<MyClass>.MemberFrom(x => x.IdTmp);

            var node = new MemberNode(null, false, memberExpression);

            Assert.AreEqual("IdTmp", node.Name);
        }

        [Test]
        public void CTor_WhenNestedMemberNameStartsWithId_MemberNameIsNotTranslated()
        {
            var parentExpression = Reflect<MyClass>.MemberFrom(x => x.Child);
            var memberExpression = Reflect<MyClass>.MemberFrom(x => x.Child.IdTmp);

            var parentNode = new MemberNode(null, false, parentExpression);
            var node = new MemberNode(parentNode, false, memberExpression);

            Assert.AreEqual("IdTmp", node.Name);
        }

        [Test]
        public void CTor_WhenNestedMemberNameStartsWithId_MemberPathIsNotTranslated()
        {
            var parentExpression = Reflect<MyClass>.MemberFrom(x => x.Child);
            var memberExpression = Reflect<MyClass>.MemberFrom(x => x.Child.IdTmp);

            var parentNode = new MemberNode(null, false, parentExpression);
            var node = new MemberNode(parentNode, false, memberExpression);

            Assert.AreEqual("Child.IdTmp", node.Path);
        }

        [Test]
        public void CTor_WhenMemberNameEndsWithId_MemberNameIsNotTranslated()
        {
            var memberExpression = Reflect<MyClass>.MemberFrom(x => x.TmpId);

            var node = new MemberNode(null, false, memberExpression);

            Assert.AreEqual("TmpId", node.Name);
        }

        [Test]
        public void CTor_WhenNestedMemberNameEndsWithId_MemberNameIsNotTranslated()
        {
            var parentExpression = Reflect<MyClass>.MemberFrom(x => x.Child);
            var memberExpression = Reflect<MyClass>.MemberFrom(x => x.Child.TmpId);

            var parentNode = new MemberNode(null, false, parentExpression);
            var node = new MemberNode(parentNode, false, memberExpression);

            Assert.AreEqual("TmpId", node.Name);
        }

        [Test]
        public void CTor_WhenNestedMemberNameEndsWithId_MemberPathIsNotTranslated()
        {
            var parentExpression = Reflect<MyClass>.MemberFrom(x => x.Child);
            var memberExpression = Reflect<MyClass>.MemberFrom(x => x.Child.TmpId);

            var parentNode = new MemberNode(null, false, parentExpression);
            var node = new MemberNode(parentNode, false, memberExpression);

            Assert.AreEqual("Child.TmpId", node.Path);
        }

        private class MyClass
        {
            public int Id { get; set; }

            public int TmpId { get; set; }

            public int IdTmp { get; set; }

            public MyChildClass Child { get; set; }
        }

        private class MyChildClass
        {
            public int Id { get; set; }

            public int TmpId { get; set; }

            public int IdTmp { get; set; }
        }
    }
}