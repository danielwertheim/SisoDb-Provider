using System;
using System.Linq;
using Machine.Specifications;
using SisoDb.PineCone.Structures.Schemas;
using SisoDb.Testing;
using SisoDb.Testing.Steps;

namespace SisoDb.Specifications.Session
{
    class Inserts
    {
        [Subject(typeof(ISession), "Insert")]
        public class when_inserting_item_with_single_string_member_that_is_null : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structure = new SingleStringMember { Value = null };
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<SingleStringMember>();
            };

            Because of =
                () => TestContext.Database.UseOnceTo().Insert(_structure);

            It should_have_one_item_inserted =
                () => TestContext.Database.should_have_X_num_of_items<SingleStringMember>(1);

            It should_have_inserted_an_item_with_string_null_value = () =>
            {
                var refetched = TestContext.Database.UseOnceTo().Query<SingleStringMember>().FirstOrDefault();
                refetched.Value.ShouldBeNull();
            };

            It should_not_have_insterted_null_value_for_string_in_any_indexes_table =
                () => TestContext.DbHelper.AnyIndexesTableHasMember<SingleStringMember>(_structureSchema, _structure.StructureId, x => x.Value).ShouldBeFalse();

            private static SingleStringMember _structure;
            private static IStructureSchema _structureSchema;
        }

        [Subject(typeof(ISession), "InsertMany")]
        public class when_inserting_three_items_with_single_string_member_that_is_null : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<SingleStringMember>();
                _structures = new[]
                {
                    new SingleStringMember {Value = null},
                    new SingleStringMember {Value = null},
                    new SingleStringMember {Value = null}
                };
            };

            Because of = () => TestContext.Database.UseOnceTo().InsertMany(_structures);

            It should_have_three_items_inserted =
                () => TestContext.Database.should_have_X_num_of_items<SingleStringMember>(3);

            It should_have_inserted_item_with_null_values = () =>
            {
                var refetched = TestContext.Database.UseOnceTo().Query<SingleStringMember>().ToArray();
                refetched.All(s => s.Value == null).ShouldBeTrue();
            };

            It should_not_have_insterted_null_value_for_string_in_any_indexes_table = () =>
            {
                foreach (var structure in _structures)
                {
                    TestContext.DbHelper.AnyIndexesTableHasMember<SingleStringMember>(
                        _structureSchema, structure.StructureId, x => x.Value).ShouldBeFalse();
                }
            };

            private static IStructureSchema _structureSchema;
            private static SingleStringMember[] _structures;
        }

        [Subject(typeof(ISession), "Insert")]
        public class when_inserting_item_with_single_datetime_member_that_is_null : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<SingleDateTimeMember>();
                _structure = new SingleDateTimeMember {Value = null};
            };

            Because of = () => TestContext.Database.UseOnceTo().Insert(_structure);

            It should_have_one_item_inserted =
                () => TestContext.Database.should_have_X_num_of_items<SingleDateTimeMember>(1);

            It should_have_inserted_an_item_with_null_values = () =>
            {
                var refetched = TestContext.Database.UseOnceTo().Query<SingleDateTimeMember>().FirstOrDefault();
                refetched.Value.ShouldBeNull();
            };

            It should_not_have_insterted_null_value_for_datetime_in_any_indexes_table =
                () => TestContext.DbHelper.AnyIndexesTableHasMember<SingleDateTimeMember>(_structureSchema, _structure.StructureId, x => x.Value).ShouldBeFalse();

            private static IStructureSchema _structureSchema;
            private static SingleDateTimeMember _structure;
        }
#if Sql2008Provider || Sql2012Provider || SqlProfilerProvider
        [Subject(typeof(ISession), "Insert")]
        public class when_inserting_item_with_single_datetime_member_having_min_value : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
            };

            Because of =
                () => TestContext.Database.UseOnceTo().Insert(new SingleDateTimeMember { Value = DateTime.MinValue });

            It should_have_one_item_inserted =
                () => TestContext.Database.should_have_X_num_of_items<SingleDateTimeMember>(1);

            It should_have_inserted_item_with_min_value = () =>
            {
                var refetched = TestContext.Database.UseOnceTo().Query<SingleDateTimeMember>().FirstOrDefault();
                refetched.Value.ShouldEqual(DateTime.MinValue);
            };
        }

        [Subject(typeof(ISession), "InsertMany")]
        public class when_inserting_items_with_datetimes_having_min_value : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
            };

            Because of = () => TestContext.Database.UseOnceTo().InsertMany(new[]
            {
                new MultiDateTimeMember { Dates = new[] { DateTime.MinValue, DateTime.MinValue } },
                new MultiDateTimeMember { Dates = new[] { DateTime.MinValue, DateTime.MinValue } },
                new MultiDateTimeMember { Dates = new[] { DateTime.MinValue, DateTime.MinValue } }
            });

            It should_have_one_item_inserted =
                () => TestContext.Database.should_have_X_num_of_items<MultiDateTimeMember>(3);

            It should_have_inserted_item_with_min_value = () =>
            {
                var refetched = TestContext.Database.UseOnceTo().Query<MultiDateTimeMember>().ToArray();
                refetched.SelectMany(r => r.Dates).All(d => d.Equals(DateTime.MinValue)).ShouldBeTrue();
            };
        }
#endif
        [Subject(typeof(ISession), "Insert")]
        public class when_inserting_item_with_single_text_member_that_is_null : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structure = new SingleTextMember { Text = null };
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<SingleTextMember>();
            };

            Because of = () => TestContext.Database.UseOnceTo().Insert(_structure);

            It should_have_one_item_inserted =
                () => TestContext.Database.should_have_X_num_of_items<SingleTextMember>(1);

            It should_have_inserted_an_item_with_null_values = () =>
            {
                var refetched = TestContext.Database.UseOnceTo().Query<SingleTextMember>().FirstOrDefault();
                refetched.Text.ShouldBeNull();
            };

            It should_not_have_insterted_null_value_for_string_in_any_indexes_table =
                () => TestContext.DbHelper.AnyIndexesTableHasMember<SingleTextMember>(_structureSchema, _structure.StructureId, x => x.Text).ShouldBeFalse();

            private static SingleTextMember _structure;
            private static IStructureSchema _structureSchema;
        }

        [Subject(typeof(ISession), "InsertMany")]
        public class when_inserting_three_items_with_single_text_member_that_is_null : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<SingleTextMember>();
                _structures = new[]
                {
                    new SingleTextMember {Text = null},
                    new SingleTextMember {Text = null},
                    new SingleTextMember {Text = null}
                };
            };

            Because of = () => TestContext.Database.UseOnceTo().InsertMany(_structures);

            It should_have_three_items_inserted =
                () => TestContext.Database.should_have_X_num_of_items<SingleTextMember>(3);

            It should_have_inserted_item_with_null_values = () =>
            {
                var refetched = TestContext.Database.UseOnceTo().Query<SingleTextMember>().ToArray();
                refetched.All(s => s.Text == null).ShouldBeTrue();
            };

            It should_not_have_insterted_null_value_for_text_in_any_indexes_table = () =>
            {
                foreach (var structure in _structures)
                {
                    TestContext.DbHelper.AnyIndexesTableHasMember<SingleTextMember>(
                        _structureSchema, structure.StructureId, x => x.Text).ShouldBeFalse();
                }
            };

            private static IStructureSchema _structureSchema;
            private static SingleTextMember[] _structures;
        }

        [Subject(typeof(ISession), "Insert")]
        public class when_inserting_item_with_unsigned_members : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _orgItem = new UnsignedMembers { UShort = 42, UInt = 142, ULong = 242 };
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<UnsignedMembers>();
            };

            Because of = () => TestContext.Database.UseOnceTo().Insert(_orgItem);

            It should_have_one_item_inserted =
                () => TestContext.Database.should_have_X_num_of_items<UnsignedMembers>(1);

            It should_have_inserted_one_item_with_correct_unsigned_values = () =>
            {
                var refetched = TestContext.Database.UseOnceTo().GetById<UnsignedMembers>(_orgItem.StructureId);
                refetched.ShouldBeValueEqualTo(_orgItem);
            };

            It should_not_have_inserted_any_ushort_indexes =
                () => TestContext.DbHelper.AnyIndexesTableHasMember<UnsignedMembers>(
                    _structureSchema,
                    _orgItem.StructureId,
                    x => x.UShort).ShouldBeFalse();

            It should_not_have_inserted_any_uint_indexes =
                () => TestContext.DbHelper.AnyIndexesTableHasMember<UnsignedMembers>(
                    _structureSchema,
                    _orgItem.StructureId,
                    x => x.UInt).ShouldBeFalse();

            It should_not_have_inserted_any_ulong_indexes =
                () => TestContext.DbHelper.AnyIndexesTableHasMember<UnsignedMembers>(
                    _structureSchema,
                    _orgItem.StructureId,
                    x => x.ULong).ShouldBeFalse();

            private static UnsignedMembers _orgItem;
            private static IStructureSchema _structureSchema;
        }

        private class SingleStringMember
        {
            public Guid StructureId { get; set; }
            public string Value { get; set; }
        }

        private class SingleTextMember
        {
            public Guid StructureId { get; set; }
            public string Text { get; set; }
        }

        private class SingleDateTimeMember
        {
            public Guid StructureId { get; set; }
            public DateTime? Value { get; set; }
        }

        private class MultiDateTimeMember
        {
            public Guid StructureId { get; set; }
            public DateTime[] Dates { get; set; }
        }

        private class UnsignedMembers
        {
            public Guid StructureId { get; set; }
            public ushort UShort { get; set; }
            public uint UInt { get; set; }
            public ulong ULong { get; set; }
        }
    }
}