using System;
using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;
using NCore;
using PineCone.Structures.Schemas;
using SisoDb.Structures;

namespace SisoDb.Testing.TestModel
{
    public static class Shoulds
    {
        public static It should_have_been_deleted_from_structures_table(this ITestDbUtils db, IStructureSchema structureSchema, ValueType structureId)
        {
            It it = () => db.RowCount(structureSchema.GetStructureTableName(), "StructureId = '{0}'".Inject(structureId)).ShouldEqual(0);

            return it;
        }

        public static It should_have_been_deleted_from_indexes_table(this ITestDbUtils db, IStructureSchema structureSchema, ValueType structureId)
        {
            It it = () => db.RowCount(structureSchema.GetIndexesTableName(), "StructureId = '{0}'".Inject(structureId)).ShouldEqual(0);

            return it;
        }

        public static It should_have_been_deleted_from_uniques_table(this ITestDbUtils db, IStructureSchema structureSchema, ValueType structureId)
        {
            It it = () => db.RowCount(structureSchema.GetUniquesTableName(), "StructureId = '{0}'".Inject(structureId)).ShouldEqual(0);

            return it;
        }

        public static It should_only_have_X_items_left<T>(this ISisoDatabase db, int numOfItemsLeft) where T : class
        {
            It it = () =>
            {
                using (var qe = db.CreateQueryEngine())
                    qe.Count<T>().ShouldEqual(numOfItemsLeft);
            };

            return it;
        }

        public static It should_have_first_and_last_item_left(this ISisoDatabase db, IList<GuidItem> structures)
        {
            It it = () =>
            {
                using (var qe = db.CreateQueryEngine())
                {
                    var items = qe.GetAll<GuidItem>().ToList();
                    items[0].StructureId.ShouldEqual(structures[0].StructureId);
                    items[1].StructureId.ShouldEqual(structures[3].StructureId);
                }
            };

            return it;
        }

        public static It should_have_first_and_last_item_left(this ISisoDatabase db, IList<IdentityItem> structures)
        {
            It it = () =>
            {
                using (var qe = db.CreateQueryEngine())
                {
                    var items = qe.GetAll<IdentityItem>().ToList();
                    items[0].StructureId.ShouldEqual(structures[0].StructureId);
                    items[1].StructureId.ShouldEqual(structures[3].StructureId);
                }
            };

            return it;
        }

        public static It should_have_first_and_last_item_left(this ISisoDatabase db, IList<BigIdentityItem> structures)
        {
            It it = () =>
            {
                using (var qe = db.CreateQueryEngine())
                {
                    var items = qe.GetAll<BigIdentityItem>().ToList();
                    items[0].StructureId.ShouldEqual(structures[0].StructureId);
                    items[1].StructureId.ShouldEqual(structures[3].StructureId);
                }
            };

            return it;
        }
    }
}