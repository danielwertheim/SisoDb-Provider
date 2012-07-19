using System;
using System.Collections.Generic;
using Machine.Specifications;
using SisoDb.NCore;
using SisoDb.Resources;
using SisoDb.Testing;

namespace SisoDb.Specifications.Session.Querying.StringFunctions
{
    class QxIsExactly
    {
        [Subject(typeof(ISession), "QxIsExactly")]
        public class when_applied_on_string_member_value_that_has_different_value_ : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = new[] { new MyClass { MyString = "ABC" }, new MyClass { MyString = "DEF" } };
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () => _fetchedStructure = TestContext.Database.UseOnceTo()
                .Query<MyClass>()
                .Where(i => i.MyString.QxIsExactly("GHI"))
                .SingleOrDefault();

            It should_not_have_fetched_any_structure = () => _fetchedStructure.ShouldBeNull();

            private static IList<MyClass> _structures;
            private static MyClass _fetchedStructure;
        }

        [Subject(typeof(ISession), "QxIsExactly")]
        public class when_applied_on_string_member_value_that_has_different_casing_ : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = new[] { new MyClass { MyString = "ABC" }, new MyClass { MyString = "DEF" } };
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () => _fetchedStructure = TestContext.Database.UseOnceTo()
                .Query<MyClass>()
                .Where(i => i.MyString.QxIsExactly("def"))
                .SingleOrDefault();

            It should_not_have_fetched_any_structure = () => _fetchedStructure.ShouldBeNull();

            private static IList<MyClass> _structures;
            private static MyClass _fetchedStructure;
        }

        [Subject(typeof(ISession), "QxIsExactly")]
        public class when_applied_on_string_member_value_that_has_same_casing_but_different_length_ : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = new[] { new MyClass { MyString = "ABC" }, new MyClass { MyString = "DEF" } };
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () => _fetchedStructure = TestContext.Database.UseOnceTo()
                .Query<MyClass>()
                .Where(i => i.MyString.QxIsExactly("DEFG"))
                .SingleOrDefault();

            It should_not_have_fetched_any_structure = () => _fetchedStructure.ShouldBeNull();

            private static IList<MyClass> _structures;
            private static MyClass _fetchedStructure;
        }

        [Subject(typeof(ISession), "QxIsExactly")]
        public class when_applied_on_string_member_value_that_has_same_casing_ : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = new[] { new MyClass { MyString = "ABC" }, new MyClass { MyString = "def" } };
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () => _fetchedStructure = TestContext.Database.UseOnceTo()
                .Query<MyClass>()
                .Where(i => i.MyString.QxIsExactly("def"))
                .SingleOrDefault();

            It should_have_fetched_the_matching_structure = () =>
            {
                _fetchedStructure.ShouldNotBeNull();
                _fetchedStructure.ShouldBeValueEqualTo(_structures[1]);
            };

            private static IList<MyClass> _structures;
            private static MyClass _fetchedStructure;
        }
        
        [Subject(typeof(ISession), "QxIsExactly")]
        public class when_applied_on_text_member_ : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = new[] { new MyClass { MyText = "ABC" }, new MyClass { MyText = "def" } };
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () => CaughtException = Catch.Exception(() => 
                _fetchedStructure = TestContext.Database.UseOnceTo()
                    .Query<MyClass>()
                    .Where(i => i.MyText.QxIsExactly("def"))
                    .SingleOrDefault());

            It should_have_thrown_not_supported_exception = () =>
            {
                CaughtException.ShouldNotBeNull();
                CaughtException.Message.ShouldEqual(ExceptionMessages.QxIsExactly_NotSupportedForTexts.Inject("MyText"));
            };

            It should_not_have_returned_any_structure = () => _fetchedStructure.ShouldBeNull();

            private static IList<MyClass> _structures;
            private static MyClass _fetchedStructure;
        }

        private class MyClass
        {
            public Guid Id { get; set; }
            public string MyString { get; set; }
            public string MyText { get; set; }
        }
    }
}