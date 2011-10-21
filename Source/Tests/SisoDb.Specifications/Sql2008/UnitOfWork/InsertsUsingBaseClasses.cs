using Machine.Specifications;
using PineCone.Structures.Schemas;
using SisoDb.Sql2008;
using SisoDb.Testing;
using SisoDb.Testing.Steps;
using SisoDb.Testing.TestModel;

namespace SisoDb.Specifications.Sql2008.UnitOfWork
{
    namespace InsertsUsingBaseClasses
    {
        [Subject(typeof(Sql2008UnitOfWork), "Insert (base classes)")]
        public class when_inserting_subclass_as_subclass : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create(StorageProviders.Sql2008);
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<MyItem>();
            };

            Because of = () =>
            {
                _structure = new MyItem
                {
                    MyItemBaseInt = 42,
                    MyItemBaseUniqueInt = 142,
                    MyItemInt = 242
                };

                using(var uow = TestContext.Database.CreateUnitOfWork())
                {
                    uow.Insert(_structure);
                    uow.Commit();
                }
            };

            It should_have_been_stored =
                () => TestContext.Database.should_have_X_num_of_items<MyItem>(1);

            It should_have_stored_child_and_base_members_in_structure_table =
                () => TestContext.Database.should_have_identical_structures(_structure);

            It should_store_base_member_in_indexes_table = 
                () => TestContext.DbHelper.IndexesTableHasMember<MyItem>(_structureSchema, _structure.StructureId, x => x.MyItemBaseInt).ShouldBeTrue();

            It should_store_unique_base_member_in_uniques_table = 
                () => TestContext.DbHelper.UniquesTableHasMember<MyItem>(_structureSchema, _structure.StructureId, x => x.MyItemBaseUniqueInt).ShouldBeTrue();

            private static IStructureSchema _structureSchema;
            private static MyItem _structure;
        }

        [Subject(typeof(Sql2008UnitOfWork), "Insert (base classes)")]
        public class when_inserting_subclass_as_baseclass : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create(StorageProviders.Sql2008);
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<MyItemBase>();
            };

            Because of = () =>
            {
                _structure = new MyItem
                {
                    MyItemBaseInt = 42,
                    MyItemBaseUniqueInt = 142,
                    MyItemInt = 242
                };

                using (var uow = TestContext.Database.CreateUnitOfWork())
                {
                    uow.Insert<MyItemBase>(_structure);
                    uow.Commit();
                }
            };

            It should_have_been_stored =
                () => TestContext.Database.should_have_X_num_of_items<MyItemBase>(1);

            It should_not_have_stored_subclass_member_in_structure_table =
                () => TestContext.Database.should_have_one_structure_with_json_not_containing<MyItemBase, MyItem>(x => x.MyItemInt);

            It should_store_base_member_in_indexes_table =
                () => TestContext.DbHelper.IndexesTableHasMember<MyItemBase>(_structureSchema, _structure.StructureId, x => x.MyItemBaseInt).ShouldBeTrue();

            It should_store_unique_base_member_in_uniques_table =
                () => TestContext.DbHelper.UniquesTableHasMember<MyItemBase>(_structureSchema, _structure.StructureId, x => x.MyItemBaseUniqueInt).ShouldBeTrue();

            private static IStructureSchema _structureSchema;
            private static MyItem _structure;
        }
    }
}