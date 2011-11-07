using NUnit.Framework;
using SisoDb.Querying.Lambdas.Converters.Sql;
using SisoDb.UnitTests.TestFactories;

namespace SisoDb.UnitTests.Querying.Lambdas.Converters.Sql.LambdaToSqlIncludeConverterTests
{
    [TestFixture]
    public class LambdaToSqlIncludeConverterTests : LambdaToSqlIncludeConverterTestBase
    {
        [Test]
        public void Process_WhenFirstLevelMember_GeneratesCorrectSql()
        {
            var lambda = Reflect<Master>.LambdaFrom(m => m.ChildOneId);
            var parsedLambda = CreateParsedLambda<ChildTypeA>(lambda);

            var processor = new LambdaToSqlIncludeConverter();
            var includes = processor.Convert(StructureSchemaTestFactory.Stub<ChildTypeA>(), parsedLambda);

            Assert.AreEqual("min(cs0.Json) as [ChildOne]", includes[0].JsonOutputDefinition);
            Assert.AreEqual("left join [ChildTypeAStructure] as cs0 on cs0.[StructureId] = si.[GuidValue] and si.[MemberPath]='ChildOneId'", includes[0].JoinString);
        }

        [Test]
        public void Process_WhenTwoFirstLevelMembers_GeneratesCorrectSql()
        {
            var lambda1 = Reflect<Master>.LambdaFrom(m => m.ChildOneId);
            var lambda2 = Reflect<Master>.LambdaFrom(m => m.ChildTwoId);
            var parsedLambda = CreateParsedLambda<ChildTypeA>(lambda1, lambda2);

            var processor = new LambdaToSqlIncludeConverter();
            var includes = processor.Convert(StructureSchemaTestFactory.Stub<ChildTypeA>(), parsedLambda);

            Assert.AreEqual("min(cs0.Json) as [ChildOne]", includes[0].JsonOutputDefinition);
            Assert.AreEqual("left join [ChildTypeAStructure] as cs0 on cs0.[StructureId] = si.[GuidValue] and si.[MemberPath]='ChildOneId'", includes[0].JoinString);
            Assert.AreEqual("min(cs1.Json) as [ChildTwo]", includes[1].JsonOutputDefinition);
            Assert.AreEqual("left join [ChildTypeAStructure] as cs1 on cs1.[StructureId] = si.[GuidValue] and si.[MemberPath]='ChildTwoId'", includes[1].JoinString);
        }

        [Test]
        public void Process_WhenFirstLevelAndNestedMember_GeneratesCorrectSql()
        {
            var lambda1 = Reflect<Master>.LambdaFrom(m => m.ChildOneId);
            var lambda2 = Reflect<Master>.LambdaFrom(m => m.NestedItem.UnknownChildId);
            var parsedLambda = CreateParsedLambda<ChildTypeA>(lambda1, lambda2);

            var processor = new LambdaToSqlIncludeConverter();
            var includes = processor.Convert(StructureSchemaTestFactory.Stub<ChildTypeA>(), parsedLambda);

            Assert.AreEqual("min(cs0.Json) as [ChildOne]", includes[0].JsonOutputDefinition);
            Assert.AreEqual("left join [ChildTypeAStructure] as cs0 on cs0.[StructureId] = si.[GuidValue] and si.[MemberPath]='ChildOneId'", includes[0].JoinString);
            Assert.AreEqual("min(cs1.Json) as [NestedItem.UnknownChild]", includes[1].JsonOutputDefinition);
            Assert.AreEqual("left join [ChildTypeAStructure] as cs1 on cs1.[StructureId] = si.[GuidValue] and si.[MemberPath]='NestedItem.UnknownChildId'", includes[1].JoinString);
        }

        [Test]
        public void Process_WhenFirstLevelAndNestedMembersReferencesDifferentTypes_GeneratesCorrectSql()
        {
            var lambda1 = Reflect<Master>.LambdaFrom(m => m.ChildOneId);
            var lambda2 = Reflect<Master>.LambdaFrom(m => m.NestedItem.UnknownChildId);
            var parsedLambda1 = CreateParsedLambda<ChildTypeA>(lambda1);
            var parsedLambda2 = CreateParsedLambda<ChildTypeB>(lambda2);
            var parsedLambda = parsedLambda1.MergeAsNew(parsedLambda2);

            var processor = new LambdaToSqlIncludeConverter();
            var includes = processor.Convert(StructureSchemaTestFactory.Stub<ChildTypeA>(), parsedLambda);

            Assert.AreEqual("min(cs0.Json) as [ChildOne]", includes[0].JsonOutputDefinition);
            Assert.AreEqual("left join [ChildTypeAStructure] as cs0 on cs0.[StructureId] = si.[GuidValue] and si.[MemberPath]='ChildOneId'", includes[0].JoinString);
            Assert.AreEqual("min(cs1.Json) as [NestedItem.UnknownChild]", includes[1].JsonOutputDefinition);
            Assert.AreEqual("left join [ChildTypeBStructure] as cs1 on cs1.[StructureId] = si.[GuidValue] and si.[MemberPath]='NestedItem.UnknownChildId'", includes[1].JoinString);
        }
    }
}