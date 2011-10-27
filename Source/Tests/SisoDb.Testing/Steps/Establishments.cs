using System;
using System.Collections.Generic;
using System.Linq;
using SisoDb.Testing.TestModel;

namespace SisoDb.Testing.Steps
{
    public static class Establishments
    {
        public static IList<string> InsertJsonItems<T>(this ISisoDatabase db, int numOfItems) where T : class
        {
            var itemsAsJson = new List<string>(numOfItems);
            var serializer = SisoEnvironment.Resources.ResolveJsonSerializer();

            for (var c = 0; c < numOfItems; c++)
            {
                itemsAsJson.Add(serializer.Serialize(new JsonItem
                {
                    Int1 = c + 1,
                    String1 = (c + 1).ToString(),
                    Decimal1 = (decimal)(c + 1) / (numOfItems * 10),
                    DateTime1 = new DateTime(2000 + c, 1, 1),
                    Ints = Enumerable.Range(1, c + 1).ToArray()
                }));
            }

            using (var uow = db.CreateUnitOfWork())
            {
                uow.InsertManyJson<T>(itemsAsJson);
                uow.Commit();
            }

            return itemsAsJson;
        }

        public static IList<JsonItem> InsertJsonItems(this ISisoDatabase db, int numOfItems)
        {
            var items = new List<JsonItem>(numOfItems);

            for (var c = 0; c < numOfItems; c++)
            {
                items.Add(new JsonItem
                {
                    Int1 = c + 1,
                    String1 = (c + 1).ToString(),
                    Decimal1 = (decimal)(c + 1) / (numOfItems * 10),
                    DateTime1 = new DateTime(2000 + c, 1, 1),
                    Ints = Enumerable.Range(1, c + 1).ToArray()
                });
            }

            using (var uow = db.CreateUnitOfWork())
            {
                uow.InsertMany(items);
                uow.Commit();
            }

            return items;
        }

        public static IList<T> InsertMany<T>(this ISisoDatabase db, IList<T> items) where T : class 
        {
            using (var uow = db.CreateUnitOfWork())
            {
                uow.InsertMany(items);
                uow.Commit();
            }

            return items;
        }

        public static T Insert<T>(this ISisoDatabase db, T item) where T : class
        {
            using (var uow = db.CreateUnitOfWork())
            {
                uow.Insert(item);
                uow.Commit();
            }

            return item;
        }

        public static IList<GuidItem> InsertGuidItems(this ISisoDatabase db, int numOfItems)
        {
            var items = new List<GuidItem>(numOfItems);

            for (var c = 0; c < numOfItems; c++)
                items.Add(new GuidItem { Value = c + 1 });

            using (var uow = db.CreateUnitOfWork())
            {
                uow.InsertMany(items);
                uow.Commit();
            }

            return items;
        }

        public static IList<UniqueGuidItem> InsertUniqueGuidItems(this ISisoDatabase db, int numOfItems)
        {
            var items = new List<UniqueGuidItem>(numOfItems);

            for (var c = 0; c < numOfItems; c++)
                items.Add(new UniqueGuidItem { UniqueValue = c + 1 });

            using (var uow = db.CreateUnitOfWork())
            {
                uow.InsertMany(items);
                uow.Commit();
            }

            return items;
        }

        public static IList<IdentityItem> InsertIdentityItems(this ISisoDatabase db, int numOfItems)
        {
            var items = new List<IdentityItem>(numOfItems);

            for (var c = 0; c < numOfItems; c++)
                items.Add(new IdentityItem { Value = c + 1 });

            using (var uow = db.CreateUnitOfWork())
            {
                uow.InsertMany(items);
                uow.Commit();
            }

            return items;
        }

        public static IList<UniqueIdentityItem> InsertUniqueIdentityItems(this ISisoDatabase db, int numOfItems)
        {
            var items = new List<UniqueIdentityItem>(numOfItems);

            for (var c = 0; c < numOfItems; c++)
                items.Add(new UniqueIdentityItem { UniqueValue = c + 1 });

            using (var uow = db.CreateUnitOfWork())
            {
                uow.InsertMany(items);
                uow.Commit();
            }

            return items;
        }

        public static IList<BigIdentityItem> InsertBigIdentityItems(this ISisoDatabase db, int numOfItems)
        {
            var items = new List<BigIdentityItem>(numOfItems);

            for (var c = 0; c < numOfItems; c++)
                items.Add(new BigIdentityItem { Value = c + 1 });

            using (var uow = db.CreateUnitOfWork())
            {
                uow.InsertMany(items);
                uow.Commit();
            }

            return items;
        }

        public static IList<UniqueBigIdentityItem> InsertUniqueBigIdentityItems(this ISisoDatabase db, int numOfItems)
        {
            var items = new List<UniqueBigIdentityItem>(numOfItems);

            for (var c = 0; c < numOfItems; c++)
                items.Add(new UniqueBigIdentityItem { UniqueValue = c + 1 });

            using (var uow = db.CreateUnitOfWork())
            {
                uow.InsertMany(items);
                uow.Commit();
            }

            return items;
        }
    }
}