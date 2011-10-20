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
        public static void should_have_been_deleted_from_structures_table(this ITestDbUtils db, IStructureSchema structureSchema, ValueType structureId)
        {
            db.RowCount(structureSchema.GetStructureTableName(), "StructureId = '{0}'".Inject(structureId)).ShouldEqual(0);
        }

        public static void should_not_have_been_deleted_from_structures_table(this ITestDbUtils db, IStructureSchema structureSchema, ValueType structureId)
        {
            db.RowCount(structureSchema.GetStructureTableName(), "StructureId = '{0}'".Inject(structureId)).ShouldEqual(1);
        }

        public static void should_have_been_deleted_from_indexes_table(this ITestDbUtils db, IStructureSchema structureSchema, ValueType structureId)
        {
            db.RowCount(structureSchema.GetIndexesTableName(), "StructureId = '{0}'".Inject(structureId)).ShouldEqual(0);
        }

        public static void should_not_have_been_deleted_from_indexes_table(this ITestDbUtils db, IStructureSchema structureSchema, ValueType structureId)
        {
            db.RowCount(structureSchema.GetIndexesTableName(), "StructureId = '{0}'".Inject(structureId)).ShouldEqual(1);
        }

        public static void should_have_been_deleted_from_uniques_table(this ITestDbUtils db, IStructureSchema structureSchema, ValueType structureId)
        {
            db.RowCount(structureSchema.GetUniquesTableName(), "StructureId = '{0}'".Inject(structureId)).ShouldEqual(0);
        }

        public static void should_not_have_been_deleted_from_uniques_table(this ITestDbUtils db, IStructureSchema structureSchema, ValueType structureId)
        {
            db.RowCount(structureSchema.GetUniquesTableName(), "StructureId = '{0}'".Inject(structureId)).ShouldEqual(1);
        }

        public static void should_only_have_X_items_left<T>(this ISisoDatabase db, int numOfItemsLeft) where T : class
        {
            using (var qe = db.CreateQueryEngine())
                qe.Count<T>().ShouldEqual(numOfItemsLeft);
        }

        public static void should_have_first_and_last_item_left(this ISisoDatabase db, IList<GuidItem> structures)
        {
            using (var qe = db.CreateQueryEngine())
            {
                var items = qe.GetAll<GuidItem>().ToList();
                items[0].StructureId.ShouldEqual(structures[0].StructureId);
                items[1].StructureId.ShouldEqual(structures[3].StructureId);
            }
        }

        public static void should_have_first_and_last_item_left(this ISisoDatabase db, IList<UniqueGuidItem> structures)
        {
            using (var qe = db.CreateQueryEngine())
            {
                var items = qe.GetAll<UniqueGuidItem>().ToList();
                items[0].StructureId.ShouldEqual(structures[0].StructureId);
                items[1].StructureId.ShouldEqual(structures[3].StructureId);
            }
        }

        public static void should_have_first_and_last_item_left(this ISisoDatabase db, IList<IdentityItem> structures)
        {
            using (var qe = db.CreateQueryEngine())
            {
                var items = qe.GetAll<IdentityItem>().ToList();
                items[0].StructureId.ShouldEqual(structures[0].StructureId);
                items[1].StructureId.ShouldEqual(structures[3].StructureId);
            }
        }

        public static void should_have_first_and_last_item_left(this ISisoDatabase db, IList<UniqueIdentityItem> structures)
        {
            using (var qe = db.CreateQueryEngine())
            {
                var items = qe.GetAll<UniqueIdentityItem>().ToList();
                items[0].StructureId.ShouldEqual(structures[0].StructureId);
                items[1].StructureId.ShouldEqual(structures[3].StructureId);
            }
        }

        public static void should_have_first_and_last_item_left(this ISisoDatabase db, IList<BigIdentityItem> structures)
        {
            using (var qe = db.CreateQueryEngine())
            {
                var items = qe.GetAll<BigIdentityItem>().ToList();
                items[0].StructureId.ShouldEqual(structures[0].StructureId);
                items[1].StructureId.ShouldEqual(structures[3].StructureId);
            }
        }
    }
}