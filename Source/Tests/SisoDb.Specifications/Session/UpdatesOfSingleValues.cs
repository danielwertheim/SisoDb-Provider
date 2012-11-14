using System;
using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;
using SisoDb.Testing;

namespace SisoDb.Specifications.Session
{
    class UpdatesOfSingleValues
    {
        [Subject(typeof(ISession), "Update (when single string member")]
        public class when_updating_item_with_single_string_member_to_null : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _item = new SingleStringMember { Value = "I have a value" };
                TestContext.Database.UseOnceTo().Insert(_item);
            };

            Because of = () =>
                TestContext.Database.UseOnceTo().Update<SingleStringMember>(_item.StructureId, i => i.Value = null);

            It should_have_one_item_stored =
                () => TestContext.Database.should_have_X_num_of_items<SingleStringMember>(1);

            It should_have_updated_string_member_to_null = () =>
            {
                var refetched = TestContext.Database.UseOnceTo().Query<SingleStringMember>().FirstOrDefault();
                refetched.ShouldNotBeNull();
                refetched.Value.ShouldBeNull();
            };

            private static SingleStringMember _item;
        }

        [Subject(typeof(ISession), "Update (when single datetime member")]
        public class when_updating_item_with_single_datetime_member_to_null : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _item = new SingleDateTimeMember { Value = new DateTime(2012, 1, 1) };
                TestContext.Database.UseOnceTo().Insert(_item);
            };

            Because of = () =>
                TestContext.Database.UseOnceTo().Update<SingleDateTimeMember>(_item.StructureId, i => i.Value = null);

            It should_have_one_item_stored =
                () => TestContext.Database.should_have_X_num_of_items<SingleDateTimeMember>(1);

            It should_have_updated_datetime_member_to_null = () =>
            {
                var refetched = TestContext.Database.UseOnceTo().Query<SingleDateTimeMember>().FirstOrDefault();
                refetched.ShouldNotBeNull();
                refetched.Value.ShouldBeNull();
            };

            private static SingleDateTimeMember _item;
        }

        [Subject(typeof(ISession), "Update (when single int member")]
        public class when_updating_item_with_single_int_member_to_null : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _item = new SingleIntMember { Value = 42 };
                TestContext.Database.UseOnceTo().Insert(_item);
            };

            Because of = () =>
                TestContext.Database.UseOnceTo().Update<SingleIntMember>(_item.StructureId, i => i.Value = null);

            It should_have_one_item_stored =
                () => TestContext.Database.should_have_X_num_of_items<SingleIntMember>(1);

            It should_have_updated_int_member_to_null = () =>
            {
                var refetched = TestContext.Database.UseOnceTo().Query<SingleIntMember>().FirstOrDefault();
                refetched.ShouldNotBeNull();
                refetched.Value.ShouldBeNull();
            };

            private static SingleIntMember _item;
        }

        [Subject(typeof(ISession), "Update (when single long member")]
        public class when_updating_item_with_single_long_member_to_null : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _item = new SingleLongMember { Value = 42 };
                TestContext.Database.UseOnceTo().Insert(_item);
            };

            Because of = () =>
                TestContext.Database.UseOnceTo().Update<SingleLongMember>(_item.StructureId, i => i.Value = null);

            It should_have_one_item_stored =
                () => TestContext.Database.should_have_X_num_of_items<SingleLongMember>(1);

            It should_have_updated_long_member_to_null = () =>
            {
                var refetched = TestContext.Database.UseOnceTo().Query<SingleLongMember>().FirstOrDefault();
                refetched.ShouldNotBeNull();
                refetched.Value.ShouldBeNull();
            };

            private static SingleLongMember _item;
        }

        [Subject(typeof(ISession), "Update (when single bool member")]
        public class when_updating_item_with_single_bool_member_to_null : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _item = new SingleBoolMember { Value = true };
                TestContext.Database.UseOnceTo().Insert(_item);
            };

            Because of = () =>
                TestContext.Database.UseOnceTo().Update<SingleBoolMember>(_item.StructureId, i => i.Value = null);

            It should_have_one_item_stored =
                () => TestContext.Database.should_have_X_num_of_items<SingleBoolMember>(1);

            It should_have_updated_bool_member_to_null = () =>
            {
                var refetched = TestContext.Database.UseOnceTo().Query<SingleBoolMember>().FirstOrDefault();
                refetched.ShouldNotBeNull();
                refetched.Value.ShouldBeNull();
            };

            private static SingleBoolMember _item;
        }

        [Subject(typeof(ISession), "Update (when single decimal member")]
        public class when_updating_item_with_single_decimal_member_to_null : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _item = new SingleDecimalMember { Value = 42.33M };
                TestContext.Database.UseOnceTo().Insert(_item);
            };

            Because of = () =>
                TestContext.Database.UseOnceTo().Update<SingleDecimalMember>(_item.StructureId, i => i.Value = null);

            It should_have_one_item_stored =
                () => TestContext.Database.should_have_X_num_of_items<SingleDecimalMember>(1);

            It should_have_updated_decimal_member_to_null = () =>
            {
                var refetched = TestContext.Database.UseOnceTo().Query<SingleDecimalMember>().FirstOrDefault();
                refetched.ShouldNotBeNull();
                refetched.Value.ShouldBeNull();
            };

            private static SingleDecimalMember _item;
        }

        [Subject(typeof(ISession), "Update (when single double member")]
        public class when_updating_item_with_single_double_member_to_null : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _item = new SingleDoubleMember { Value = 42.33 };
                TestContext.Database.UseOnceTo().Insert(_item);
            };

            Because of = () =>
                TestContext.Database.UseOnceTo().Update<SingleDoubleMember>(_item.StructureId, i => i.Value = null);

            It should_have_one_item_stored =
                () => TestContext.Database.should_have_X_num_of_items<SingleDoubleMember>(1);

            It should_have_updated_double_member_to_null = () =>
            {
                var refetched = TestContext.Database.UseOnceTo().Query<SingleDoubleMember>().FirstOrDefault();
                refetched.ShouldNotBeNull();
                refetched.Value.ShouldBeNull();
            };

            private static SingleDoubleMember _item;
        }

        [Subject(typeof(ISession), "Update (when single nested array element of string")]
        public class when_updating_item_with_array_of_single_string_member_with_member_having_null : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _item = new Root();
                TestContext.Database.UseOnceTo().Insert(_item);
            };

            Because of = () =>
                TestContext.Database.UseOnceTo().Update<Root>(_item.StructureId, i => i.StringElements.Add(new StringElement { Value = null }));

            It should_have_one_item_stored =
                () => TestContext.Database.should_have_X_num_of_items<Root>(1);

            It should_have_nested_element_with_string_member_being_null = () =>
            {
                var refetched = TestContext.Database.UseOnceTo().Query<Root>().FirstOrDefault();
                refetched.ShouldNotBeNull();
                refetched.StringElements.Count.ShouldEqual(1);
                refetched.StringElements.First().Value.ShouldBeNull();
            };

            private static Root _item;
        }

        [Subject(typeof(ISession), "Update (when single nested array element of datetime")]
        public class when_updating_item_with_array_of_single_datetime_member_with_member_having_null : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _item = new Root();
                TestContext.Database.UseOnceTo().Insert(_item);
            };

            Because of = () =>
                TestContext.Database.UseOnceTo().Update<Root>(_item.StructureId, i => i.DateElements.Add(new DateTimeElement { Value = null }));

            It should_have_one_item_stored =
                () => TestContext.Database.should_have_X_num_of_items<Root>(1);

            It should_have_nested_element_with_datetime_member_being_null = () =>
            {
                var refetched = TestContext.Database.UseOnceTo().Query<Root>().FirstOrDefault();
                refetched.ShouldNotBeNull();
                refetched.DateElements.Count.ShouldEqual(1);
                refetched.DateElements.First().Value.ShouldBeNull();
            };

            private static Root _item;
        }

        [Subject(typeof(ISession), "Update (when single nested array element of int")]
        public class when_updating_item_with_array_of_single_int_member_with_member_having_null : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _item = new Root();
                TestContext.Database.UseOnceTo().Insert(_item);
            };

            Because of = () =>
                TestContext.Database.UseOnceTo().Update<Root>(_item.StructureId, i => i.IntElements.Add(new IntElement { Value = null }));

            It should_have_one_item_stored =
                () => TestContext.Database.should_have_X_num_of_items<Root>(1);

            It should_have_nested_element_with_int_member_being_null = () =>
            {
                var refetched = TestContext.Database.UseOnceTo().Query<Root>().FirstOrDefault();
                refetched.ShouldNotBeNull();
                refetched.IntElements.Count.ShouldEqual(1);
                refetched.IntElements.First().Value.ShouldBeNull();
            };

            private static Root _item;
        }

        [Subject(typeof(ISession), "Update (when single nested array element of long")]
        public class when_updating_item_with_array_of_single_long_member_with_member_having_null : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _item = new Root();
                TestContext.Database.UseOnceTo().Insert(_item);
            };

            Because of = () =>
                TestContext.Database.UseOnceTo().Update<Root>(_item.StructureId, i => i.LongElements.Add(new LongElement { Value = null }));

            It should_have_one_item_stored =
                () => TestContext.Database.should_have_X_num_of_items<Root>(1);

            It should_have_nested_element_with_long_member_being_null = () =>
            {
                var refetched = TestContext.Database.UseOnceTo().Query<Root>().FirstOrDefault();
                refetched.ShouldNotBeNull();
                refetched.LongElements.Count.ShouldEqual(1);
                refetched.LongElements.First().Value.ShouldBeNull();
            };

            private static Root _item;
        }

        [Subject(typeof(ISession), "Update (when single nested array element of bool")]
        public class when_updating_item_with_array_of_single_bool_member_with_member_having_null : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _item = new Root();
                TestContext.Database.UseOnceTo().Insert(_item);
            };

            Because of = () =>
                TestContext.Database.UseOnceTo().Update<Root>(_item.StructureId, i => i.BoolElements.Add(new BoolElement { Value = null }));

            It should_have_one_item_stored =
                () => TestContext.Database.should_have_X_num_of_items<Root>(1);

            It should_have_nested_element_with_bool_member_being_null = () =>
            {
                var refetched = TestContext.Database.UseOnceTo().Query<Root>().FirstOrDefault();
                refetched.ShouldNotBeNull();
                refetched.BoolElements.Count.ShouldEqual(1);
                refetched.BoolElements.First().Value.ShouldBeNull();
            };

            private static Root _item;
        }

        [Subject(typeof(ISession), "Update (when single nested array element of decimal")]
        public class when_updating_item_with_array_of_single_decimal_member_with_member_having_null : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _item = new Root();
                TestContext.Database.UseOnceTo().Insert(_item);
            };

            Because of = () =>
                TestContext.Database.UseOnceTo().Update<Root>(_item.StructureId, i => i.DecimalElements.Add(new DecimalElement { Value = null }));

            It should_have_one_item_stored =
                () => TestContext.Database.should_have_X_num_of_items<Root>(1);

            It should_have_nested_element_with_decimal_member_being_null = () =>
            {
                var refetched = TestContext.Database.UseOnceTo().Query<Root>().FirstOrDefault();
                refetched.ShouldNotBeNull();
                refetched.DecimalElements.Count.ShouldEqual(1);
                refetched.DecimalElements.First().Value.ShouldBeNull();
            };

            private static Root _item;
        }

        [Subject(typeof(ISession), "Update (when single nested array element of double")]
        public class when_updating_item_with_array_of_single_double_member_with_member_having_null : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _item = new Root();
                TestContext.Database.UseOnceTo().Insert(_item);
            };

            Because of = () =>
                TestContext.Database.UseOnceTo().Update<Root>(_item.StructureId, i => i.DoubleElements.Add(new DoubleElement { Value = null }));

            It should_have_one_item_stored =
                () => TestContext.Database.should_have_X_num_of_items<Root>(1);

            It should_have_nested_element_with_double_member_being_null = () =>
            {
                var refetched = TestContext.Database.UseOnceTo().Query<Root>().FirstOrDefault();
                refetched.ShouldNotBeNull();
                refetched.DoubleElements.Count.ShouldEqual(1);
                refetched.DoubleElements.First().Value.ShouldBeNull();
            };

            private static Root _item;
        }

        private class Root
        {
            public Guid StructureId { get; set; }
            public List<StringElement> StringElements { get; set; }
            public List<TextElement> TextElements { get; set; }
            public List<DateTimeElement> DateElements { get; set; }
            public List<IntElement> IntElements { get; set; }
            public List<LongElement> LongElements { get; set; }
            public List<BoolElement> BoolElements { get; set; }
            public List<DecimalElement> DecimalElements { get; set; }
            public List<DoubleElement> DoubleElements { get; set; }

            public Root()
            {
                StringElements = new List<StringElement>();
                TextElements = new List<TextElement>();
                DateElements = new List<DateTimeElement>();
                IntElements = new List<IntElement>();
                LongElements = new List<LongElement>();
                BoolElements = new List<BoolElement>();
                DecimalElements = new List<DecimalElement>();
                DoubleElements = new List<DoubleElement>();
            }
        }

        private class StringElement
        {
            public string Value { get; set; }
        }

        private class TextElement
        {
            public string Text { get; set; }
        }

        private class DateTimeElement
        {
            public DateTime? Value { get; set; }
        }

        private class IntElement
        {
            public int? Value { get; set; }
        }

        private class LongElement
        {
            public long? Value { get; set; }
        }

        private class BoolElement
        {
            public bool? Value { get; set; }
        }

        private class DecimalElement
        {
            public decimal? Value { get; set; }
        }

        private class DoubleElement
        {
            public double? Value { get; set; }
        }

        private class SingleStringMember
        {
            public Guid StructureId { get; set; }
            public string Value { get; set; }
        }

        private class SingleDateTimeMember
        {
            public Guid StructureId { get; set; }
            public DateTime? Value { get; set; }
        }

        private class SingleIntMember
        {
            public Guid StructureId { get; set; }
            public int? Value { get; set; }
        }

        private class SingleLongMember
        {
            public Guid StructureId { get; set; }
            public long? Value { get; set; }
        }

        private class SingleBoolMember
        {
            public Guid StructureId { get; set; }
            public bool? Value { get; set; }
        }

        private class SingleDecimalMember
        {
            public Guid StructureId { get; set; }
            public decimal? Value { get; set; }
        }

        private class SingleDoubleMember
        {
            public Guid StructureId { get; set; }
            public double? Value { get; set; }
        }
    }
}