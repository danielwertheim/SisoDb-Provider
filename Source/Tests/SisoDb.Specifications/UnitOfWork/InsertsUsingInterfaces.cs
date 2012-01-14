using Machine.Specifications;
using PineCone.Structures.Schemas;
using SisoDb.Testing;
using SisoDb.Testing.Steps;
using SisoDb.Testing.TestModel;

namespace SisoDb.Specifications.UnitOfWork
{
	class InsertsUsingInterfaces
    {
        [Subject(typeof(IWriteSession), "Insert (interfaces)")]
        public class when_inserting_subclass_as_subclass : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<MyItemWithInterface>();
            };

            Because of = () =>
            {
                _structure = new MyItemWithInterface
                {
                    MyItemInterfaceInt = 42,
                    MyItemInterfaceUniqueInt = 142,
                    MyItemInt = 242
                };

                using (var uow = TestContext.Database.BeginWriteSession())
                {
                    uow.Insert(_structure);
                }
            };

            It should_have_been_stored =
                () => TestContext.Database.should_have_X_num_of_items<MyItemWithInterface>(1);

            It should_have_stored_child_and_base_members_in_structure_table =
                () => TestContext.Database.should_have_identical_structures(_structure);

            It should_store_base_member_in_indexes_table =
                () => TestContext.DbHelper.AnyIndexesTableHasMember<MyItemWithInterface>(_structureSchema, _structure.StructureId, x => x.MyItemInterfaceInt).ShouldBeTrue();

            It should_store_unique_base_member_in_uniques_table =
                () => TestContext.DbHelper.UniquesTableHasMember<MyItemWithInterface>(_structureSchema, _structure.StructureId, x => x.MyItemInterfaceUniqueInt).ShouldBeTrue();

            private static IStructureSchema _structureSchema;
            private static MyItemWithInterface _structure;
        }

        [Subject(typeof(IWriteSession), "Insert (interfaces)")]
        public class when_inserting_subclass_as_interface : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<IMyItemInterface>();
            };

            Because of = () =>
            {
                _structure = new MyItemWithInterface()
                {
                    MyItemInterfaceInt = 42,
                    MyItemInterfaceUniqueInt = 142,
                    MyItemInt = 242
                };

                using (var uow = TestContext.Database.BeginWriteSession())
                {
                    uow.Insert(_structure);
                }
            };

            It should_have_been_stored =
                () => TestContext.Database.should_have_X_num_of_items<IMyItemInterface>(1);

            It should_have_stored_subclass_member_in_structure_table =
                () => TestContext.Database.should_have_one_structure_with_json_containing<IMyItemInterface, MyItemWithInterface>(x => x.MyItemInt);

            It should_store_base_member_in_indexes_table =
                () => TestContext.DbHelper.AnyIndexesTableHasMember<IMyItemInterface>(_structureSchema, _structure.StructureId, x => x.MyItemInterfaceInt).ShouldBeTrue();

            It should_store_unique_base_member_in_uniques_table =
                () => TestContext.DbHelper.UniquesTableHasMember<IMyItemInterface>(_structureSchema, _structure.StructureId, x => x.MyItemInterfaceUniqueInt).ShouldBeTrue();

            private static IStructureSchema _structureSchema;
            private static IMyItemInterface _structure;
        }  
    }
}