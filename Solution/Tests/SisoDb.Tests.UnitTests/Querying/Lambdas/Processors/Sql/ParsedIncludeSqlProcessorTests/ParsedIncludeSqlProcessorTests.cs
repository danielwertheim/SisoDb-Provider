using NUnit.Framework;
using SisoDb.Querying.Lambdas.Processors.Sql;
using SisoDb.Reflections;

namespace SisoDb.Tests.UnitTests.Querying.Lambdas.Processors.Sql.ParsedIncludeSqlProcessorTests
{
    [TestFixture]
    public class ParsedIncludeSqlProcessorTests : ParsedIncludeSqlProcessorTestBase
    {
        [Test]
        public void Process_WhenFirstLevelMember_GeneratesCorrectSql()
        {
            var lambda = Reflect<Master>.LambdaFrom(m => m.ChildOneId);
            var parsedLambda = CreateParsedLambda<ChildTypeA>(lambda);

            var processor = new ParsedIncludeSqlProcessor(new MemberNameGeneratorFake());
            var includes = processor.Process(parsedLambda);

            const string expectedSql = "(select cs0.[json] from [dbo].[ChildTypeAStructure] as cs0 where si.[ChildOneId] = cs0.SisoId) as [ChildOne]";
            Assert.AreEqual(expectedSql, includes[0].Sql);
        }

        [Test]
        public void Process_WhenTwoFirstLevelMembers_GeneratesCorrectSql()
        {
            var lambda1 = Reflect<Master>.LambdaFrom(m => m.ChildOneId);
            var lambda2 = Reflect<Master>.LambdaFrom(m => m.ChildTwoId);
            var parsedLambda = CreateParsedLambda<ChildTypeA>(lambda1, lambda2);

            var processor = new ParsedIncludeSqlProcessor(new MemberNameGeneratorFake());
            var includes = processor.Process(parsedLambda);

            const string expectedSql1 = "(select cs0.[json] from [dbo].[ChildTypeAStructure] as cs0 where si.[ChildOneId] = cs0.SisoId) as [ChildOne]";
            const string expectedSql2 = "(select cs1.[json] from [dbo].[ChildTypeAStructure] as cs1 where si.[ChildTwoId] = cs1.SisoId) as [ChildTwo]";
            Assert.AreEqual(expectedSql1, includes[0].Sql);
            Assert.AreEqual(expectedSql2, includes[1].Sql);
        }

        [Test]
        public void Process_WhenFirstLevelAndNestedMember_GeneratesCorrectSql()
        {
            var lambda1 = Reflect<Master>.LambdaFrom(m => m.ChildOneId);
            var lambda2 = Reflect<Master>.LambdaFrom(m => m.NestedItem.UnknownChildId);
            var parsedLambda = CreateParsedLambda<ChildTypeA>(lambda1, lambda2);

            var processor = new ParsedIncludeSqlProcessor(new MemberNameGeneratorFake());
            var includes = processor.Process(parsedLambda);

            const string expectedSql1 = "(select cs0.[json] from [dbo].[ChildTypeAStructure] as cs0 where si.[ChildOneId] = cs0.SisoId) as [ChildOne]";
            const string expectedSql2 = "(select cs1.[json] from [dbo].[ChildTypeAStructure] as cs1 where si.[NestedItem.UnknownChildId] = cs1.SisoId) as [NestedItem.UnknownChild]";
            Assert.AreEqual(expectedSql1, includes[0].Sql);
            Assert.AreEqual(expectedSql2, includes[1].Sql);
        }

        [Test]
        public void Process_WhenFirstLevelAndNestedMembersReferencesDifferentTypes_GeneratesCorrectSql()
        {
            var lambda1 = Reflect<Master>.LambdaFrom(m => m.ChildOneId);
            var lambda2 = Reflect<Master>.LambdaFrom(m => m.NestedItem.UnknownChildId);
            var parsedLambda1 = CreateParsedLambda<ChildTypeA>(lambda1);
            var parsedLambda2 = CreateParsedLambda<ChildTypeB>(lambda2);
            var parsedLambda = parsedLambda1.MergeAsNew(parsedLambda2);

            var processor = new ParsedIncludeSqlProcessor(new MemberNameGeneratorFake());
            var includes = processor.Process(parsedLambda);

            const string expectedSql1 = "(select cs0.[json] from [dbo].[ChildTypeAStructure] as cs0 where si.[ChildOneId] = cs0.SisoId) as [ChildOne]";
            const string expectedSql2 = "(select cs1.[json] from [dbo].[ChildTypeBStructure] as cs1 where si.[NestedItem.UnknownChildId] = cs1.SisoId) as [NestedItem.UnknownChild]";
            Assert.AreEqual(expectedSql1, includes[0].Sql);
            Assert.AreEqual(expectedSql2, includes[1].Sql);
        }
    }
}