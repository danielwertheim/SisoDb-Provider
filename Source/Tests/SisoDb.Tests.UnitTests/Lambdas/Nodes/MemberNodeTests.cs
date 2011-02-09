using NUnit.Framework;
using SisoDb.Lambdas.Nodes;

namespace SisoDb.Tests.UnitTests.Lambdas.Nodes
{
    [TestFixture]
    public class MemberNodeTests
    {
        [Test]
        public void CTor_WhenMemberNameIsId_TranslatedToStructureId()
        {
            var memberExpression = Type<MyClass>.GetMemberExpression(x => x.Id);

            var node = new MemberNode(null, false, memberExpression);

            Assert.AreEqual("StructureId", node.Name);
        }

        [Test]
        public void CTor_WhenMemberNameStartsWithId_MemberNameIsNotTranslated()
        {
            var memberExpression = Type<MyClass>.GetMemberExpression(x => x.IdTmp);

            var node = new MemberNode(null, false, memberExpression);

            Assert.AreEqual("IdTmp", node.Name);
        }

        [Test]
        public void CTor_WhenMemberNameEndsWithId_MemberNameIsNotTranslated()
        {
            var memberExpression = Type<MyClass>.GetMemberExpression(x => x.TmpId);

            var node = new MemberNode(null, false, memberExpression);

            Assert.AreEqual("TmpId", node.Name);
        }

        private class MyClass
        {
            public int Id { get; set; }

            public int TmpId { get; set; }

            public int IdTmp { get; set; }
        }
    }
}