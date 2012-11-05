using System;
using NUnit.Framework;
using SisoDb.Querying.Lambdas.Nodes;
using SisoDb.Structures.Schemas;

namespace SisoDb.UnitTests.Querying.Lambdas.Nodes
{
    [TestFixture]
    public class SortingNodeTests : UnitTestBase
    {
        private readonly IDataTypeConverter _dataTypeConverter = new DataTypeConverter();

        private SortingNode CreateSortingNode(string memberPath, Type type)
        {
            return new SortingNode(memberPath, type, _dataTypeConverter.Convert(type, memberPath));
        }

        [Test]
        public void Ctor_WhenTypeIsSpecified_MemberTypeIsAssigned()
        {
            var node1 = CreateSortingNode("GOOFY", typeof (int));
            var node2 = CreateSortingNode("GOOFY", typeof(string));

            Assert.AreEqual(typeof(int), node1.DataType);
            Assert.AreEqual(typeof(string), node2.DataType);
        }

        [Test]
        public void Ctor_WhenMemberPathIsStructureId_MemberNameIsTranslated()
        {
            var node = CreateSortingNode("StructureId", typeof(int));

            Assert.AreEqual("StructureId", node.MemberPath);
        }

        [Test]
        public void Ctor_WhenMemberPathIsId_MemberNameIsTranslated()
        {
            var node = CreateSortingNode("Id", typeof(int));

            Assert.AreEqual("Id", node.MemberPath);
        }

        [Test]
        public void Ctor_WhenMemberPathStartsWithId_MemberNameIsTranslated()
        {
            var node = CreateSortingNode("IdTmp", typeof(int));

            Assert.AreEqual("IdTmp", node.MemberPath);
        }

        [Test]
        public void Ctor_WhenMemberPathEndsWithId_MemberNameIsTranslated()
        {
            var node = CreateSortingNode("TmpId", typeof(int));

            Assert.AreEqual("TmpId", node.MemberPath);
        }

        [Test]
        public void Ctor_WhenNestedMemberPathIsId_MemberNameIsTranslated()
        {
            var node = CreateSortingNode("Nested.Id", typeof(int));

            Assert.AreEqual("Nested.Id", node.MemberPath);
        }

        [Test]
        public void CTor_WhenNestedMemberPathStartsWithId_MemberNameIsTranslated()
        {
            var node = CreateSortingNode("Nested.IdTmp", typeof(int));

            Assert.AreEqual("Nested.IdTmp", node.MemberPath);
        }

        [Test]
        public void CTor_WhenNestedMemberPathEndsWithId_MemberNameIsTranslated()
        {
            var node = CreateSortingNode("Nested.TmpId", typeof(int));

            Assert.AreEqual("Nested.TmpId", node.MemberPath);
        }
    }
}