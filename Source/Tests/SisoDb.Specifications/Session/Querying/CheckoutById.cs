using System;
using System.Collections.Generic;
using Machine.Specifications;
using SisoDb.Specifications.Model;
using SisoDb.Testing;

namespace SisoDb.Specifications.Session.Querying
{
    class CheckoutById
    {
        [Subject(typeof(ISession), "Checkout by Id (guid)")]
        public class when_getting_one_of_four_guid_items : SpecificationBase
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
                    _fetchedStructure = session.CheckoutById<QueryGuidItem>(_structures[1].StructureId);
                }
            };

            It should_fetch_the_structure =
                () => _fetchedStructure.ShouldBeValueEqualTo(_structures[1]);

            private static IList<QueryGuidItem> _structures;
            private static QueryGuidItem _fetchedStructure;
        }

        [Subject(typeof(ISession), "Checkout by Id (guid)")]
        public class when_getting_by_id_and_other_session_allready_has_checked_out_the_guid_item : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryGuidItem.CreateFourItems<QueryGuidItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);

                _firstSession = TestContext.Database.BeginSession();
                _fetchedStructure = _firstSession.CheckoutById<QueryGuidItem>(_structures[1].StructureId);
            };

            Cleanup after = () =>
            {
                if (_firstSession != null)
                {
                    _firstSession.Dispose();
                    _firstSession = null;
                }
            };

            Because of = () =>
            {
                CaughtException = Catch.Exception(() =>
                {
                    using (var session = TestContext.Database.BeginSession())
                    {
                        _fetchedStructure = session.GetById<QueryGuidItem>(_structures[1].StructureId);
                    }
                });
            };

#if !SqlCe4Provider
            It should_have_timed_out_trying_to_get_the_structure =
                () => CaughtException.ShouldHaveTimedOut();
#endif
#if SqlCe4Provider
            It should_not_have_timed_out_trying_to_get_the_structure =
                () => CaughtException.ShouldBeNull();
#endif
            private static ISession _firstSession;
            private static IList<QueryGuidItem> _structures;
            private static QueryGuidItem _fetchedStructure;
        }

        [Subject(typeof(ISession), "Checkout by Id (guid)")]
        public class when_other_session_allready_has_checked_out_the_guid_item : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryGuidItem.CreateFourItems<QueryGuidItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);

                _firstSession = TestContext.Database.BeginSession();
                _fetchedStructure = _firstSession.CheckoutById<QueryGuidItem>(_structures[1].StructureId);
            };

            Cleanup after = () =>
            {
                if (_firstSession != null)
                {
                    _firstSession.Dispose();
                    _firstSession = null;
                }
            };

            Because of = () =>
            {
                CaughtException = Catch.Exception(() =>
                {
                    using (var session = TestContext.Database.BeginSession())
                    {
                        _fetchedStructure = session.CheckoutById<QueryGuidItem>(_structures[1].StructureId);
                    }
                });
            };

            It should_have_timed_out_trying_to_get_the_structure =
                () => CaughtException.ShouldHaveTimedOut();

            private static ISession _firstSession;
            private static IList<QueryGuidItem> _structures;
            private static QueryGuidItem _fetchedStructure;
        }

        [Subject(typeof(ISession), "Checkout by Id (identity)")]
        public class when_getting_one_of_four_identity_items : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryIdentityItem.CreateFourItems<QueryIdentityItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    _fetchedStructure = session.CheckoutById<QueryIdentityItem>(_structures[1].StructureId);
                }
            };

            It should_fetch_the_structure =
                () => _fetchedStructure.ShouldBeValueEqualTo(_structures[1]);

            private static IList<QueryIdentityItem> _structures;
            private static QueryIdentityItem _fetchedStructure;
        }

        [Subject(typeof(ISession), "Checkout by Id (identity)")]
        public class when_getting_by_id_and_other_session_allready_has_checked_out_the_identity_item : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryIdentityItem.CreateFourItems<QueryIdentityItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);

                _firstSession = TestContext.Database.BeginSession();
                _fetchedStructure = _firstSession.CheckoutById<QueryIdentityItem>(_structures[1].StructureId);
            };

            Cleanup after = () =>
            {
                if (_firstSession != null)
                {
                    _firstSession.Dispose();
                    _firstSession = null;
                }
            };

            Because of = () =>
            {
                CaughtException = Catch.Exception(() =>
                {
                    using (var session = TestContext.Database.BeginSession())
                    {
                        _fetchedStructure = session.GetById<QueryIdentityItem>(_structures[1].StructureId);
                    }
                });
            };

#if !SqlCe4Provider
            It should_have_timed_out_trying_to_get_the_structure =
                () => CaughtException.ShouldHaveTimedOut();
#endif
#if SqlCe4Provider
            It should_not_have_timed_out_trying_to_get_the_structure =
                () => CaughtException.ShouldBeNull();
#endif

            private static ISession _firstSession;
            private static IList<QueryIdentityItem> _structures;
            private static QueryIdentityItem _fetchedStructure;
        }

        [Subject(typeof(ISession), "Checkout by Id (identity)")]
        public class when_other_session_allready_has_checked_out_the_identity_item : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryIdentityItem.CreateFourItems<QueryIdentityItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);

                _firstSession = TestContext.Database.BeginSession();
                _fetchedStructure = _firstSession.CheckoutById<QueryIdentityItem>(_structures[1].StructureId);
            };

            Cleanup after = () =>
            {
                if (_firstSession != null)
                {
                    _firstSession.Dispose();
                    _firstSession = null;
                }
            };

            Because of = () =>
            {
                CaughtException = Catch.Exception(() =>
                {
                    using (var session = TestContext.Database.BeginSession())
                    {
                        _fetchedStructure = session.CheckoutById<QueryIdentityItem>(_structures[1].StructureId);
                    }
                });
            };

            It should_have_timed_out_trying_to_get_the_structure =
                () => CaughtException.ShouldHaveTimedOut();

            private static ISession _firstSession;
            private static IList<QueryIdentityItem> _structures;
            private static QueryIdentityItem _fetchedStructure;
        }

        [Subject(typeof(ISession), "Checkout by Id (big identity)")]
        public class when_getting_one_of_four_big_identity_items : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryBigIdentityItem.CreateFourItems<QueryBigIdentityItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    _fetchedStructure = session.CheckoutById<QueryBigIdentityItem>(_structures[1].StructureId);
                }
            };

            It should_fetch_the_structure =
                () => _fetchedStructure.ShouldBeValueEqualTo(_structures[1]);

            private static IList<QueryBigIdentityItem> _structures;
            private static QueryBigIdentityItem _fetchedStructure;
        }

        [Subject(typeof(ISession), "Checkout by Id (big identity)")]
        public class when_getting_by_id_and_other_session_allready_has_checked_out_the_big_identity_item : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryBigIdentityItem.CreateFourItems<QueryBigIdentityItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);

                _firstSession = TestContext.Database.BeginSession();
                _fetchedStructure = _firstSession.CheckoutById<QueryBigIdentityItem>(_structures[1].StructureId);
            };

            Cleanup after = () =>
            {
                if (_firstSession != null)
                {
                    _firstSession.Dispose();
                    _firstSession = null;
                }
            };

            Because of = () =>
            {
                CaughtException = Catch.Exception(() =>
                {
                    using (var session = TestContext.Database.BeginSession())
                    {
                        _fetchedStructure = session.GetById<QueryBigIdentityItem>(_structures[1].StructureId);
                    }
                });
            };

#if !SqlCe4Provider
            It should_have_timed_out_trying_to_get_the_structure =
                () => CaughtException.ShouldHaveTimedOut();
#endif
#if SqlCe4Provider
            It should_not_have_timed_out_trying_to_get_the_structure =
                () => CaughtException.ShouldBeNull();
#endif

            private static ISession _firstSession;
            private static IList<QueryBigIdentityItem> _structures;
            private static QueryBigIdentityItem _fetchedStructure;
        }

        [Subject(typeof(ISession), "Checkout by Id (big identity)")]
        public class when_other_session_allready_has_checked_out_the_big_identity_item : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = QueryBigIdentityItem.CreateFourItems<QueryBigIdentityItem>();
                TestContext.Database.UseOnceTo().InsertMany(_structures);

                _firstSession = TestContext.Database.BeginSession();
                _fetchedStructure = _firstSession.CheckoutById<QueryBigIdentityItem>(_structures[1].StructureId);
            };

            Cleanup after = () =>
            {
                if (_firstSession != null)
                {
                    _firstSession.Dispose();
                    _firstSession = null;
                }
            };

            Because of = () =>
            {
                CaughtException = Catch.Exception(() =>
                {
                    using (var session = TestContext.Database.BeginSession())
                    {
                        _fetchedStructure = session.CheckoutById<QueryBigIdentityItem>(_structures[1].StructureId);
                    }
                });
            };

            It should_have_timed_out_trying_to_get_the_structure =
                () => CaughtException.ShouldHaveTimedOut();

            private static ISession _firstSession;
            private static IList<QueryBigIdentityItem> _structures;
            private static QueryBigIdentityItem _fetchedStructure;
        }

        private static void EnsureTimedOut(Exception ex)
        {
            (ex.Message.Contains("Timeout") || ex.Message.Contains("Timed out")).ShouldBeTrue();
        }
    }
}
