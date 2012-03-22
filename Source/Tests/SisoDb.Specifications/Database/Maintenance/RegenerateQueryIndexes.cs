using System;
using Machine.Specifications;
using PineCone.Structures.Schemas;
using SisoDb.Testing;

namespace SisoDb.Specifications.Database.Maintenance
{
    class RegenerateQueryIndexes
    {
        [Subject(typeof(ISisoDatabaseMaintenance), "RegenerateQueryIndexes")]
        public class when_manually_dropped_query_indexes_and_regenerating : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();

                _structures = new[] { MyStructure.Create(), MyStructure.Create() };
                TestContext.Database.UseOnceTo().InsertMany(_structures);

                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<MyStructure>();
                TestContext.DbHelper.DeleteQueryIndexesFor(_structureSchema, new[] { _structures[0].StructureId });
            };

            Because of = () => TestContext.Database.Maintenance.RegenerateQueryIndexesFor<MyStructure>();

            It should_have_one_integer_value_for_structure_one = () =>
                TestContext.DbHelper.AnyIndexesTableHasMember<MyStructure>(_structureSchema, _structures[0].StructureId, m => m.IntValue);

            It should_have_one_integer_value_for_structure_two = () =>
                TestContext.DbHelper.AnyIndexesTableHasMember<MyStructure>(_structureSchema, _structures[1].StructureId, m => m.IntValue);

            It should_have_one_decimal_value_for_structure_one = () =>
                TestContext.DbHelper.AnyIndexesTableHasMember<MyStructure>(_structureSchema, _structures[0].StructureId, m => m.DecimalValue);

            It should_have_one_decimal_value_for_structure_two = () =>
                TestContext.DbHelper.AnyIndexesTableHasMember<MyStructure>(_structureSchema, _structures[1].StructureId, m => m.DecimalValue);

            It should_have_one_guid_value_for_structure_one = () =>
                TestContext.DbHelper.AnyIndexesTableHasMember<MyStructure>(_structureSchema, _structures[0].StructureId, m => m.GuidValue);

            It should_have_one_guid_value_for_structure_two = () =>
                TestContext.DbHelper.AnyIndexesTableHasMember<MyStructure>(_structureSchema, _structures[1].StructureId, m => m.GuidValue);

            It should_have_one_bool_value_for_structure_one = () =>
                TestContext.DbHelper.AnyIndexesTableHasMember<MyStructure>(_structureSchema, _structures[0].StructureId, m => m.BoolValue);

            It should_have_one_bool_value_for_structure_two = () =>
                TestContext.DbHelper.AnyIndexesTableHasMember<MyStructure>(_structureSchema, _structures[1].StructureId, m => m.BoolValue);

            It should_have_one_datetime_value_for_structure_one = () =>
                TestContext.DbHelper.AnyIndexesTableHasMember<MyStructure>(_structureSchema, _structures[0].StructureId, m => m.DateTimeValue);

            It should_have_one_datetime_value_for_structure_two = () =>
                TestContext.DbHelper.AnyIndexesTableHasMember<MyStructure>(_structureSchema, _structures[1].StructureId, m => m.DateTimeValue);

            It should_have_one_string_value_for_structure_one = () =>
                TestContext.DbHelper.AnyIndexesTableHasMember<MyStructure>(_structureSchema, _structures[0].StructureId, m => m.StringValue);

            It should_have_one_string_value_for_structure_two = () =>
                TestContext.DbHelper.AnyIndexesTableHasMember<MyStructure>(_structureSchema, _structures[1].StructureId, m => m.StringValue);

            It should_have_one_text_value_for_structure_one = () =>
                TestContext.DbHelper.AnyIndexesTableHasMember<MyStructure>(_structureSchema, _structures[0].StructureId, m => m.TextValue);

            It should_have_one_text_value_for_structure_two = () =>
                TestContext.DbHelper.AnyIndexesTableHasMember<MyStructure>(_structureSchema, _structures[1].StructureId, m => m.TextValue);

            private static IStructureSchema _structureSchema;
            private static MyStructure[] _structures;
        }

        private class MyStructure
        {
            public Guid StructureId { get; set; }
            public int IntValue { get; set; }
            public decimal DecimalValue { get; set; }
            public Guid GuidValue { get; set; }
            public bool BoolValue { get; set; }
            public DateTime DateTimeValue { get; set; }
            public string StringValue { get; set; }
            public Text TextValue { get; set; }

            public static MyStructure Create()
            {
                return new MyStructure
                {
                    IntValue = 42,
                    DecimalValue = 13.3M,
                    GuidValue = Guid.NewGuid(),
                    BoolValue = false,
                    DateTimeValue = new DateTime(2012, 2, 3, 11, 32, 59),
                    StringValue = "Foo",
                    TextValue = "Bar"
                };
            }
        }
    }
}