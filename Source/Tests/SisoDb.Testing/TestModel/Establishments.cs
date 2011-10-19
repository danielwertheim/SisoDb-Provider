using System.Collections.Generic;

namespace SisoDb.Testing.TestModel
{
    public static class Establishments
    {
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
                items.Add(new UniqueGuidItem { Value = c + 1 });

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
                items.Add(new UniqueIdentityItem { Value = c + 1 });

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
    }
}