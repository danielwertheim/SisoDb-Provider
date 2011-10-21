using System;
using System.Collections.Generic;
using System.Linq;
using EnsureThat;
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

        public static void should_have_X_items_left<T>(this ISisoDatabase db, int numOfItemsLeft) where T : class
        {
            using (var qe = db.CreateQueryEngine())
                qe.Count<T>().ShouldEqual(numOfItemsLeft);
        }

        public static void should_have_first_and_last_item_left<T>(this ISisoDatabase db, IList<T> structures) where T : class 
        {
            var structureScema = db.StructureSchemas.GetSchema<T>();

            using (var qe = db.CreateQueryEngine())
            {
                var items = qe.GetAll<T>().ToList();
                
                structureScema.IdAccessor.GetValue(items[0]).Value.ShouldEqual(
                    structureScema.IdAccessor.GetValue(structures[0]).Value);

                structureScema.IdAccessor.GetValue(items[1]).Value.ShouldEqual(
                    structureScema.IdAccessor.GetValue(structures[3]).Value);
            }
        }

        public static void should_have_ids<T>(this ISisoDatabase db, params ValueType[] ids) where T : class
        {
            Ensure.That(ids, "ids").HasItems();

            using (var qe = db.CreateQueryEngine())
            {
                foreach (var id in ids)
                    qe.GetById<T>(id).ShouldNotBeNull();
            }
        }

        public static void should_have_identical_structures<T>(this ISisoDatabase db, params T[] structures) where T : class
        {
            Ensure.That(structures, "structures").HasItems();

            var structureSchema = db.StructureSchemas.GetSchema(typeof(T));

            using (var qe = db.CreateQueryEngine())
            {
                foreach (var structure in structures)
                {
                    var id = structureSchema.IdAccessor.GetValue(structure);
                    var refetched = qe.GetById<T>(id.Value);
                    refetched.ShouldBeValueEqualTo(structure);
                }
            }
        }

        public static void should_have_valid_structures<T>(this ISisoDatabase db, Action<int, T, T> validationRule, params T[] structures) where T : class
        {
            Ensure.That(structures, "structures").HasItems();

            var structureSchema = db.StructureSchemas.GetSchema<T>();

            using (var qe = db.CreateQueryEngine())
            {
                for (var c = 0; c < structures.Length; c++)
                {
                    var structure = structures[c];
                    var id = structureSchema.IdAccessor.GetValue(structure);
                    var refetched = qe.GetById<T>(id.Value);
                    validationRule(c, structure, refetched);
                }
            }
        }
    }
}