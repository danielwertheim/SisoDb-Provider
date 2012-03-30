using System;
using System.Collections.Generic;
using Machine.Specifications;
using NCore;
using SisoDb.Maintenance;
using SisoDb.Resources;
using SisoDb.Testing;

namespace SisoDb.Specifications.Database.Maintenance
{
	class Migrations
	{
        [Subject(typeof(ISisoDatabaseMaintenance), "Migrate")]
		public class when_stored_as_person_and_migrated_to_sales_person_with_more_fine_grained_properties_using_func : SpecificationBase
		{
			Establish context = () =>
			{
				TestContext = TestContextFactory.Create();

				var orgItem = new Model1.Person { Name = "Daniel Wertheim", Address = "The Street 1\r\n12345\r\nThe City" };
                TestContext.Database.UseOnceTo().Insert(orgItem);
				_personId = orgItem.StructureId;
			};

			Because of = () =>
                TestContext.Database.Maintenance.Migrate<Model1.Person, Model1.SalesPerson>((p, sp) =>
				{
					var names = p.Name.Split(' ');
					sp.Firstname = names[0];
					sp.Lastname = names[1];

					var address = p.Address.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
					sp.Office.Street = address[0];
					sp.Office.Zip = address[1];
					sp.Office.City = address[2];

					return MigrationStatuses.Keep;
				});

			It should_not_have_removed_the_person = () =>
			{
				using (var q = TestContext.Database.BeginSession())
				{
                    q.Query<Model1.Person>().Count().ShouldEqual(1);
				}
			};

			It should_have_created_a_salesperson_from_the_person = () =>
			{
				using (var q = TestContext.Database.BeginSession())
				{
                    var refetchedSalesPerson = q.GetById<Model1.SalesPerson>(_personId);
					refetchedSalesPerson.Firstname.ShouldEqual("Daniel");
					refetchedSalesPerson.Lastname.ShouldEqual("Wertheim");
					refetchedSalesPerson.Office.Street.ShouldEqual("The Street 1");
					refetchedSalesPerson.Office.Zip.ShouldEqual("12345");
					refetchedSalesPerson.Office.City.ShouldEqual("The City");
				}
			};

			private static Guid _personId;
		}

        [Subject(typeof(ISisoDatabaseMaintenance), "Migrate")]
        public class when_stored_as_person_and_migrated_to_sales_person_with_more_fine_grained_properties_using_migration : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();

                var orgItem = new Model1.Person { Name = "Daniel Wertheim", Address = "The Street 1\r\n12345\r\nThe City" };
                TestContext.Database.UseOnceTo().Insert(orgItem);
                _personId = orgItem.StructureId;
            };

            Because of = () =>
                TestContext.Database.Maintenance.Migrate(Migrate<Model1.Person>.To<Model1.SalesPerson>().Using((p, sp) =>
                {
                    var names = p.Name.Split(' ');
                    sp.Firstname = names[0];
                    sp.Lastname = names[1];

                    var address = p.Address.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    sp.Office.Street = address[0];
                    sp.Office.Zip = address[1];
                    sp.Office.City = address[2];

                    return MigrationStatuses.Keep;
                }));

            It should_not_have_removed_the_person = () =>
            {
                using (var q = TestContext.Database.BeginSession())
                {
                    q.Query<Model1.Person>().Count().ShouldEqual(1);
                }
            };

            It should_have_created_a_salesperson_from_the_person = () =>
            {
                using (var q = TestContext.Database.BeginSession())
                {
                    var refetchedSalesPerson = q.GetById<Model1.SalesPerson>(_personId);
                    refetchedSalesPerson.Firstname.ShouldEqual("Daniel");
                    refetchedSalesPerson.Lastname.ShouldEqual("Wertheim");
                    refetchedSalesPerson.Office.Street.ShouldEqual("The Street 1");
                    refetchedSalesPerson.Office.Zip.ShouldEqual("12345");
                    refetchedSalesPerson.Office.City.ShouldEqual("The City");
                }
            };

            private static Guid _personId;
        }

        [Subject(typeof(ISisoDatabaseMaintenance), "Migrate")]
        public class when_stored_as_person_and_migrated_to_sales_person_with_more_fine_grained_properties_using_migration_with_anonymous_object : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();

                var orgItem = new Model1.Person { Name = "Daniel Wertheim", Address = "The Street 1\r\n12345\r\nThe City" };
                TestContext.Database.UseOnceTo().Insert(orgItem);
                _personId = orgItem.StructureId;
            };

            Because of = () =>
                TestContext.Database.Maintenance.Migrate(Migrate<Model1.Person>.To<Model1.SalesPerson>().Using(
                new
                {
                    StructureId = default(Guid),
                    Name = default(string),
                    Address = default(string)
                },
                (p, sp) =>
                {
                    var names = p.Name.Split(' ');
                    sp.Firstname = names[0];
                    sp.Lastname = names[1];

                    var address = p.Address.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    sp.Office.Street = address[0];
                    sp.Office.Zip = address[1];
                    sp.Office.City = address[2];

                    return MigrationStatuses.Keep;
                }));

            It should_not_have_removed_the_person = () =>
            {
                using (var q = TestContext.Database.BeginSession())
                {
                    q.Query<Model1.Person>().Count().ShouldEqual(1);
                }
            };

            It should_have_created_a_salesperson_from_the_person = () =>
            {
                using (var q = TestContext.Database.BeginSession())
                {
                    var refetchedSalesPerson = q.GetById<Model1.SalesPerson>(_personId);
                    refetchedSalesPerson.Firstname.ShouldEqual("Daniel");
                    refetchedSalesPerson.Lastname.ShouldEqual("Wertheim");
                    refetchedSalesPerson.Office.Street.ShouldEqual("The Street 1");
                    refetchedSalesPerson.Office.Zip.ShouldEqual("12345");
                    refetchedSalesPerson.Office.City.ShouldEqual("The City");
                }
            };

            private static Guid _personId;
        }

        [Subject(typeof(ISisoDatabaseMaintenance), "Migrate")]
        public class when_stored_as_person_and_migrated_to_new_person_with_more_fine_grained_properties_using_func : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();

                var orgItem = new Model1.Person { Name = "Daniel Wertheim", Address = "The Street 1\r\n12345\r\nThe City" };
                TestContext.Database.UseOnceTo().Insert(orgItem);
                _personId = orgItem.StructureId;
            };

            Because of = () =>
                TestContext.Database.Maintenance.Migrate<Model1.Person, Model2.Person>((p, sp) =>
                {
                    var names = p.Name.Split(' ');
                    sp.Firstname = names[0];
                    sp.Lastname = names[1];

                    var address = p.Address.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    sp.Office.Street = address[0];
                    sp.Office.Zip = address[1];
                    sp.Office.City = address[2];

                    return MigrationStatuses.Keep;
                });

            It should_have_one_person = () =>
            {
                using (var q = TestContext.Database.BeginSession())
                {
                    q.Query<Model2.Person>().Count().ShouldEqual(1);
                }
            };

            It should_have_updated_from_old_to_new_person = () =>
            {
                using (var q = TestContext.Database.BeginSession())
                {
                    var refetchedSalesPerson = q.GetById<Model2.Person>(_personId);
                    refetchedSalesPerson.Firstname.ShouldEqual("Daniel");
                    refetchedSalesPerson.Lastname.ShouldEqual("Wertheim");
                    refetchedSalesPerson.Office.Street.ShouldEqual("The Street 1");
                    refetchedSalesPerson.Office.Zip.ShouldEqual("12345");
                    refetchedSalesPerson.Office.City.ShouldEqual("The City");
                }
            };

            private static Guid _personId;
        }

        [Subject(typeof(ISisoDatabaseMaintenance), "Migrate")]
        public class when_stored_as_person_and_migrated_to_new_person_with_more_fine_grained_properties_using_migration : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();

                var orgItem = new Model1.Person { Name = "Daniel Wertheim", Address = "The Street 1\r\n12345\r\nThe City" };
                TestContext.Database.UseOnceTo().Insert(orgItem);
                _personId = orgItem.StructureId;
            };

            Because of = () =>
                TestContext.Database.Maintenance.Migrate(Migrate<Model1.Person>.To<Model2.Person>().Using((p, sp) =>
                {
                    var names = p.Name.Split(' ');
                    sp.Firstname = names[0];
                    sp.Lastname = names[1];

                    var address = p.Address.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    sp.Office.Street = address[0];
                    sp.Office.Zip = address[1];
                    sp.Office.City = address[2];

                    return MigrationStatuses.Keep;
                }));

            It should_have_one_person = () =>
            {
                using (var q = TestContext.Database.BeginSession())
                {
                    q.Query<Model2.Person>().Count().ShouldEqual(1);
                }
            };

            It should_have_updated_from_old_to_new_person = () =>
            {
                using (var q = TestContext.Database.BeginSession())
                {
                    var refetchedSalesPerson = q.GetById<Model2.Person>(_personId);
                    refetchedSalesPerson.Firstname.ShouldEqual("Daniel");
                    refetchedSalesPerson.Lastname.ShouldEqual("Wertheim");
                    refetchedSalesPerson.Office.Street.ShouldEqual("The Street 1");
                    refetchedSalesPerson.Office.Zip.ShouldEqual("12345");
                    refetchedSalesPerson.Office.City.ShouldEqual("The City");
                }
            };

            private static Guid _personId;
        }

        [Subject(typeof(ISisoDatabaseMaintenance), "Migrate")]
        public class when_stored_as_person_and_migrated_to_new_person_with_more_fine_grained_properties_using_migration_with_anonymous : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();

                var orgItem = new Model1.Person { Name = "Daniel Wertheim", Address = "The Street 1\r\n12345\r\nThe City" };
                TestContext.Database.UseOnceTo().Insert(orgItem);
                _personId = orgItem.StructureId;
            };

            Because of = () =>
                TestContext.Database.Maintenance.Migrate(Migrate<Model1.Person>.To<Model2.Person>().Using(
                new
                {
                    StructureId = default(Guid),
                    Name = default(string),
                    Address = default(string)
                },
                (p, sp) =>
                {
                    var names = p.Name.Split(' ');
                    sp.Firstname = names[0];
                    sp.Lastname = names[1];

                    var address = p.Address.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    sp.Office.Street = address[0];
                    sp.Office.Zip = address[1];
                    sp.Office.City = address[2];

                    return MigrationStatuses.Keep;
                }));

            It should_have_one_person = () =>
            {
                using (var q = TestContext.Database.BeginSession())
                {
                    q.Query<Model2.Person>().Count().ShouldEqual(1);
                }
            };

            It should_have_updated_from_old_to_new_person = () =>
            {
                using (var q = TestContext.Database.BeginSession())
                {
                    var refetchedSalesPerson = q.GetById<Model2.Person>(_personId);
                    refetchedSalesPerson.Firstname.ShouldEqual("Daniel");
                    refetchedSalesPerson.Lastname.ShouldEqual("Wertheim");
                    refetchedSalesPerson.Office.Street.ShouldEqual("The Street 1");
                    refetchedSalesPerson.Office.Zip.ShouldEqual("12345");
                    refetchedSalesPerson.Office.City.ShouldEqual("The City");
                }
            };

            private static Guid _personId;
        }

        [Subject(typeof(ISisoDatabaseMaintenance), "Migrate")]
        public class when_stored_as_person_and_migrated_to_same_type_of_person_with_more_fine_grained_properties_using_func : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();

                var orgItem = new Model1.Person { Name = "Daniel Wertheim", Address = "The Street 1\r\n12345\r\nThe City" };
                TestContext.Database.UseOnceTo().Insert(orgItem);
                _personId = orgItem.StructureId;
            };

            Because of = () =>
                TestContext.Database.Maintenance.Migrate<Model1.Person, Model1.Person>((p, sp) =>
                {
                    sp.Name = "New name";

                    return MigrationStatuses.Keep;
                });

            It should_have_one_person = () =>
            {
                using (var q = TestContext.Database.BeginSession())
                {
                    q.Query<Model1.Person>().Count().ShouldEqual(1);
                }
            };

            It should_have_updated_from_old_to_new_person = () =>
            {
                using (var q = TestContext.Database.BeginSession())
                {
                    var refetchedSalesPerson = q.GetById<Model1.Person>(_personId);
                    refetchedSalesPerson.Name.ShouldEqual("New name");
                    refetchedSalesPerson.Address.ShouldEqual("The Street 1\r\n12345\r\nThe City");
                }
            };

            private static Guid _personId;
        }

        [Subject(typeof(ISisoDatabaseMaintenance), "Migrate")]
        public class when_stored_as_person_and_migrated_to_same_type_of_person_with_more_fine_grained_properties_using_migration : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();

                var orgItem = new Model1.Person { Name = "Daniel Wertheim", Address = "The Street 1\r\n12345\r\nThe City" };
                TestContext.Database.UseOnceTo().Insert(orgItem);
                _personId = orgItem.StructureId;
            };

            Because of = () =>
                TestContext.Database.Maintenance.Migrate(Migrate<Model1.Person>.To<Model1.Person>().Using((p, sp) =>
                {
                    sp.Name = "New name";

                    return MigrationStatuses.Keep;
                }));

            It should_have_one_person = () =>
            {
                using (var q = TestContext.Database.BeginSession())
                {
                    q.Query<Model1.Person>().Count().ShouldEqual(1);
                }
            };

            It should_have_updated_from_old_to_new_person = () =>
            {
                using (var q = TestContext.Database.BeginSession())
                {
                    var refetchedSalesPerson = q.GetById<Model1.Person>(_personId);
                    refetchedSalesPerson.Name.ShouldEqual("New name");
                    refetchedSalesPerson.Address.ShouldEqual("The Street 1\r\n12345\r\nThe City");
                }
            };

            private static Guid _personId;
        }

        [Subject(typeof(ISisoDatabaseMaintenance), "Migrate")]
        public class when_stored_as_person_and_migrated_to_same_type_of_person_with_more_fine_grained_properties_using_migration_with_anonymous : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();

                var orgItem = new Model1.Person { Name = "Daniel Wertheim", Address = "The Street 1\r\n12345\r\nThe City" };
                TestContext.Database.UseOnceTo().Insert(orgItem);
                _personId = orgItem.StructureId;
            };

            Because of = () =>
                TestContext.Database.Maintenance.Migrate(Migrate<Model1.Person>.To<Model1.Person>().Using(
                new
                {
                    StructureId = default(Guid),
                    Name = default(string),
                    Address = default(string)
                },
                (p, sp) =>
                {
                    sp.Name = "New name";

                    return MigrationStatuses.Keep;
                }));

            It should_have_one_person = () =>
            {
                using (var q = TestContext.Database.BeginSession())
                {
                    q.Query<Model1.Person>().Count().ShouldEqual(1);
                }
            };

            It should_have_updated_from_old_to_new_person = () =>
            {
                using (var q = TestContext.Database.BeginSession())
                {
                    var refetchedSalesPerson = q.GetById<Model1.Person>(_personId);
                    refetchedSalesPerson.Name.ShouldEqual("New name");
                    refetchedSalesPerson.Address.ShouldEqual("The Street 1\r\n12345\r\nThe City");
                }
            };

            private static Guid _personId;
        }

        [Subject(typeof(ISisoDatabaseMaintenance), "Migrate")]
        public class when_three_structures_with_identities_exists_and_trash_is_made_on_second_using_func : SpecificationBase
        {
            Establish context = () =>
            {
                var orgItem1 = new Model1.IdentityItemOld { String1 = "A" };
                var orgItem2 = new Model1.IdentityItemOld { String1 = "B" };
                var orgItem3 = new Model1.IdentityItemOld { String1 = "C" };

                var testContext = TestContextFactory.Create();
                testContext.Database.UseOnceTo().InsertMany(new[] { orgItem1, orgItem2, orgItem3 });
                
                _orgItem1Id = orgItem1.StructureId;
                _orgItem2Id = orgItem2.StructureId;
                _orgItem3Id = orgItem3.StructureId;

                TestContext = TestContextFactory.Create();
            };

            Because of = () =>
                TestContext.Database.Maintenance.Migrate<Model1.IdentityItemOld, Model2.IdentityItemNew>((oldItem, newItem) =>
                {
                    newItem.NewString1 = oldItem.String1;

                    if (oldItem.StructureId == _orgItem2Id)
                        return MigrationStatuses.Trash;

                    return MigrationStatuses.Keep;
                });

            It should_have_kept_and_updated_all_members_except_id_of_the_first_item = () =>
            {
                using (var q = TestContext.Database.BeginSession())
                {
                    var newItem1 = q.GetById<Model2.IdentityItemNew>(_orgItem1Id);
                    newItem1.ShouldNotBeNull();
                    newItem1.NewString1 = "A";
                }
            };

            It should_have_removed_the_second_item = () =>
            {
                using (var q = TestContext.Database.BeginSession())
                {
                    var newItem2 = q.GetById<Model2.IdentityItemNew>(_orgItem2Id);
                    newItem2.ShouldBeNull();
                }
            };

            It should_have_kept_and_updated_all_members_except_id_of_the_last_item = () =>
            {
                using (var q = TestContext.Database.BeginSession())
                {
                    var newItem3 = q.GetById<Model2.IdentityItemNew>(_orgItem3Id);
                    newItem3.ShouldNotBeNull();
                    newItem3.NewString1 = "C";
                }
            };

            private static int _orgItem1Id, _orgItem2Id, _orgItem3Id;
        }

        [Subject(typeof(ISisoDatabaseMaintenance), "Migrate")]
        public class when_three_structures_with_guids_exists_and_trash_is_made_on_second_using_func : SpecificationBase
        {
            Establish context = () =>
            {
                var orgItem1 = new Model1.GuidItemOld { String1 = "A" };
                var orgItem2 = new Model1.GuidItemOld { String1 = "B" };
                var orgItem3 = new Model1.GuidItemOld { String1 = "C" };

                var testContext = TestContextFactory.Create();
                using (var session = testContext.Database.BeginSession())
                {
                    session.InsertMany(new[] { orgItem1, orgItem2, orgItem3 });
                }

                _orgItem1Id = orgItem1.StructureId;
                _orgItem2Id = orgItem2.StructureId;
                _orgItem3Id = orgItem3.StructureId;

                TestContext = TestContextFactory.Create();
            };

            Because of = () =>
                TestContext.Database.Maintenance.Migrate<Model1.GuidItemOld, Model2.GuidItemNew>((oldItem, newItem) =>
                {
                    newItem.NewString1 = oldItem.String1;

                    if (oldItem.StructureId == _orgItem2Id)
                        return MigrationStatuses.Trash;

                    return MigrationStatuses.Keep;
                });

            It should_have_kept_and_updated_all_members_except_id_of_the_first_item = () =>
            {
                using (var q = TestContext.Database.BeginSession())
                {
                    var newItem1 = q.GetById<Model2.GuidItemNew>(_orgItem1Id);
                    newItem1.ShouldNotBeNull();
                    newItem1.NewString1 = "A";
                }
            };

            It should_have_removed_the_second_item = () =>
            {
                using (var q = TestContext.Database.BeginSession())
                {
                    var newItem2 = q.GetById<Model2.GuidItemNew>(_orgItem2Id);
                    newItem2.ShouldBeNull();
                }
            };

            It should_have_kept_and_updated_all_members_except_id_of_the_last_item = () =>
            {
                using (var q = TestContext.Database.BeginSession())
                {
                    var newItem3 = q.GetById<Model2.GuidItemNew>(_orgItem3Id);
                    newItem3.ShouldNotBeNull();
                    newItem3.NewString1 = "C";
                }
            };

            private static Guid _orgItem1Id, _orgItem2Id, _orgItem3Id;
        }

        [Subject(typeof(ISisoDatabaseMaintenance), "Migrate")]
        public class when_two_structures_exists_and_abort_is_made_on_last_using_func : SpecificationBase
        {
            Establish context = () =>
            {
                var orgItem1 = new Model1.GuidItemOld { Int1 = 142, String1 = "A" };
                var orgItem2 = new Model1.GuidItemOld { Int1 = 242, String1 = "B" };

                var testContext = TestContextFactory.Create();
                testContext.Database.UseOnceTo().InsertMany(new[] { orgItem1, orgItem2 });

                _orgItem1Id = orgItem1.StructureId;
                _orgItem2Id = orgItem2.StructureId;

                TestContext = TestContextFactory.Create();
            };

            Because of = () =>
                TestContext.Database.Maintenance.Migrate<Model1.GuidItemOld, Model2.GuidItemNew>((oldItem, newItem) =>
                {
                    newItem.NewString1 = "New" + oldItem.String1;

                    if (oldItem.StructureId == _orgItem2Id)
                        return MigrationStatuses.Abort;

                    return MigrationStatuses.Keep;
                });

            It should_have_kept_old_items_untouched = () =>
            {
                TestContext.Database.StructureSchemas.Clear();

                IList<Model1.GuidItemOld> items;

                using (var q = TestContext.Database.BeginSession())
                    items = q.Query<Model1.GuidItemOld>().ToList();

                items[0].StructureId.ShouldEqual(_orgItem1Id);
                items[0].Int1.ShouldEqual(142);
                items[0].String1.ShouldEqual("A");

                items[1].StructureId.ShouldEqual(_orgItem2Id);
                items[1].Int1.ShouldEqual(242);
                items[1].String1.ShouldEqual("B");
            };

            private static Guid _orgItem1Id, _orgItem2Id;
        }

        [Subject(typeof(ISisoDatabaseMaintenance), "Migrate")]
        public class when_the_second_of_two_new_structures_does_not_get_a_identity_id_using_func : SpecificationBase
        {
            Establish context = () =>
            {
                var orgItem1 = new Model1.IdentityItemOld { Int1 = 142, String1 = "A" };
                var orgItem2 = new Model1.IdentityItemOld { Int1 = 242, String1 = "B" };

                var testContext = TestContextFactory.Create();
                testContext.Database.UseOnceTo().InsertMany(new[] { orgItem1, orgItem2 });
                
                _orgItem1Id = orgItem1.StructureId;
                _orgItem2Id = orgItem2.StructureId;

                TestContext = TestContextFactory.Create();
            };

            Because of = () => CaughtException = Catch.Exception(() =>
                TestContext.Database.Maintenance.Migrate<Model1.IdentityItemOld, Model2.IdentityItemNew>((oldItem, newItem) =>
                {
                    if (oldItem.StructureId == _orgItem2Id)
                        newItem.StructureId = 0;

                    newItem.NewString1 = "New" + oldItem.String1;

                    return MigrationStatuses.Keep;
                })
            );

            It should_have_failed = () => CaughtException.ShouldNotBeNull();

            It should_have_descriptive_message = () =>
                CaughtException.Message.ShouldEqual(ExceptionMessages.StructureSetMigrator_NewIdDoesNotMatchOldId.Inject(0, _orgItem2Id));

            It should_have_kept_old_items_untouched = () =>
            {
                TestContext.Database.StructureSchemas.Clear();

                IList<Model1.IdentityItemOld> items;

                using (var q = TestContext.Database.BeginSession())
                {
                    items = q.Query<Model1.IdentityItemOld>().ToList();
                }

                items.Count.ShouldEqual(2);

                items[0].StructureId.ShouldEqual(_orgItem1Id);
                items[0].Int1.ShouldEqual(142);
                items[0].String1.ShouldEqual("A");

                items[1].StructureId.ShouldEqual(_orgItem2Id);
                items[1].Int1.ShouldEqual(242);
                items[1].String1.ShouldEqual("B");
            };

            private static int _orgItem1Id, _orgItem2Id;
        }

        [Subject(typeof(ISisoDatabaseMaintenance), "Migrate")]
        public class when_the_second_of_two_new_structures_does_not_get_a_guid_id_using_func : SpecificationBase
        {
            Establish context = () =>
            {
                var orgItem1 = new Model1.GuidItemOld { Int1 = 142, String1 = "A" };
                var orgItem2 = new Model1.GuidItemOld { Int1 = 242, String1 = "B" };

                var testContext = TestContextFactory.Create();
                testContext.Database.UseOnceTo().InsertMany(new[] { orgItem1, orgItem2 });

                _orgItem1Id = orgItem1.StructureId;
                _orgItem2Id = orgItem2.StructureId;

                TestContext = TestContextFactory.Create();
            };

            Because of = () => CaughtException = Catch.Exception(() =>
                TestContext.Database.Maintenance.Migrate<Model1.GuidItemOld, Model2.GuidItemNew>((oldItem, newItem) =>
                {
                    if (oldItem.StructureId == _orgItem2Id)
                        newItem.StructureId = Guid.Empty;

                    newItem.NewString1 = "New" + oldItem.String1;

                    return MigrationStatuses.Keep;
                })
            );

            It should_have_failed = () => CaughtException.ShouldNotBeNull();

            It should_have_descriptive_message = () =>
                CaughtException.Message.ShouldEqual(ExceptionMessages.StructureSetMigrator_NewIdDoesNotMatchOldId.Inject(Guid.Empty, _orgItem2Id));

            It should_have_kept_old_items_untouched = () =>
            {
                TestContext.Database.StructureSchemas.Clear();

                IList<Model1.GuidItemOld> items;

                using (var q = TestContext.Database.BeginSession())
                {
                    items = q.Query<Model1.GuidItemOld>().ToList();
                }

                items.Count.ShouldEqual(2);

                items[0].StructureId.ShouldEqual(_orgItem1Id);
                items[0].Int1.ShouldEqual(142);
                items[0].String1.ShouldEqual("A");

                items[1].StructureId.ShouldEqual(_orgItem2Id);
                items[1].Int1.ShouldEqual(242);
                items[1].String1.ShouldEqual("B");
            };

            private static Guid _orgItem1Id, _orgItem2Id;
        }

        [Subject(typeof(ISisoDatabaseMaintenance), "Migrate")]
        public class when_the_second_of_two_new_structures_does_get_a_new_identity_id_using_func : SpecificationBase
        {
            Establish context = () =>
            {
                var orgItem1 = new Model1.IdentityItemOld { Int1 = 142, String1 = "A" };
                var orgItem2 = new Model1.IdentityItemOld { Int1 = 242, String1 = "B" };

                var testContext = TestContextFactory.Create();
                testContext.Database.UseOnceTo().InsertMany(new[] { orgItem1, orgItem2 });

                _orgItem1Id = orgItem1.StructureId;
                _orgItem2Id = orgItem2.StructureId;
                _newItem2Id = _orgItem2Id + 1;

                TestContext = TestContextFactory.Create();
            };

            Because of = () => CaughtException = Catch.Exception(() =>
                TestContext.Database.Maintenance.Migrate<Model1.IdentityItemOld, Model2.IdentityItemNew>((oldItem, newItem) =>
                {
                    if (oldItem.StructureId == _orgItem2Id)
                        newItem.StructureId = _newItem2Id;

                    newItem.NewString1 = "New" + oldItem.String1;

                    return MigrationStatuses.Keep;
                }));

            It should_have_failed = () => CaughtException.ShouldNotBeNull();

            It should_have_descriptive_message = () =>
                CaughtException.Message.ShouldEqual(ExceptionMessages.StructureSetMigrator_NewIdDoesNotMatchOldId.Inject(_newItem2Id, _orgItem2Id));

            It should_have_kept_old_items_untouched = () =>
            {
                TestContext.Database.StructureSchemas.Clear();

                IList<Model1.IdentityItemOld> items;

                using (var q = TestContext.Database.BeginSession())
                {
                    items = q.Query<Model1.IdentityItemOld>().ToList();
                }

                items.Count.ShouldEqual(2);

                items[0].StructureId.ShouldEqual(_orgItem1Id);
                items[0].Int1.ShouldEqual(142);
                items[0].String1.ShouldEqual("A");

                items[1].StructureId.ShouldEqual(_orgItem2Id);
                items[1].Int1.ShouldEqual(242);
                items[1].String1.ShouldEqual("B");
            };

            private static int _orgItem1Id, _orgItem2Id;
            private static int _newItem2Id;
        }

        [Subject(typeof(ISisoDatabaseMaintenance), "Migrate")]
        public class when_the_second_of_two_new_structures_does_get_a_new_guid_id_using_func : SpecificationBase
        {
            Establish context = () =>
            {
                var orgItem1 = new Model1.GuidItemOld { Int1 = 142, String1 = "A" };
                var orgItem2 = new Model1.GuidItemOld { Int1 = 242, String1 = "B" };

                var testContext = TestContextFactory.Create();
                using (var session = testContext.Database.BeginSession())
                {
                    session.InsertMany(new[] { orgItem1, orgItem2 });
                }

                _orgItem1Id = orgItem1.StructureId;
                _orgItem2Id = orgItem2.StructureId;
                _newItem2Id = Guid.Parse("ED563B06-7D30-4136-BE05-B12A0D5B9798");

                TestContext = TestContextFactory.Create();
            };

            Because of = () => CaughtException = Catch.Exception(() =>
                TestContext.Database.Maintenance.Migrate<Model1.GuidItemOld, Model2.GuidItemNew>((oldItem, newItem) =>
                {
                    if (oldItem.StructureId == _orgItem2Id)
                        newItem.StructureId = _newItem2Id;

                    newItem.NewString1 = "New" + oldItem.String1;

                    return MigrationStatuses.Keep;
                }));

            It should_have_failed = () => CaughtException.ShouldNotBeNull();

            It should_have_descriptive_message = () =>
                CaughtException.Message.ShouldEqual(ExceptionMessages.StructureSetMigrator_NewIdDoesNotMatchOldId.Inject(_newItem2Id, _orgItem2Id));

            It should_have_kept_old_items_untouched = () =>
            {
                TestContext.Database.StructureSchemas.Clear();

                IList<Model1.GuidItemOld> items;

                using (var q = TestContext.Database.BeginSession())
                {
                    items = q.Query<Model1.GuidItemOld>().ToList();
                }

                items.Count.ShouldEqual(2);

                items[0].StructureId.ShouldEqual(_orgItem1Id);
                items[0].Int1.ShouldEqual(142);
                items[0].String1.ShouldEqual("A");

                items[1].StructureId.ShouldEqual(_orgItem2Id);
                items[1].Int1.ShouldEqual(242);
                items[1].String1.ShouldEqual("B");
            };

            private static Guid _orgItem1Id, _orgItem2Id;
            private static Guid _newItem2Id;
        }
        
        class Model1
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

            public class IdentityItemOld
            {
                public int StructureId { get; set; }
                public string String1 { get; set; }
                public int Int1 { get; set; }
            }

            public class GuidItemOld
            {
                public Guid StructureId { get; set; }
                public string String1 { get; set; }
                public int Int1 { get; set; }
            }
		}

	    class Model2
	    {
            public class Person
            {
                public Guid StructureId { get; set; }
                public string Firstname { get; set; }
                public string Lastname { get; set; }
                public Address Office { get; set; }

                public Person()
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

            public class IdentityItemNew
            {
                public int StructureId { get; set; }
                public string NewString1 { get; set; }
            }

            public class GuidItemNew
            {
                public Guid StructureId { get; set; }
                public string NewString1 { get; set; }
            }
	    }
	}
}