using System;
using System.Linq;
using System.Runtime.Caching;
using Machine.Specifications;
using SisoDb.MsMemoryCache;
using SisoDb.Testing;

namespace SisoDb.Specifications.Session
{
    class Caching
    {
        [Subject(typeof(ISession), "MsMemoryCache")]
        public class when_caching_is_enabled_and_item_exists_in_cache : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();

                _cacheContainer = new MemoryCache(typeof(IAmCached).Name);
                TestContext.Database.CacheProvider = new MsMemCacheProvider(t => _cacheContainer);
                TestContext.Database.CacheProvider.EnableFor(typeof(IAmCached));
                TestContext.Database.UseOnceTo().Insert(_originalStructure = new IAmCached { Value = 1 });
                TestContext.Database.UseOnceTo().GetById<IAmCached>(_originalStructure.Id); //Touch in other session to get it in cache
            };

            Because of = () =>
            {
                _itemExists = TestContext.Database.UseOnceTo().Query<IAmCached>().Exists(_originalStructure.Id);
            };

            It should_have_one_item_in_cache =
                () => _cacheContainer.GetCount().ShouldEqual(1);

            It should_have_one_the_item_in_cache =
                () => (_cacheContainer.First().Value as IAmCached).ShouldBeValueEqualTo(_originalStructure);

            It should_exist =
                () => _itemExists.ShouldBeTrue();
            
            private static MemoryCache _cacheContainer;
            private static IAmCached _originalStructure;
            private static bool _itemExists;
        }

        [Subject(typeof(ISession), "MsMemoryCache")]
        public class when_caching_is_enabled_and_item_does_not_exists_in_cache : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();

                _cacheContainer = new MemoryCache(typeof(IAmCached).Name);
                TestContext.Database.CacheProvider = new MsMemCacheProvider(t => _cacheContainer);
                TestContext.Database.CacheProvider.EnableFor(typeof(IAmCached));
                TestContext.Database.UseOnceTo().Insert(_originalStructure = new IAmCached { Value = 1 });
            };

            Because of = () =>
            {
                _itemExists = TestContext.Database.UseOnceTo().Query<IAmCached>().Exists(_originalStructure.Id);
            };

            It should_not_have_anything_in_cache =
                () => _cacheContainer.GetCount().ShouldEqual(0);

            It should_exist =
                () => _itemExists.ShouldBeTrue();

            private static MemoryCache _cacheContainer;
            private static IAmCached _originalStructure;
            private static bool _itemExists;
        }

        [Subject(typeof(ISession), "MsMemoryCache")]
        public class when_caching_is_enabled_and_nothing_is_stored_and_getbyid_is_called : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();

                _cacheContainer = new MemoryCache(typeof(IAmCached).Name);
                TestContext.Database.CacheProvider = new MsMemCacheProvider(t => _cacheContainer);
                TestContext.Database.CacheProvider.EnableFor(typeof(IAmCached));
            };

            Because of = () =>
            {
                _resultingStructure = TestContext.Database.UseOnceTo().GetById<IAmCached>(Guid.Parse("0D091F0B-63C1-4DFD-841C-36495DD76424"));
            };

            It should_have_returned_null =
                () => _resultingStructure.ShouldBeNull();

            It should_not_have_put_anything_in_cache =
                () => _cacheContainer.GetCount().ShouldEqual(0);

            private static MemoryCache _cacheContainer;
            private static IAmCached _resultingStructure;
        }

        [Subject(typeof(ISession), "MsMemoryCache")]
        public class when_caching_is_enabled_and_nothing_is_stored_and_getbyids_is_called : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();

                _cacheContainer = new MemoryCache(typeof(IAmCached).Name);
                TestContext.Database.CacheProvider = new MsMemCacheProvider(t => _cacheContainer);
                TestContext.Database.CacheProvider.EnableFor(typeof(IAmCached));
            };

            Because of = () =>
            {
                _resultingStructures = TestContext.Database.UseOnceTo().GetByIds<IAmCached>(new[] { Guid.Parse("0D091F0B-63C1-4DFD-841C-36495DD76424") });
            };

            It should_have_returned_empty_array =
                () => _resultingStructures.Any().ShouldBeFalse();

            It should_not_have_put_anything_in_cache =
                () => _cacheContainer.GetCount().ShouldEqual(0);

            private static MemoryCache _cacheContainer;
            private static IAmCached[] _resultingStructures;
        }

        [Subject(typeof(ISession), "MsMemoryCache")]
        public class when_caching_is_enabled_and_contains_two_items_and_getbyid_is_called : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();

                _cacheContainer = new MemoryCache(typeof(IAmCached).Name);
                TestContext.Database.CacheProvider = new MsMemCacheProvider(t => _cacheContainer);
                TestContext.Database.CacheProvider.EnableFor(typeof(IAmCached));

                _originalStructures = new[] { new IAmCached { Value = 1 }, new IAmCached { Value = 2 } };
                TestContext.Database.UseOnceTo().InsertMany(_originalStructures);
            };

            Because of = () =>
            {
                _resultingStructure = TestContext.Database.UseOnceTo().GetById<IAmCached>(_originalStructures[1].Id);
            };

            It should_have_returned_non_null_structure =
                () => _resultingStructure.ShouldNotBeNull();

            It should_have_exactly_one_item_in_cache =
                () => _cacheContainer.GetCount().ShouldEqual(1);

            It should_have_put__returnet_structure_in_cache =
                () =>
                {
                    var cachedStructure = (IAmCached)_cacheContainer.First().Value;
                    cachedStructure.ShouldBeValueEqualTo(_resultingStructure);
                    cachedStructure.ShouldBeValueEqualTo(_originalStructures[1]);
                };

            private static MemoryCache _cacheContainer;
            private static IAmCached[] _originalStructures;
            private static IAmCached _resultingStructure;
        }

        [Subject(typeof(ISession), "MsMemoryCache")]
        public class when_caching_is_enabled_and_contains_two_items_and_getbyids_is_called : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();

                _cacheContainer = new MemoryCache(typeof(IAmCached).Name);
                TestContext.Database.CacheProvider = new MsMemCacheProvider(t => _cacheContainer);
                TestContext.Database.CacheProvider.EnableFor(typeof(IAmCached));

                _originalStructures = new[] { new IAmCached { Value = 1 }, new IAmCached { Value = 2 } };
                TestContext.Database.UseOnceTo().InsertMany(_originalStructures);
            };

            Because of = () =>
            {
                _resultingStructures = TestContext.Database.UseOnceTo().GetByIds<IAmCached>(_originalStructures[0].Id, _originalStructures[1].Id);
            };

            It should_have_returned_two_items =
                () => _resultingStructures.Length.ShouldEqual(2);

            It should_have_returned_two_non_null_items = 
                () => _resultingStructures.Count(s => s != null).ShouldEqual(2);

            It should_have_exactly_one_item_in_cache =
                () => _cacheContainer.GetCount().ShouldEqual(2);

            It should_have_put_structures_in_cache =
                () =>
                {
                    var cachedStructures = _cacheContainer.Select(kv => kv.Value).Cast<IAmCached>().OrderBy(s => s.Id).ToArray();
                    cachedStructures.ShouldBeValueEqualTo(_resultingStructures);
                    cachedStructures.ShouldBeValueEqualTo(_originalStructures);
                };

            private static MemoryCache _cacheContainer;
            private static IAmCached[] _originalStructures;
            private static IAmCached[] _resultingStructures;
        }

        [Subject(typeof(ISession), "MsMemoryCache")]
        public class when_caching_is_enabled_and_one_of_two_is_deleted_and_getbyids_is_called_in_different_session : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();

                _cacheContainer = new MemoryCache(typeof(IAmCached).Name);
                TestContext.Database.CacheProvider = new MsMemCacheProvider(t => _cacheContainer);
                TestContext.Database.CacheProvider.EnableFor(typeof(IAmCached));

                _originalStructures = new[] { new IAmCached { Value = 1 }, new IAmCached { Value = 2 } };
                TestContext.Database.UseOnceTo().InsertMany(_originalStructures);
            };

            Because of = () =>
            {
                TestContext.Database.UseOnceTo().DeleteById<IAmCached>(_originalStructures[0].Id);
                _resultingStructures = TestContext.Database.UseOnceTo().GetByIds<IAmCached>(_originalStructures[0].Id, _originalStructures[1].Id);
            };

            It should_have_returned_one_item =
                () => _resultingStructures.Length.ShouldEqual(1);

            It should_have_returned_one_non_null_item =
                () => _resultingStructures.FirstOrDefault().ShouldNotBeNull();

            It should_have_exactly_one_item_in_cache =
                () => _cacheContainer.GetCount().ShouldEqual(1);

            It should_have_correct_item_left_in_cache =
                () =>
                {
                    var cachedStructure = (IAmCached)_cacheContainer.First().Value;
                    cachedStructure.ShouldBeValueEqualTo(_resultingStructures[0]);
                    cachedStructure.ShouldBeValueEqualTo(_originalStructures[1]);
                };

            private static MemoryCache _cacheContainer;
            private static IAmCached[] _originalStructures;
            private static IAmCached[] _resultingStructures;
        }

        [Subject(typeof(ISession), "MsMemoryCache")]
        public class when_caching_is_enabled_and_one_of_two_is_deleted_and_getbyids_is_called_in_same_session : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();

                _cacheContainer = new MemoryCache(typeof(IAmCached).Name);
                TestContext.Database.CacheProvider = new MsMemCacheProvider(t => _cacheContainer);
                TestContext.Database.CacheProvider.EnableFor(typeof(IAmCached));

                _originalStructures = new[] { new IAmCached { Value = 1 }, new IAmCached { Value = 2 } };
                TestContext.Database.UseOnceTo().InsertMany(_originalStructures);
            };

            Because of = () =>
            {
                using(var session = TestContext.Database.BeginSession())
                {
                    session.DeleteById<IAmCached>(_originalStructures[0].Id);
                    _resultingStructures = session.GetByIds<IAmCached>(_originalStructures[0].Id, _originalStructures[1].Id).ToArray();   
                }
            };

            It should_have_returned_one_item =
                () => _resultingStructures.Length.ShouldEqual(1);

            It should_have_returned_one_non_null_item =
                () => _resultingStructures.FirstOrDefault().ShouldNotBeNull();

            It should_have_no_items_in_cache =
                () => _cacheContainer.GetCount().ShouldEqual(0);

            private static MemoryCache _cacheContainer;
            private static IAmCached[] _originalStructures;
            private static IAmCached[] _resultingStructures;
        }

        [Subject(typeof(ISession), "MsMemoryCache")]
        public class when_caching_is_enabled_and_one_of_two_is_deleted_and_getbyids_is_called_before_and_after_in_same_session : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();

                _cacheContainer = new MemoryCache(typeof(IAmCached).Name);
                TestContext.Database.CacheProvider = new MsMemCacheProvider(t => _cacheContainer);
                TestContext.Database.CacheProvider.EnableFor(typeof(IAmCached));

                _originalStructures = new[] { new IAmCached { Value = 1 }, new IAmCached { Value = 2 } };
                TestContext.Database.UseOnceTo().InsertMany(_originalStructures);
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    _resultingStructures = session.GetByIds<IAmCached>(_originalStructures[0].Id, _originalStructures[1].Id).ToArray();
                    session.DeleteById<IAmCached>(_originalStructures[0].Id);
                    _resultingStructures = session.GetByIds<IAmCached>(_originalStructures[0].Id, _originalStructures[1].Id).ToArray();
                }
            };

            It should_have_returned_one_item =
                () => _resultingStructures.Length.ShouldEqual(1);

            It should_have_returned_one_non_null_item =
                () => _resultingStructures.FirstOrDefault().ShouldNotBeNull();

            It should_have_exactly_one_item_in_cache =
                () => _cacheContainer.GetCount().ShouldEqual(1);

            It should_have_correct_item_left_in_cache =
                () =>
                {
                    var cachedStructure = (IAmCached)_cacheContainer.First().Value;
                    cachedStructure.ShouldBeValueEqualTo(_resultingStructures[0]);
                    cachedStructure.ShouldBeValueEqualTo(_originalStructures[1]);
                };

            private static MemoryCache _cacheContainer;
            private static IAmCached[] _originalStructures;
            private static IAmCached[] _resultingStructures;
        }

        [Subject(typeof(ISession), "MsMemoryCache")]
        public class when_caching_is_enabled_and_one_of_two_is_updated_and_getbyids_is_called_in_different_session : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();

                _cacheContainer = new MemoryCache(typeof(IAmCached).Name);
                TestContext.Database.CacheProvider = new MsMemCacheProvider(t => _cacheContainer);
                TestContext.Database.CacheProvider.EnableFor(typeof(IAmCached));

                _originalStructures = new[] { new IAmCached { Value = 1 }, new IAmCached { Value = 2 } };
                TestContext.Database.UseOnceTo().InsertMany(_originalStructures);
            };

            Because of = () =>
            {
                _originalStructures[0].Value = 142;
                TestContext.Database.UseOnceTo().Update(_originalStructures[0]);
                _resultingStructures = TestContext.Database.UseOnceTo().GetByIds<IAmCached>(_originalStructures[0].Id, _originalStructures[1].Id);
            };

            It should_have_returned_two_items =
                () => _resultingStructures.Length.ShouldEqual(2);

            It should_have_returned_two_non_null_item =
                () => _resultingStructures.Count(s => s != null).ShouldEqual(2);

            It should_have_exactly_two_items_in_cache =
                () => _cacheContainer.GetCount().ShouldEqual(2);

            It should_have_correct_item_left_in_cache =
                () =>
                {
                    var cachedStructures = _cacheContainer.Select(kv => kv.Value).Cast<IAmCached>().OrderBy(s => s.Id).ToArray();
                    cachedStructures.ShouldBeValueEqualTo(_resultingStructures);
                    cachedStructures.ShouldBeValueEqualTo(_originalStructures);
                };

            private static MemoryCache _cacheContainer;
            private static IAmCached[] _originalStructures;
            private static IAmCached[] _resultingStructures;
        }

        [Subject(typeof(ISession), "MsMemoryCache")]
        public class when_caching_is_enabled_and_one_of_two_is_updated_and_getbyids_is_called_before_and_after_in_same_session : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();

                _cacheContainer = new MemoryCache(typeof(IAmCached).Name);
                TestContext.Database.CacheProvider = new MsMemCacheProvider(t => _cacheContainer);
                TestContext.Database.CacheProvider.EnableFor(typeof(IAmCached));

                _originalStructures = new[] { new IAmCached { Value = 1 }, new IAmCached { Value = 2 } };
                TestContext.Database.UseOnceTo().InsertMany(_originalStructures);
            };

            Because of = () =>
            {
                _originalStructures[0].Value = 142;

                using(var session = TestContext.Database.BeginSession())
                {
                    _resultingStructures = session.GetByIds<IAmCached>(_originalStructures[0].Id, _originalStructures[1].Id).ToArray();   
                    session.Update(_originalStructures[0]);
                    _resultingStructures = session.GetByIds<IAmCached>(_originalStructures[0].Id, _originalStructures[1].Id).OrderBy(s => s.Id).ToArray();   
                }
            };

            It should_have_returned_two_items =
                () => _resultingStructures.Length.ShouldEqual(2);

            It should_have_returned_two_non_null_item =
                () => _resultingStructures.Count(s => s != null).ShouldEqual(2);

            It should_have_exactly_one_item_in_cache =
                () => _cacheContainer.GetCount().ShouldEqual(1);

            It should_have_correct_item_left_in_cache =
                () =>
                {
                    var cachedStructure = (IAmCached)_cacheContainer.First().Value;
                    cachedStructure.ShouldBeValueEqualTo(_resultingStructures[1]);
                    cachedStructure.ShouldBeValueEqualTo(_originalStructures[1]);
                };

            private static MemoryCache _cacheContainer;
            private static IAmCached[] _originalStructures;
            private static IAmCached[] _resultingStructures;
        }

        [Subject(typeof(ISession), "MsMemoryCache")]
        public class when_caching_is_enabled_and_two_original_items_exists_and_one_extra_is_inserted_and_getbyids_is_called_before_and_after_in_same_session : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();

                _cacheContainer = new MemoryCache(typeof(IAmCached).Name);
                TestContext.Database.CacheProvider = new MsMemCacheProvider(t => _cacheContainer);
                TestContext.Database.CacheProvider.EnableFor(typeof(IAmCached));

                _originalStructures = new[] { new IAmCached { Value = 1 }, new IAmCached { Value = 2 } };
                TestContext.Database.UseOnceTo().InsertMany(_originalStructures);
            };

            Because of = () =>
            {
                _originalStructures = new IAmCached[]{_originalStructures[0], _originalStructures[1], new IAmCached{Value = 3}};
                using (var session = TestContext.Database.BeginSession())
                {
                    _resultingStructures = session.GetByIds<IAmCached>(_originalStructures[0].Id, _originalStructures[1].Id).ToArray();
                    session.Insert(_originalStructures[2]);
                    _resultingStructures = session.GetByIds<IAmCached>(_originalStructures[0].Id, _originalStructures[1].Id, _originalStructures[2].Id).OrderBy(s => s.Id).ToArray();
                }
            };

            It should_have_returned_three_items =
                () => _resultingStructures.Length.ShouldEqual(3);

            It should_have_returned_three_non_null_item =
                () => _resultingStructures.Count(s => s != null).ShouldEqual(3);

            It should_have_exactly_two_items_in_cache =
                () => _cacheContainer.GetCount().ShouldEqual(2);

            It should_have_correct_item_left_in_cache =
                () =>
                {
                    var cachedStructures = _cacheContainer.Select(kv => kv.Value).Cast<IAmCached>().OrderBy(s => s.Id).ToArray();
                    cachedStructures.ShouldBeValueEqualTo(_resultingStructures.Take(2).ToArray());
                    cachedStructures.ShouldBeValueEqualTo(_originalStructures);
                };

            private static MemoryCache _cacheContainer;
            private static IAmCached[] _originalStructures;
            private static IAmCached[] _resultingStructures;
        }

        private class IAmCached
        {
            public Guid Id { get; set; }

            public int Value { get; set; }
        }
    }
}