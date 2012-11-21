using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Machine.Specifications;
using SisoDb.MsMemoryCache;
using SisoDb.Testing;

namespace SisoDb.Specifications.Session
{
    class Caching
    {
        [Subject(typeof(ISession), "MsMemoryCache, Exists")]
        public class when_caching_is_enabled_and_item_exists_in_cache : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                TestContext.Database.CacheProvider = new MsMemCacheProvider();
                TestContext.Database.CacheProvider.EnableFor(typeof(IAmCached));

                _cache = TestContext.Database.CacheProvider[typeof(IAmCached)];

                TestContext.Database.UseOnceTo().Insert(_originalStructure = new IAmCached { Value = 1 });
                TestContext.Database.UseOnceTo().GetById<IAmCached>(_originalStructure.Id); //Touch in other session to get it in cache
            };

            Because of = () =>
            {
                _itemExists = TestContext.Database.UseOnceTo().Query<IAmCached>().Exists(_originalStructure.Id);
            };

            It should_have_one_item_in_cache =
                () => _cache.Count().ShouldEqual(1);

            It should_have_one_the_item_in_cache =
                () => _cache.GetAll<IAmCached>().First().ShouldBeValueEqualTo(_originalStructure);

            It should_exist =
                () => _itemExists.ShouldBeTrue();

            private static ICache _cache;
            private static IAmCached _originalStructure;
            private static bool _itemExists;
        }

        [Subject(typeof(ISession), "MsMemoryCache, Exists")]
        public class when_caching_is_enabled_and_item_does_not_exists_in_cache : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                TestContext.Database.CacheProvider = new MsMemCacheProvider();
                TestContext.Database.CacheProvider.EnableFor(typeof(IAmCached));

                _cache = TestContext.Database.CacheProvider[typeof(IAmCached)];

                TestContext.Database.UseOnceTo().Insert(_originalStructure = new IAmCached { Value = 1 });
            };

            Because of = () =>
            {
                _itemExists = TestContext.Database.UseOnceTo().Query<IAmCached>().Exists(_originalStructure.Id);
            };

            It should_not_have_anything_in_cache =
                () => _cache.Count().ShouldEqual(0);

            It should_exist =
                () => _itemExists.ShouldBeTrue();

            private static ICache _cache;
            private static IAmCached _originalStructure;
            private static bool _itemExists;
        }

        [Subject(typeof(ISession), "MsMemoryCache, GetById")]
        public class when_caching_is_enabled_and_nothing_is_stored_and_getbyid_is_called : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                TestContext.Database.CacheProvider = new MsMemCacheProvider();
                TestContext.Database.CacheProvider.EnableFor(typeof(IAmCached));

                _cache = TestContext.Database.CacheProvider[typeof(IAmCached)];
            };

            Because of = () =>
            {
                _resultingStructure = TestContext.Database.UseOnceTo().GetById<IAmCached>(Guid.Parse("0D091F0B-63C1-4DFD-841C-36495DD76424"));
            };

            It should_have_returned_null =
                () => _resultingStructure.ShouldBeNull();

            It should_not_have_put_anything_in_cache =
                () => _cache.Count().ShouldEqual(0);

            private static ICache _cache;
            private static IAmCached _resultingStructure;
        }

        [Subject(typeof(ISession), "MsMemoryCache, GetByIds")]
        public class when_caching_is_enabled_and_nothing_is_stored_and_getbyids_is_called : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                TestContext.Database.CacheProvider = new MsMemCacheProvider();
                TestContext.Database.CacheProvider.EnableFor(typeof(IAmCached));

                _cache = TestContext.Database.CacheProvider[typeof(IAmCached)];
            };

            Because of = () =>
            {
                _resultingStructures = TestContext.Database.UseOnceTo().GetByIds<IAmCached>(new[] { Guid.Parse("0D091F0B-63C1-4DFD-841C-36495DD76424") });
            };

            It should_have_returned_empty_array =
                () => _resultingStructures.Any().ShouldBeFalse();

            It should_not_have_put_anything_in_cache =
                () => _cache.Count().ShouldEqual(0);

            private static ICache _cache;
            private static IAmCached[] _resultingStructures;
        }

        [Subject(typeof(ISession), "MsMemoryCache, GetById")]
        public class when_caching_is_enabled_and_contains_two_items_and_getbyid_is_called : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                TestContext.Database.CacheProvider = new MsMemCacheProvider();
                TestContext.Database.CacheProvider.EnableFor(typeof(IAmCached));

                _cache = TestContext.Database.CacheProvider[typeof(IAmCached)];

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
                () => _cache.Count().ShouldEqual(1);

            It should_have_put__returnet_structure_in_cache =
                () =>
                {
                    var cachedStructure = _cache.GetAll<IAmCached>().First();
                    cachedStructure.ShouldBeValueEqualTo(_resultingStructure);
                    cachedStructure.ShouldBeValueEqualTo(_originalStructures[1]);
                };

            private static ICache _cache;
            private static IAmCached[] _originalStructures;
            private static IAmCached _resultingStructure;
        }

        [Subject(typeof(ISession), "MsMemoryCache, GetByIds")]
        public class when_caching_is_enabled_and_contains_two_items_and_getbyids_is_called : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                TestContext.Database.CacheProvider = new MsMemCacheProvider();
                TestContext.Database.CacheProvider.EnableFor(typeof(IAmCached));

                _cache = TestContext.Database.CacheProvider[typeof(IAmCached)];

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
                () => _cache.Count().ShouldEqual(2);

            It should_have_put_structures_in_cache =
                () =>
                {
                    var cachedStructures = _cache.GetAll<IAmCached>().OrderBy(s => s.Id).ToArray();
                    cachedStructures.ShouldBeValueEqualTo(_resultingStructures);
                    cachedStructures.ShouldBeValueEqualTo(_originalStructures);
                };

            private static ICache _cache;
            private static IAmCached[] _originalStructures;
            private static IAmCached[] _resultingStructures;
        }

        [Subject(typeof(ISession), "MsMemoryCache, DeleteById")]
        public class when_caching_is_enabled_and_one_of_two_is_deleted_and_getbyids_is_called_in_different_session : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                TestContext.Database.CacheProvider = new MsMemCacheProvider();
                TestContext.Database.CacheProvider.EnableFor(typeof(IAmCached));

                _cache = TestContext.Database.CacheProvider[typeof(IAmCached)];

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
                () => _cache.Count().ShouldEqual(1);

            It should_have_correct_item_left_in_cache =
                () =>
                {
                    var cachedStructure = _cache.GetAll<IAmCached>().First();
                    cachedStructure.ShouldBeValueEqualTo(_resultingStructures[0]);
                    cachedStructure.ShouldBeValueEqualTo(_originalStructures[1]);
                };

            private static ICache _cache;
            private static IAmCached[] _originalStructures;
            private static IAmCached[] _resultingStructures;
        }

        [Subject(typeof(ISession), "MsMemoryCache, DeleteById")]
        public class when_caching_is_enabled_and_one_of_two_is_deleted_and_getbyids_is_called_in_same_session : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                TestContext.Database.CacheProvider = new MsMemCacheProvider();
                TestContext.Database.CacheProvider.EnableFor(typeof(IAmCached));

                _cache = TestContext.Database.CacheProvider[typeof(IAmCached)];

                _originalStructures = new[] { new IAmCached { Value = 1 }, new IAmCached { Value = 2 } };
                TestContext.Database.UseOnceTo().InsertMany(_originalStructures);
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
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
                () => _cache.Count().ShouldEqual(0);

            private static ICache _cache;
            private static IAmCached[] _originalStructures;
            private static IAmCached[] _resultingStructures;
        }

        [Subject(typeof(ISession), "MsMemoryCache, DeleteById")]
        public class when_caching_is_enabled_and_one_of_two_is_deleted_and_getbyids_is_called_before_and_after_in_same_session : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                TestContext.Database.CacheProvider = new MsMemCacheProvider();
                TestContext.Database.CacheProvider.EnableFor(typeof(IAmCached));

                _cache = TestContext.Database.CacheProvider[typeof(IAmCached)];

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

            It should_have_returned_two_items_since_it_pulls_one_in_from_db =
                () => _resultingStructures.Length.ShouldEqual(2);

            It should_have_returned_correct_items =
                () => _originalStructures.ShouldBeValueEqualTo(_resultingStructures);

            It should_have_exactly_one_item_in_cache =
                () => _cache.Count().ShouldEqual(1);

            It should_have_correct_item_left_in_cache = () =>
            {
                var cachedStructure = _cache.GetAll<IAmCached>().First();
                cachedStructure.ShouldBeValueEqualTo(_resultingStructures[0]);
                cachedStructure.ShouldBeValueEqualTo(_originalStructures[1]);
            };

            private static ICache _cache;
            private static IAmCached[] _originalStructures;
            private static IAmCached[] _resultingStructures;
        }

        [Subject(typeof(ISession), "MsMemoryCache, Update")]
        public class when_caching_is_enabled_and_one_of_two_is_updated_and_getbyids_is_called_in_different_session : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                TestContext.Database.CacheProvider = new MsMemCacheProvider();
                TestContext.Database.CacheProvider.EnableFor(typeof(IAmCached));

                _cache = TestContext.Database.CacheProvider[typeof(IAmCached)];

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
                () => _cache.Count().ShouldEqual(2);

            It should_have_correct_item_left_in_cache =
                () =>
                {
                    var cachedStructures = _cache.GetAll<IAmCached>().OrderBy(s => s.Id).ToArray();
                    cachedStructures.ShouldBeValueEqualTo(_resultingStructures);
                    cachedStructures.ShouldBeValueEqualTo(_originalStructures);
                };

            private static ICache _cache;
            private static IAmCached[] _originalStructures;
            private static IAmCached[] _resultingStructures;
        }

        [Subject(typeof(ISession), "MsMemoryCache, Update")]
        public class when_caching_is_enabled_and_one_of_two_is_updated_and_getbyids_is_called_before_and_after_in_same_session : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                TestContext.Database.CacheProvider = new MsMemCacheProvider();
                TestContext.Database.CacheProvider.EnableFor(typeof(IAmCached));

                _cache = TestContext.Database.CacheProvider[typeof(IAmCached)];

                _originalStructures = new[] { new IAmCached { Value = 1 }, new IAmCached { Value = 2 } };
                TestContext.Database.UseOnceTo().InsertMany(_originalStructures);
            };

            Because of = () =>
            {
                _originalStructures[0].Value = 142;

                using (var session = TestContext.Database.BeginSession())
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
                () => _cache.Count().ShouldEqual(1);

            It should_have_correct_item_left_in_cache =
                () =>
                {
                    var cachedStructure = _cache.GetAll<IAmCached>().First();
                    cachedStructure.ShouldBeValueEqualTo(_resultingStructures[1]);
                    cachedStructure.ShouldBeValueEqualTo(_originalStructures[1]);
                };

            private static ICache _cache;
            private static IAmCached[] _originalStructures;
            private static IAmCached[] _resultingStructures;
        }

        [Subject(typeof(ISession), "MsMemoryCache, Insert")]
        public class when_caching_is_enabled_and_two_original_items_exists_and_one_extra_is_inserted_and_getbyids_is_called_before_and_after_in_same_session : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                TestContext.Database.CacheProvider = new MsMemCacheProvider();
                TestContext.Database.CacheProvider.EnableFor(typeof(IAmCached));

                _cache = TestContext.Database.CacheProvider[typeof(IAmCached)];

                _originalStructures = new[] { new IAmCached { Value = 1 }, new IAmCached { Value = 2 } };
                TestContext.Database.UseOnceTo().InsertMany(_originalStructures);
            };

            Because of = () =>
            {
                _originalStructures = new IAmCached[] { _originalStructures[0], _originalStructures[1], new IAmCached { Value = 3 } };
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
                () => _cache.Count().ShouldEqual(2);

            It should_have_correct_item_left_in_cache =
                () =>
                {
                    var cachedStructures = _cache.GetAll<IAmCached>().OrderBy(s => s.Id).ToArray();
                    cachedStructures.ShouldBeValueEqualTo(_resultingStructures.Take(2).ToArray());
                    cachedStructures.ShouldBeValueEqualTo(_originalStructures);
                };

            private static ICache _cache;
            private static IAmCached[] _originalStructures;
            private static IAmCached[] _resultingStructures;
        }

        [Subject(typeof(ISession), "MsMemoryCache, GetByQuery")]
        public class when_msmemcache_is_enabled_and_get_by_query_is_executed : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                TestContext.Database.CacheProvider = new MsMemCacheProvider();
                TestContext.Database.CacheProvider.EnableFor(typeof(IAmCached));

                _cache = TestContext.Database.CacheProvider[typeof(IAmCached)];

                _originalStructures = new[] { new IAmCached { Value = 1 }, new IAmCached { Value = 2 } };
                TestContext.Database.UseOnceTo().InsertMany(_originalStructures);
            };

            Because of =
                () => _resultingStructure = TestContext.Database.UseOnceTo().GetByQuery<IAmCached>(i => i.Value == _originalStructures[1].Value);

            It should_have_exactly_one_item_in_cache =
                () => _cache.Count().ShouldEqual(1);

            It should_have_fetched_one_struture =
                () => _resultingStructure.ShouldNotBeNull();

            It should_fetch_the_second_structure =
                () => _resultingStructure.ShouldBeValueEqualTo(_originalStructures[1]);

            private static ICache _cache;
            private static IAmCached[] _originalStructures;
            private static IAmCached _resultingStructure;
        }

        [Subject(typeof(ISession), "MsMemoryCache, GetByQuery")]
        public class when_msmemcache_is_enabled_and_get_by_query_is_executed_twice : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                TestContext.Database.CacheProvider = new TestableMsMemCacheProvider();
                TestContext.Database.CacheProvider.EnableFor(typeof(IAmCached));

                _cache = TestContext.Database.CacheProvider[typeof(IAmCached)];

                _originalStructures = new[] { new IAmCached { Value = 1 }, new IAmCached { Value = 2 } };
                TestContext.Database.UseOnceTo().InsertMany(_originalStructures);
                TestContext.Database.UseOnceTo().GetByQuery<IAmCached>(i => i.Value == _originalStructures[1].Value);
            };

            Because of = () =>
            {
                _resultingStructure = TestContext.Database.UseOnceTo().GetByQuery<IAmCached>(i => i.Value == _originalStructures[1].Value);
            };

            It should_have_been_called_twice =
                () => ((TestableMsMemCache)TestContext.Database.CacheProvider[typeof(IAmCached)]).QueryCount.ShouldEqual(2);

            It should_have_exactly_one_item_in_cache =
                () => _cache.Count().ShouldEqual(1);

            It should_have_fetched_one_struture =
                () => _resultingStructure.ShouldNotBeNull();

            It should_fetch_the_second_structure =
                () => _resultingStructure.ShouldBeValueEqualTo(_originalStructures[1]);

            private static ICache _cache;
            private static IAmCached[] _originalStructures;
            private static IAmCached _resultingStructure;
        }

        [Subject(typeof(ISession), "MsMemoryCache, Query().Cacheable()")]
        public class when_msmemcache_is_enabled_and_query_is_executed : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                TestContext.Database.CacheProvider = new MsMemCacheProvider();
                TestContext.Database.CacheProvider.EnableFor(typeof(IAmCached));

                _cache = TestContext.Database.CacheProvider[typeof(IAmCached)];

                _originalStructures = new[] { new IAmCached { Value = 1 }, new IAmCached { Value = 2 } };
                TestContext.Database.UseOnceTo().InsertMany(_originalStructures);
            };

            Because of = () =>
            {
                _resultingStructures = TestContext.Database.UseOnceTo()
                    .Query<IAmCached>()
                    .Where(i => i.Value > 0)
                    .Cacheable()
                    .ToArray();
            };

            It should_have_exactly_two_items_in_cache =
                () => _cache.Count().ShouldEqual(2);

            It should_fetch_the_second_structure =
                () => _resultingStructures.ShouldBeValueEqualTo(_originalStructures);

            private static ICache _cache;
            private static IAmCached[] _originalStructures;
            private static IAmCached[] _resultingStructures;
        }

        private class IAmCached
        {
            public Guid Id { get; set; }

            public int Value { get; set; }
        }

        private class TestableMsMemCacheProvider : MsMemCacheProvider
        {
            protected override ICache OnCreate(Type structureType)
            {
                return new TestableMsMemCache(MemCacheConfigFn.Invoke(structureType));
            }
        }

        private class TestableMsMemCache : MsMemCache
        {
            public int QueryCount { get; private set; }

            public TestableMsMemCache(MsMemCacheConfig cacheConfig)
                : base(cacheConfig) { }

            public override IEnumerable<T> Query<T>(Expression<Func<T, bool>> predicate)
            {
                QueryCount++;
                return base.Query(predicate);
            }
        }
    }
}