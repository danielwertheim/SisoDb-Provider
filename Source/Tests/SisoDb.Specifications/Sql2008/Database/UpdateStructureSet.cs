using System;
using Machine.Specifications;
using SisoDb.Sql2008;
using SisoDb.Testing;

namespace SisoDb.Specifications.Sql2008.Database
{
    namespace UpdateStructureSet
    {
        [Subject(typeof(Sql2008Database), "Update structure set")]
        public class when_stored_as_person_and_transformed_to_sales_person_with_more_fine_grained_properties : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create(StorageProviders.Sql2008);

                var orgItem = new ModelComplexUpdates.Person { Name = "Daniel Wertheim", Address = "The Street 1\r\n12345\r\nThe City" };
                using (var uow = TestContext.Database.CreateUnitOfWork())
                {
                    uow.Insert(orgItem);
                    uow.Commit();
                }

                _personId = orgItem.StructureId;
            };

            Because of = () =>
                TestContext.Database.UpdateStructureSet<ModelComplexUpdates.Person, ModelComplexUpdates.SalesPerson>((p, sp) =>
                {
                    var names = p.Name.Split(' ');
                    sp.Firstname = names[0];
                    sp.Lastname = names[1];

                    var address = p.Address.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    sp.Office.Street = address[0];
                    sp.Office.Zip = address[1];
                    sp.Office.City = address[2];

                    return StructureSetUpdaterStatuses.Keep;
                });

            It should_have_removed_the_person = () =>
            {
                using (var q = TestContext.Database.CreateQueryEngine())
                {
                    q.Count<ModelComplexUpdates.Person>().ShouldEqual(0);
                }
            };

            It should_have_created_a_salesperson_from_the_person = () =>
            {
                using (var q = TestContext.Database.CreateQueryEngine())
                {
                    var refetchedSalesPerson = q.GetById<ModelComplexUpdates.SalesPerson>(_personId);
                    refetchedSalesPerson.Firstname.ShouldEqual("Daniel");
                    refetchedSalesPerson.Lastname.ShouldEqual("Wertheim");
                    refetchedSalesPerson.Office.Street.ShouldEqual("The Street 1");
                    refetchedSalesPerson.Office.Zip.ShouldEqual("12345");
                    refetchedSalesPerson.Office.City.ShouldEqual("The City");
                }
            };

            private static Guid _personId;
        }

        [Subject(typeof(Sql2008Database), "Update structure set")]
        public class when_three_structures_with_identities_exists_and_trash_is_made_on_second : SpecificationBase
        {
            Establish context = () =>
            {
                var orgItem1 = new ModelOld.ItemForPropChange { String1 = "A" };
                var orgItem2 = new ModelOld.ItemForPropChange { String1 = "B" };
                var orgItem3 = new ModelOld.ItemForPropChange { String1 = "C" };

                var testContext = TestContextFactory.Create(StorageProviders.Sql2008);
                using (var uow = testContext.Database.CreateUnitOfWork())
                {
                    uow.InsertMany(new[] { orgItem1, orgItem2, orgItem3 });
                    uow.Commit();
                }

                _orgItem1Id = orgItem1.StructureId;
                _orgItem2Id = orgItem2.StructureId;
                _orgItem3Id = orgItem3.StructureId;

                TestContext = TestContextFactory.Create(StorageProviders.Sql2008);
            };

            private Because of = () =>
                TestContext.Database.UpdateStructureSet<ModelOld.ItemForPropChange, ModelNew.ItemForPropChange>(
                (oldItem, newItem) =>
                {
                    newItem.NewString1 = oldItem.String1;

                    if (oldItem.StructureId == _orgItem2Id)
                        return StructureSetUpdaterStatuses.Trash;

                    return StructureSetUpdaterStatuses.Keep;
                });

            It should_have_kept_and_updated_the_first_and_last_items = () =>
            {
                using (var q = TestContext.Database.CreateQueryEngine())
                {
                    var newItem1 = q.GetById<ModelNew.ItemForPropChange>(_orgItem1Id);
                    newItem1.ShouldNotBeNull();
                    newItem1.NewString1 = "A";

                    var newItem2 = q.GetById<ModelNew.ItemForPropChange>(_orgItem2Id);
                    newItem2.ShouldBeNull();

                    var newItem3 = q.GetById<ModelNew.ItemForPropChange>(_orgItem3Id);
                    newItem3.ShouldNotBeNull();
                    newItem3.NewString1 = "C";
                }
            };

            It should_have_removed_the_second_item = () =>
            {
                using (var q = TestContext.Database.CreateQueryEngine())
                {
                    var newItem2 = q.GetById<ModelNew.ItemForPropChange>(_orgItem2Id);
                    newItem2.ShouldBeNull();
                }
            };

            private static int _orgItem1Id, _orgItem2Id, _orgItem3Id;
        }

        namespace ModelComplexUpdates
        {
            public class Person
            {
                public Guid StructureId { get; set; }

                public string Name { get; set; }

                public string Address { get; set; }
            }

            public class SalesPerson
            {
                public Guid StructureId { get; set; }

                public string Firstname { get; set; }

                public string Lastname { get; set; }

                public Address Office { get; set; }

                public SalesPerson()
                {
                    Office = new Address();
                }
            }

            public class Address
            {
                public string Street { get; set; }
                public string Zip { get; set; }
                public string City { get; set; }
            }
        }

        namespace ModelOld
        {
            public class ItemForPropChange
            {
                public int StructureId { get; set; }

                public string String1 { get; set; }

                public int Int1 { get; set; }
            }

            public class GuidItemForPropChange
            {
                public Guid StructureId { get; set; }

                public string String1 { get; set; }

                public int Int1 { get; set; }
            }
        }

        namespace ModelNew
        {
            public class ItemForPropChange
            {
                public int StructureId { get; set; }

                public string NewString1 { get; set; }
            }

            public class GuidItemForPropChange
            {
                public Guid StructureId { get; set; }

                public string NewString1 { get; set; }
            }
        }
    }
}