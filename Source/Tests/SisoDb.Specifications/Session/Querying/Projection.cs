using System.Collections.Generic;
using Machine.Specifications;
using SisoDb.Specifications.Model;
using SisoDb.Testing;

namespace SisoDb.Specifications.Session.Querying
{
    class Projection
    {
        [Subject(typeof(ISisoQueryable<>), "Projection")]
        public class when_mapping_properties_one_to_one_using_anonymous_type : SpecificationBase
        {
            Establish context = () =>
            {
                _structures = new[]
                {
                    new QueryGuidItem {IntegerValue = 1, NullableIntegerValue = null, StringValue = "A"},
                    new QueryGuidItem {IntegerValue = 2, NullableIntegerValue = 1, StringValue = "B"},
                    new QueryGuidItem {IntegerValue = 3, NullableIntegerValue = 2, StringValue = "C"}
                };

                TestContext = TestContextFactory.Create();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () =>
                _result = TestContext.Database.UseOnceTo().Query<QueryGuidItem>().ToArrayOf(x => new
                {
                    x.IntegerValue, 
                    x.NullableIntegerValue, 
                    x.StringValue
                });

            It should_have_returned_all_three_items = () =>
                _result.Length.ShouldEqual(3);

            It should_projected_all_three_items = () =>
            {
                ShouldHaveMappedCorrectly(_structures[0], _result[0]);
                ShouldHaveMappedCorrectly(_structures[1], _result[1]);
                ShouldHaveMappedCorrectly(_structures[2], _result[2]);
            };

            private static void ShouldHaveMappedCorrectly(QueryGuidItem original, dynamic result)
            {
                original.NullableIntegerValue.ShouldEqual((int?)result.NullableIntegerValue);
                original.IntegerValue.ShouldEqual((int)result.IntegerValue);
                original.StringValue.ShouldEqual((string)result.StringValue);
            }

            private static IList<QueryGuidItem> _structures;
            private static dynamic[] _result;
        }
    }
}