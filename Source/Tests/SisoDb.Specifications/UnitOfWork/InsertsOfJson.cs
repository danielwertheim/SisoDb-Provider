using System;
using System.Linq;
using Machine.Specifications;
using NCore;
using SisoDb.Testing;
using SisoDb.Testing.TestModel;
using SisoDb.Testing.Steps;

namespace SisoDb.Specifications.UnitOfWork
{
	class InsertsOfJson
    {
        [Subject(typeof(IUnitOfWork), "Insert (json)")]
        public class when_json_is_inserted : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _json = "{\"String1\":\"1\",\"Int1\":1,\"Decimal1\":0.1,\"DateTime1\":\"\\/Date(946681200000+0000)\\/\",\"Ints\":[1,2]}";
            };

            Because of = () =>
            {
                using(var uow = TestContext.Database.CreateUnitOfWork())
                {
                    uow.InsertJson<JsonItem>(_json);
                    uow.Commit();
                }
            };

            It should_have_been_inserted = 
                () => TestContext.Database.should_have_X_num_of_items<JsonItem>(1);

            It should_read_back_as_json_correctly = () =>
            {
                string json;

                using (var qe = TestContext.Database.CreateReadSession())
                    json = qe.Query<JsonItem>().SingleAsJson();

                var jsonWithoutStructureId = "{" + json.Remove(0, 50);
                jsonWithoutStructureId.ShouldEqual(_json);
            };

            It should_map_back_to_model = () =>
            {
                JsonItem structure;

                using (var qe = TestContext.Database.CreateReadSession())
                {
                    structure = qe.Query<JsonItem>().Single();
                }

                structure.String1.ShouldEqual("1");
                structure.Int1.ShouldEqual(1);
                structure.Decimal1.ShouldEqual(0.1m);
                structure.DateTime1.ShouldEqual(new DateTime(2000, 1, 1));
                structure.Ints.ShouldEqual(new[] { 1, 2 });
            };

            private static string _json;
        }

        [Subject(typeof(IUnitOfWork), "Insert (json)")]
        public class when_json_with_value_for_id_is_inserted : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _idString = "f767c34625fce011983e544249037e42";
                _json = "{{\"StructureId\":\"{0}\",\"String1\":\"1\"}}".Inject(_idString);
            };

            Because of = () =>
            {
                using (var uow = TestContext.Database.CreateUnitOfWork())
                {
                    uow.InsertJson<JsonItem>(_json);
                    uow.Commit();
                }
            };

            It should_have_been_inserted =
                () => TestContext.Database.should_have_X_num_of_items<JsonItem>(1);

            It should_not_have_stored_any_structure_with_the_json_id =
                () => TestContext.Database.should_not_have_ids<JsonItem>(Guid.Parse(_idString));

            It should_have_assigned_new_value_for_id = () =>
            {
                JsonItem structure;
                
                using (var qe = TestContext.Database.CreateReadSession())
                    structure = qe.Query<JsonItem>().Single();

                structure.StructureId.ShouldNotEqual(Guid.Parse(_idString));
            };

            private static string _json;
            private static string _idString;
        }

        [Subject(typeof(IUnitOfWork), "Insert (json)")]
        public class when_json_with_wrong_member_casing_is_inserted : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _json = "{\"STRING1\":\"1\"}";
            };

            Because of = () =>
            {
                using (var uow = TestContext.Database.CreateUnitOfWork())
                {
                    uow.InsertJson<JsonItem>(_json);
                    uow.Commit();
                }
            };

            It should_have_been_inserted =
                () => TestContext.Database.should_have_X_num_of_items<JsonItem>(1);

            It should_not_map_back_member_with_wrong_casing = () =>
            {
                JsonItem structure;

                using (var qe = TestContext.Database.CreateReadSession())
                    structure = qe.Query<JsonItem>().Single();

                structure.String1.ShouldNotEqual("1");
            };

            private static string _json;
        }
    }
}