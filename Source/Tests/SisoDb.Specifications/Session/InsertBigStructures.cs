using System;
using Machine.Specifications;
using SisoDb.Resources;
using SisoDb.Testing;

namespace SisoDb.Specifications.Session
{
    class InsertBigStructures
    {
        [Subject(typeof(ISession), "Insert")]
        public class when_inserting_batch_of_items_with_single_string_index_of_4001_characters : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = new[]
                {
                    new Model { LongText = new string('a', 4001) }, 
                    new Model { LongText = new string('b', 4001) }, 
                    new Model { LongText = new string('c', 4001) }
                };
            };

#if Sql2008Provider || Sql2012Provider || SqlProfilerProvider
            Because of = () => 
                TestContext.Database.UseOnceTo().InsertMany(_structures);

            It should_have_inserted_items = () =>
            {
                var refetched = TestContext.Database.UseOnceTo().Query<Model>().ToArray();
                refetched.Length.ShouldEqual(3);

                refetched[0].LongText.ShouldEqual(new string('a', 4001));
                refetched[1].LongText.ShouldEqual(new string('b', 4001));
                refetched[2].LongText.ShouldEqual(new string('c', 4001));
            };
#endif

#if SqlCe4Provider
            Because of = () =>
                CaughtException = Catch.Exception(() => TestContext.Database.UseOnceTo().InsertMany(_structures));

            It should_have_thrown_an_exception = () =>
            {
                CaughtException.ShouldNotBeNull();
                CaughtException.Message.ShouldStartWith("String truncation: max=4000");
            };
#endif
            private static Model[] _structures;
        }

        [Subject(typeof(ISession), "Insert")]
        public class when_inserting_single_item_with_single_string_index_of_4001_characters : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
            };

#if Sql2008Provider || Sql2012Provider || SqlProfilerProvider
            Because of = () =>
                TestContext.Database.UseOnceTo().Insert(new Model { LongText = new string('a', 4001) });

            It should_have_one_item_inserted = () =>
            {
                var refetched = TestContext.Database.UseOnceTo().Query<Model>().FirstOrDefault();
                refetched.ShouldNotBeNull();
                refetched.LongText.ShouldEqual(new string('a', 4001));
            };
#endif

#if SqlCe4Provider
            Because of = () =>
                CaughtException = Catch.Exception(() => TestContext.Database.UseOnceTo().Insert(new Model { LongText = new string('a', 4001) }));

            It should_have_thrown_an_exception = () =>
            {
                CaughtException.ShouldNotBeNull();
                CaughtException.Message.ShouldEqual(ExceptionMessages.SqlCe4_ToLongIndividualStringValue);
            };
#endif
        }

        private class Model
        {
            public Guid Id { get; set; }
            public string LongText { get; set; }
        }
    }
}