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
            var serializer = db.Serializer;
            var structureSchema = db.StructureSchemas.GetSchema<T>();

            for (var c = 0; c < numOfItems; c++)
            {
                itemsAsJson.Add(serializer.Serialize(new JsonItem
                {
                    Int1 = c + 1,
                    String1 = (c + 1).ToString(),
                    Decimal1 = (decimal)(c + 1) / (numOfItems * 10),
                    DateTime1 = new DateTime(2000 + c, 1, 1),
                    Ints = Enumerable.Range(1, c + 1).ToArray()
                }, structureSchema));
            }

            db.UseOnceTo().InsertManyJson<T>(itemsAsJson);

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

            db.UseOnceTo().InsertMany(items);
            
            return items;
        }

        public static IList<GuidItem> InsertGuidItems(this ISisoDatabase db, int numOfItems)
        {
            var items = new List<GuidItem>(numOfItems);

            for (var c = 0; c < numOfItems; c++)
                items.Add(new GuidItem { Value = c + 1, GuidValue = Guid.NewGuid() });

            db.UseOnceTo().InsertMany(items);
            
            return items;
        }

        public static IList<StringItem> InsertStringItems(this ISisoDatabase db, int numOfItems)
        {
            var items = new List<StringItem>(numOfItems);

            for (var c = 0; c < numOfItems; c++)
                items.Add(new StringItem { StructureId = (c + 1).ToString(), Value = c + 1 });

            db.UseOnceTo().InsertMany(items);

            return items;
        }

        public static IList<UniqueGuidItem> InsertUniqueGuidItems(this ISisoDatabase db, int numOfItems)
        {
            var items = new List<UniqueGuidItem>(numOfItems);

            for (var c = 0; c < numOfItems; c++)
                items.Add(new UniqueGuidItem { UniqueValue = c + 1 });

            db.UseOnceTo().InsertMany(items);

            return items;
        }

        public static IList<UniqueStringItem> InsertUniqueStringItems(this ISisoDatabase db, int numOfItems)
        {
            var items = new List<UniqueStringItem>(numOfItems);

            for (var c = 0; c < numOfItems; c++)
                items.Add(new UniqueStringItem { StructureId = (c + 1).ToString(), UniqueValue = c + 1 });

            db.UseOnceTo().InsertMany(items);

            return items;
        }

        public static IList<IdentityItem> InsertIdentityItems(this ISisoDatabase db, int numOfItems)
        {
            var items = new List<IdentityItem>(numOfItems);

            for (var c = 0; c < numOfItems; c++)
                items.Add(new IdentityItem { Value = c + 1 });

            db.UseOnceTo().InsertMany(items);

            return items;
        }

        public static IList<UniqueIdentityItem> InsertUniqueIdentityItems(this ISisoDatabase db, int numOfItems)
        {
            var items = new List<UniqueIdentityItem>(numOfItems);

            for (var c = 0; c < numOfItems; c++)
                items.Add(new UniqueIdentityItem { UniqueValue = c + 1 });

            db.UseOnceTo().InsertMany(items);

            return items;
        }

        public static IList<BigIdentityItem> InsertBigIdentityItems(this ISisoDatabase db, int numOfItems)
        {
            var items = new List<BigIdentityItem>(numOfItems);

            for (var c = 0; c < numOfItems; c++)
                items.Add(new BigIdentityItem { Value = c + 1 });

            db.UseOnceTo().InsertMany(items);

            return items;
        }

        public static IList<UniqueBigIdentityItem> InsertUniqueBigIdentityItems(this ISisoDatabase db, int numOfItems)
        {
            var items = new List<UniqueBigIdentityItem>(numOfItems);

            for (var c = 0; c < numOfItems; c++)
                items.Add(new UniqueBigIdentityItem { UniqueValue = c + 1 });

            db.UseOnceTo().InsertMany(items);

            return items;
        }
    }
}