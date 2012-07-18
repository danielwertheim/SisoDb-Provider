using System;
using System.Collections.Generic;
using Machine.Specifications;
using SisoDb.Specifications.Model;
using SisoDb.Testing;

namespace SisoDb.Specifications.Session.Querying
{
    class GetById
    {
        [Subject(typeof(ISession), "Get by Id (guid)")]
        public class when_set_with_guid_id_is_empty : SpecificationBase
        {
            Establish context = () => TestContext = TestContextFactory.Create();

            Because of = () =>
                _fetchedStructure = TestContext.Database.UseOnceTo().GetById<QueryGuidItem>(Guid.Parse("ABF5FC75-1E74-4564-B55A-DB3594394BE3"));

            It should_not_fetch_any_structure =
                () => _fetchedStructure.ShouldBeNull();

            private static QueryGuidItem _fetchedStructure;
        }

        [Subject(typeof(ISession), "Get by Id (string)")]
        public class when_set_with_string_id_is_empty : SpecificationBase
        {
            Establish context = () => TestContext = TestContextFactory.Create();

            Because of = () =>
                _fetchedStructure = TestContext.Database.UseOnceTo().GetById<QueryStringItem>("Foo");

            It should_not_fetch_any_structure =
                () => _fetchedStructure.ShouldBeNull();

            private static QueryStringItem _fetchedStructure;
        }

        [Subject(typeof(ISession), "Get by Id (identity)")]
        public class when_set_with_identity_id_is_empty : SpecificationBase
        {
            Establish context = () => TestContext = TestContextFactory.Create();

            Because of = () =>
                _fetchedStructure = TestContext.Database.UseOnceTo().GetById<QueryIdentityItem>(42);

            It should_not_fetch_any_structure =
                () => _fetchedStructure.ShouldBeNull();

            private static QueryIdentityItem _fetchedStructure;
        }

        [Subject(typeof(ISession), "Get by Id (big identity)")]
        public class when_set_with_big_identity_id_is_empty : SpecificationBase
        {
            Establish context = () => TestContext = TestContextFactory.Create();

            Because of = () =>
                _fetchedStructure = TestContext.Database.UseOnceTo().GetById<QueryBigIdentityItem>(42);

            It should_not_fetch_any_structure =
                () => _fetchedStructure.ShouldBeNull();

            private static QueryBigIdentityItem _fetchedStructure;
        }

        [Subject(typeof(ISession), "Get by Id as Json (guid)")]
        public class when_json_set_with_guid_id_is_empty : SpecificationBase
        {
            Establish context = () => TestContext = TestContextFactory.Create();

            Because of = () =>
                _fetchedStructure = TestContext.Database.UseOnceTo().GetByIdAsJson<QueryGuidItem>(Guid.Parse("ABF5FC75-1E74-4564-B55A-DB3594394BE3"));

            It should_not_fetch_any_structure =
                () => _fetchedStructure.ShouldBeNull();

            private static string _fetchedStructure;
        }

        [Subject(typeof(ISession), "Get by Id as Json (string)")]
        public class when_json_set_with_string_id_is_empty : SpecificationBase
        {
            Establish context = () => TestContext = TestContextFactory.Create();

            Because of = () =>
                _fetchedStructure = TestContext.Database.UseOnceTo().GetByIdAsJson<QueryStringItem>("Foo");

            It should_not_fetch_any_structure =
                () => _fetchedStructure.ShouldBeNull();

            private static string _fetchedStructure;
        }

        [Subject(typeof(ISession), "Get by Id as Json (identity)")]
        public class when_json_set_with_identity_id_is_empty : SpecificationBase
        {
            Establish context = () => TestContext = TestContextFactory.Create();

            Because of = () =>
                _fetchedStructure = TestContext.Database.UseOnceTo().GetByIdAsJson<QueryIdentityItem>(42);

            It should_not_fetch_any_structure =
                () => _fetchedStructure.ShouldBeNull();

            private static string _fetchedStructure;
        }

        [Subject(typeof(ISession), "Get by Id as Json (big identity)")]
        public class when_json_set_with_big_identity_id_is_empty : SpecificationBase
        {
            Establish context = () => TestContext = TestContextFactory.Create();

            Because of = () =>
                _fetchedStructure = TestContext.Database.UseOnceTo().GetByIdAsJson<QueryBigIdentityItem>(42);

            It should_not_fetch_any_structure =
                () => _fetchedStructure.ShouldBeNull();

            private static string _fetchedStructure;
        }

        [Subject(typeof(ISession), "Get by Id (guid)")]
        public class when_set_with_guid_id_contains_four_items : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryGuidItem.CreateFourItems<QueryGuidItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of =
                () => _fetchedStructure = TestContext.Database.UseOnceTo().GetById<QueryGuidItem>(_structures[1].StructureId);

            It should_fetch_the_structure = 
                () => _fetchedStructure.ShouldBeValueEqualTo(_structures[1]);

            private static IList<QueryGuidItem> _structures;
            private static QueryGuidItem _fetchedStructure;
        }

        [Subject(typeof(ISession), "Get by Id (string)")]
        public class when_set_with_string_id_contains_four_items : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryStringItem.CreateFourItems<QueryStringItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of =
                () => _fetchedStructure = TestContext.Database.UseOnceTo().GetById<QueryStringItem>(_structures[1].StructureId);

            It should_fetch_the_structure =
                () => _fetchedStructure.ShouldBeValueEqualTo(_structures[1]);

            private static IList<QueryStringItem> _structures;
            private static QueryStringItem _fetchedStructure;
        }

        [Subject(typeof(ISession), "Get by Id (identity)")]
        public class when_set_with_identity_id_contains_four_items : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryIdentityItem.CreateFourItems<QueryIdentityItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of =
                () => _fetchedStructure = TestContext.Database.UseOnceTo().GetById<QueryIdentityItem>(_structures[1].StructureId);

            It should_fetch_the_structure =
                () => _fetchedStructure.ShouldBeValueEqualTo(_structures[1]);

            private static IList<QueryIdentityItem> _structures;
            private static QueryIdentityItem _fetchedStructure;
        }

        [Subject(typeof(ISession), "Get by Id (big identity)")]
        public class when_set_with_big_identity_id_contains_four_items : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryBigIdentityItem.CreateFourItems<QueryBigIdentityItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of =
                () => _fetchedStructure = TestContext.Database.UseOnceTo().GetById<QueryBigIdentityItem>(_structures[1].StructureId);

            It should_fetch_the_structure =
                () => _fetchedStructure.ShouldBeValueEqualTo(_structures[1]);

            private static IList<QueryBigIdentityItem> _structures;
            private static QueryBigIdentityItem _fetchedStructure;
        }

        [Subject(typeof(ISession), "Get by Id (guid)")]
        public class when_set_with_guid_id_contains_four_items_using_non_generic_api : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryGuidItem.CreateFourItems<QueryGuidItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of =
                () => _fetchedStructure = TestContext.Database.UseOnceTo().GetById(typeof(QueryGuidItem), _structures[1].StructureId) as QueryGuidItem;

            It should_fetch_the_structure =
                () => _fetchedStructure.ShouldBeValueEqualTo(_structures[1]);

            private static IList<QueryGuidItem> _structures;
            private static QueryGuidItem _fetchedStructure;
        }

        [Subject(typeof(ISession), "Get by Id (string)")]
        public class when_set_with_string_id_contains_four_items_using_non_generic_api : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryStringItem.CreateFourItems<QueryStringItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of =
                () => _fetchedStructure = TestContext.Database.UseOnceTo().GetById(typeof(QueryStringItem), _structures[1].StructureId) as QueryStringItem;

            It should_fetch_the_structure =
                () => _fetchedStructure.ShouldBeValueEqualTo(_structures[1]);

            private static IList<QueryStringItem> _structures;
            private static QueryStringItem _fetchedStructure;
        }

        [Subject(typeof(ISession), "Get by Id (identity)")]
        public class when_set_with_identity_id_contains_four_items_using_non_generic_api : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryIdentityItem.CreateFourItems<QueryIdentityItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of =
                () => _fetchedStructure = TestContext.Database.UseOnceTo().GetById(typeof(QueryIdentityItem), _structures[1].StructureId) as QueryIdentityItem;

            It should_fetch_the_structure =
                () => _fetchedStructure.ShouldBeValueEqualTo(_structures[1]);

            private static IList<QueryIdentityItem> _structures;
            private static QueryIdentityItem _fetchedStructure;
        }

        [Subject(typeof(ISession), "Get by Id (big identity)")]
        public class when_set_with_big_identity_id_contains_four_items_using_non_generic_api : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryBigIdentityItem.CreateFourItems<QueryBigIdentityItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of =
                () => _fetchedStructure = TestContext.Database.UseOnceTo().GetById(typeof(QueryBigIdentityItem), _structures[1].StructureId) as QueryBigIdentityItem;

            It should_fetch_the_structure =
                () => _fetchedStructure.ShouldBeValueEqualTo(_structures[1]);

            private static IList<QueryBigIdentityItem> _structures;
            private static QueryBigIdentityItem _fetchedStructure;
        }
        
        [Subject(typeof(ISession), "Get by Id as Json (guid)")]
        public class when_json_set_with_guid_id_contains_four_items : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryGuidItem.CreateFourItems<QueryGuidItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of =
                () => _fetchedStructure = TestContext.Database.UseOnceTo().GetByIdAsJson<QueryGuidItem>(_structures[1].StructureId);

            It should_fetch_the_structure =
                () => _fetchedStructure.ShouldEqual(_structures[1].AsJson());

            private static IList<QueryGuidItem> _structures;
            private static string _fetchedStructure;
        }

        [Subject(typeof(ISession), "Get by Id as Json (string)")]
        public class when_json_set_with_string_id_contains_four_items : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryStringItem.CreateFourItems<QueryStringItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of =
                () => _fetchedStructure = TestContext.Database.UseOnceTo().GetByIdAsJson<QueryStringItem>(_structures[1].StructureId);

            It should_fetch_the_structure =
                () => _fetchedStructure.ShouldEqual(_structures[1].AsJson());

            private static IList<QueryStringItem> _structures;
            private static string _fetchedStructure;
        }

        [Subject(typeof(ISession), "Get by Id as Json (identity)")]
        public class when_json_set_with_identity_id_contains_four_items : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryIdentityItem.CreateFourItems<QueryIdentityItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of =
                () => _fetchedStructure = TestContext.Database.UseOnceTo().GetByIdAsJson<QueryIdentityItem>(_structures[1].StructureId);

            It should_fetch_the_structure =
                () => _fetchedStructure.ShouldEqual(_structures[1].AsJson());

            private static IList<QueryIdentityItem> _structures;
            private static string _fetchedStructure;
        }

        [Subject(typeof(ISession), "Get by Id as Json (big identity)")]
        public class when_json_set_with_big_identity_id_contains_four_items : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryBigIdentityItem.CreateFourItems<QueryBigIdentityItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of =
                () => _fetchedStructure = TestContext.Database.UseOnceTo().GetByIdAsJson<QueryBigIdentityItem>(_structures[1].StructureId);

            It should_fetch_the_structure =
                () => _fetchedStructure.ShouldEqual(_structures[1].AsJson());

            private static IList<QueryBigIdentityItem> _structures;
            private static string _fetchedStructure;
        }

        [Subject(typeof(ISession), "Get by Id (guid)")]
        public class when_set_with_guid_contains_four_items_that_are_in_uncommitted_mode : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryGuidItem.CreateFourItems<QueryGuidItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    session.InsertMany(_structures);

                    _fetchedStructure = session.GetById<QueryGuidItem>(_structures[1].StructureId);
                }
            };

            It should_fetch_the_structure =
                () => _fetchedStructure.ShouldBeValueEqualTo(_structures[1]);

            private static IList<QueryGuidItem> _structures;
            private static QueryGuidItem _fetchedStructure;
        }
    }
}