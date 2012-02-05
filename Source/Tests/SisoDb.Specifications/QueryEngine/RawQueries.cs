using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;
using SisoDb.Dac;
using SisoDb.Querying;
using SisoDb.Specifications.Model;
using SisoDb.Testing;

namespace SisoDb.Specifications.QueryEngine
{
	class RawQueries
    {
        [Subject(typeof(ISession), "Raw Query")]
        public class when_raw_query_returns_no_result : SpecificationBase
        {
            Establish context = () => TestContext = TestContextFactory.Create();

            Because of = () =>
            {
				var query = new RawQuery(@"select '{}' as Json where 1=2;");
                _fetchedStructures = TestContext.Database.UseOnceTo().RawQuery<QueryGuidItem>(query).ToList();
            };

            It should_have_fetched_0_structures =
                () => _fetchedStructures.Count.ShouldEqual(0);

            private static IList<QueryGuidItem> _fetchedStructures;
        }

		[Subject(typeof(ISession), "Raw Query as Json")]
		public class when_raw_query_returns_no_json_result : SpecificationBase
		{
			Establish context = () => TestContext = TestContextFactory.Create();

			Because of = () =>
			{
				var query = new RawQuery(@"select '{}' as Json where 1=2;");
				_fetchedStructures = TestContext.Database.UseOnceTo().RawQueryAsJson<QueryGuidItem>(query).ToList();
			};

			It should_have_fetched_0_structures =
				() => _fetchedStructures.Count.ShouldEqual(0);

			private static IList<string> _fetchedStructures;
		}

		[Subject(typeof(ISession), "Raw Query")]
		public class when_raw_query_with_parameters : SpecificationBase
		{
			Establish context = () =>
			{
				TestContext = TestContextFactory.Create();
				_structures = QueryGuidItem.CreateFourItems<QueryGuidItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
			};

			Because of = () =>
			{
				var query = new RawQuery("select s.[Json] from (select s.[StructureId] from [QueryGuidItemStructure] s inner join [QueryGuidItemIntegers] mem0 on mem0.[StructureId] = s.[StructureId] and mem0.[MemberPath] = 'SortOrder' where (mem0.[Value] between @minSortOrder and @maxSortOrder) group by s.[StructureId]) rs inner join [QueryGuidItemStructure] s on s.[StructureId] = rs.[StructureId];");
				query.Add(
					new DacParameter("minSortOrder", _structures[1].SortOrder),
					new DacParameter("maxSortOrder", _structures[2].SortOrder));

				_fetchedStructures = TestContext.Database.UseOnceTo().RawQuery<QueryGuidItem>(query).ToList();
			};

			It should_have_fetched_two_structures =
				() => _fetchedStructures.Count.ShouldEqual(2);

			It should_have_fetched_the_two_middle_structures = () =>
			{
				_fetchedStructures[0].ShouldBeValueEqualTo(_structures[1]);
				_fetchedStructures[1].ShouldBeValueEqualTo(_structures[2]);
			};

			private static IList<QueryGuidItem> _structures;
			private static IList<QueryGuidItem> _fetchedStructures;
		}

		[Subject(typeof(ISession), "Raw Query as Json")]
		public class when_raw_query_with_parameters_returning_json : SpecificationBase
		{
			Establish context = () =>
			{
				TestContext = TestContextFactory.Create();
				_structures = QueryGuidItem.CreateFourItems<QueryGuidItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
			};

			Because of = () =>
			{
				var query = new RawQuery("select s.[Json] from (select s.[StructureId] from [QueryGuidItemStructure] s inner join [QueryGuidItemIntegers] mem0 on mem0.[StructureId] = s.[StructureId] and mem0.[MemberPath] = 'SortOrder' where (mem0.[Value] between @minSortOrder and @maxSortOrder) group by s.[StructureId]) rs inner join [QueryGuidItemStructure] s on s.[StructureId] = rs.[StructureId];");
				query.Add(
					new DacParameter("minSortOrder", _structures[1].SortOrder),
					new DacParameter("maxSortOrder", _structures[2].SortOrder));

				_fetchedStructures = TestContext.Database.UseOnceTo().RawQueryAsJson<QueryGuidItem>(query).ToList();
			};

			It should_have_fetched_two_structures =
				() => _fetchedStructures.Count.ShouldEqual(2);

			It should_have_fetched_the_two_middle_structures = () =>
			{
				_fetchedStructures[0].ShouldEqual(_structures[1].AsJson());
				_fetchedStructures[1].ShouldEqual(_structures[2].AsJson());
			};

			private static IList<QueryGuidItem> _structures;
			private static IList<string> _fetchedStructures;
		}
	}
}