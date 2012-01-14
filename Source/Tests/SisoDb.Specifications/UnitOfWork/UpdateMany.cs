using System;
using System.Collections.Generic;
using Machine.Specifications;
using NCore;
using SisoDb.Resources;
using SisoDb.Testing;

namespace SisoDb.Specifications.UnitOfWork
{
	class UpdateMany
	{
		[Subject(typeof(IWriteSession), "Update many")]
		public class when_stored_as_person_and_transformed_to_sales_person_with_more_fine_grained_properties : SpecificationBase
		{
			Establish context = () =>
			{
				TestContext = TestContextFactory.Create();

				var orgItem = new ModelComplexUpdates.Person { Name = "Daniel Wertheim", Address = "The Street 1\r\n12345\r\nThe City" };
				using (var session = TestContext.Database.BeginWriteSession())
				{
					session.Insert(orgItem);
				}

				_personId = orgItem.StructureId;
			};

			Because of = () =>
			{
				using (var session = TestContext.Database.BeginWriteSession())
				{
					session.UpdateMany<ModelComplexUpdates.Person, ModelComplexUpdates.SalesPerson>((p, sp) =>
					{
						var names = p.Name.Split(' ');
						sp.Firstname = names[0];
						sp.Lastname = names[1];

						var address = p.Address.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
						sp.Office.Street = address[0];
						sp.Office.Zip = address[1];
						sp.Office.City = address[2];

						return UpdateManyModifierStatus.Keep;
					});
				}
			};

			It should_have_removed_the_person = () =>
			{
				using (var q = TestContext.Database.BeginReadSession())
				{
					q.Query<ModelComplexUpdates.Person>().Count().ShouldEqual(0);
				}
			};

			It should_have_created_a_salesperson_from_the_person = () =>
			{
				using (var q = TestContext.Database.BeginReadSession())
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

		[Subject(typeof(IWriteSession), "Update many")]
		public class when_three_structures_with_identities_exists_and_trash_is_made_on_second : SpecificationBase
		{
			Establish context = () =>
			{
				var orgItem1 = new ModelOld.ItemForPropChange { String1 = "A" };
				var orgItem2 = new ModelOld.ItemForPropChange { String1 = "B" };
				var orgItem3 = new ModelOld.ItemForPropChange { String1 = "C" };

				var testContext = TestContextFactory.Create();
				using (var session = testContext.Database.BeginWriteSession())
				{
					session.InsertMany(new[] { orgItem1, orgItem2, orgItem3 });
				}

				_orgItem1Id = orgItem1.StructureId;
				_orgItem2Id = orgItem2.StructureId;
				_orgItem3Id = orgItem3.StructureId;

				TestContext = TestContextFactory.Create();
			};

			Because of = () =>
			{
				using (var session = TestContext.Database.BeginWriteSession())
				{
					session.UpdateMany<ModelOld.ItemForPropChange, ModelNew.ItemForPropChange>((oldItem, newItem) =>
					{
						newItem.NewString1 = oldItem.String1;

						if (oldItem.StructureId == _orgItem2Id)
							return UpdateManyModifierStatus.Trash;

						return UpdateManyModifierStatus.Keep;
					});
				}
			};

			It should_have_kept_and_updated_all_members_except_id_of_the_first_item = () =>
			{
				using (var q = TestContext.Database.BeginReadSession())
				{
					var newItem1 = q.GetById<ModelNew.ItemForPropChange>(_orgItem1Id);
					newItem1.ShouldNotBeNull();
					newItem1.NewString1 = "A";
				}
			};

			It should_have_removed_the_second_item = () =>
			{
				using (var q = TestContext.Database.BeginReadSession())
				{
					var newItem2 = q.GetById<ModelNew.ItemForPropChange>(_orgItem2Id);
					newItem2.ShouldBeNull();
				}
			};

			It should_have_kept_and_updated_all_members_except_id_of_the_last_item = () =>
			{
				using (var q = TestContext.Database.BeginReadSession())
				{
					var newItem3 = q.GetById<ModelNew.ItemForPropChange>(_orgItem3Id);
					newItem3.ShouldNotBeNull();
					newItem3.NewString1 = "C";
				}
			};

			private static int _orgItem1Id, _orgItem2Id, _orgItem3Id;
		}

		[Subject(typeof(IWriteSession), "Update many")]
		public class when_three_structures_with_guids_exists_and_trash_is_made_on_second : SpecificationBase
		{
			Establish context = () =>
			{
				var orgItem1 = new ModelOld.GuidItemForPropChange { String1 = "A" };
				var orgItem2 = new ModelOld.GuidItemForPropChange { String1 = "B" };
				var orgItem3 = new ModelOld.GuidItemForPropChange { String1 = "C" };

				var testContext = TestContextFactory.Create();
				using (var session = testContext.Database.BeginWriteSession())
				{
					session.InsertMany(new[] { orgItem1, orgItem2, orgItem3 });
				}

				_orgItem1Id = orgItem1.StructureId;
				_orgItem2Id = orgItem2.StructureId;
				_orgItem3Id = orgItem3.StructureId;

				TestContext = TestContextFactory.Create();
			};

			Because of = () =>
			{
				using (var session = TestContext.Database.BeginWriteSession())
				{
					session.UpdateMany<ModelOld.GuidItemForPropChange, ModelNew.GuidItemForPropChange>((oldItem, newItem) =>
					{
						newItem.NewString1 = oldItem.String1;

						if (oldItem.StructureId == _orgItem2Id)
							return UpdateManyModifierStatus.Trash;

						return UpdateManyModifierStatus.Keep;
					});
				}
			};

			It should_have_kept_and_updated_all_members_except_id_of_the_first_item = () =>
			{
				using (var q = TestContext.Database.BeginReadSession())
				{
					var newItem1 = q.GetById<ModelNew.GuidItemForPropChange>(_orgItem1Id);
					newItem1.ShouldNotBeNull();
					newItem1.NewString1 = "A";
				}
			};

			It should_have_removed_the_second_item = () =>
			{
				using (var q = TestContext.Database.BeginReadSession())
				{
					var newItem2 = q.GetById<ModelNew.GuidItemForPropChange>(_orgItem2Id);
					newItem2.ShouldBeNull();
				}
			};

			It should_have_kept_and_updated_all_members_except_id_of_the_last_item = () =>
			{
				using (var q = TestContext.Database.BeginReadSession())
				{
					var newItem3 = q.GetById<ModelNew.GuidItemForPropChange>(_orgItem3Id);
					newItem3.ShouldNotBeNull();
					newItem3.NewString1 = "C";
				}
			};

			private static Guid _orgItem1Id, _orgItem2Id, _orgItem3Id;
		}

		[Subject(typeof(IWriteSession), "Update many")]
		public class when_the_second_of_two_new_structures_does_not_get_a_identity_id : SpecificationBase
		{
			Establish context = () =>
			{
				var orgItem1 = new ModelOld.ItemForPropChange { Int1 = 142, String1 = "A" };
				var orgItem2 = new ModelOld.ItemForPropChange { Int1 = 242, String1 = "B" };

				var testContext = TestContextFactory.Create();
				using (var session = testContext.Database.BeginWriteSession())
				{
					session.InsertMany(new[] { orgItem1, orgItem2 });
				}

				_orgItem1Id = orgItem1.StructureId;
				_orgItem2Id = orgItem2.StructureId;
				
				TestContext = TestContextFactory.Create();
			};

			Because of = () =>
			{
				using (var session = TestContext.Database.BeginWriteSession())
				{
					session.UpdateMany<ModelOld.ItemForPropChange, ModelNew.ItemForPropChange>((oldItem, newItem) =>
					{
						if (oldItem.StructureId == _orgItem2Id)
							newItem.StructureId = 0;

						newItem.NewString1 = "New" + oldItem.String1;

						return UpdateManyModifierStatus.Keep;
					});
				}
			};

			It should_have_updated_all_items_and_allowed_new_id = () =>
			{
				TestContext.Database.StructureSchemas.Clear();

				IList<ModelNew.ItemForPropChange> items;

				using (var q = TestContext.Database.BeginReadSession())
				{
					items = q.Query<ModelNew.ItemForPropChange>().ToList();
				}

				items.Count.ShouldEqual(2);

				items[0].StructureId.ShouldEqual(0);
				items[0].NewString1.ShouldEqual("NewB");

				items[1].StructureId.ShouldEqual(_orgItem1Id);
				items[1].NewString1.ShouldEqual("NewA");
			};

			private static int _orgItem1Id, _orgItem2Id;
		}

		[Subject(typeof(IWriteSession), "Update many")]
		public class when_the_second_of_two_new_structures_does_not_get_a_guid_id : SpecificationBase
		{
			Establish context = () =>
			{
				var orgItem1 = new ModelOld.GuidItemForPropChange { Int1 = 142, String1 = "A" };
				var orgItem2 = new ModelOld.GuidItemForPropChange { Int1 = 242, String1 = "B" };

				var testContext = TestContextFactory.Create();
				using (var session = testContext.Database.BeginWriteSession())
				{
					session.InsertMany(new[] { orgItem1, orgItem2 });
				}

				_orgItem1Id = orgItem1.StructureId;
				_orgItem2Id = orgItem2.StructureId;

				TestContext = TestContextFactory.Create();
			};

			Because of = () =>
			{
				using (var session = TestContext.Database.BeginWriteSession())
				{
					session.UpdateMany<ModelOld.GuidItemForPropChange, ModelNew.GuidItemForPropChange>((oldItem, newItem) =>
					{
						if (oldItem.StructureId == _orgItem2Id)
							newItem.StructureId = Guid.Empty;

						newItem.NewString1 = "New" + oldItem.String1;

						return UpdateManyModifierStatus.Keep;
					});
				}
			};

			It should_have_updated_all_items_and_allowed_new_id = () =>
			{
				TestContext.Database.StructureSchemas.Clear();

				IList<ModelNew.GuidItemForPropChange> items;

				using (var q = TestContext.Database.BeginReadSession())
				{
					items = q.Query<ModelNew.GuidItemForPropChange>().ToList();
				}

				items.Count.ShouldEqual(2);

				items[0].StructureId.ShouldEqual(Guid.Empty);
				items[0].NewString1.ShouldEqual("NewB");

				items[1].StructureId.ShouldEqual(_orgItem1Id);
				items[1].NewString1.ShouldEqual("NewA");
			};

			private static Guid _orgItem1Id, _orgItem2Id;
		}

		[Subject(typeof(IWriteSession), "Update many")]
		public class when_the_second_of_two_new_structures_does_get_a_new_identity_id : SpecificationBase
		{
			Establish context = () =>
			{
				var orgItem1 = new ModelOld.ItemForPropChange { Int1 = 142, String1 = "A" };
				var orgItem2 = new ModelOld.ItemForPropChange { Int1 = 242, String1 = "B" };

				var testContext = TestContextFactory.Create();
				using (var session = testContext.Database.BeginWriteSession())
				{
					session.InsertMany(new[] { orgItem1, orgItem2 });
				}

				_orgItem1Id = orgItem1.StructureId;
				_orgItem2Id = orgItem2.StructureId;
				_newItem2Id = _orgItem2Id + 1;

				TestContext = TestContextFactory.Create();
			};

			Because of = () =>
			{
				using (var session = TestContext.Database.BeginWriteSession())
				{
					session.UpdateMany<ModelOld.ItemForPropChange, ModelNew.ItemForPropChange>((oldItem, newItem) =>
					{
						if (oldItem.StructureId == _orgItem2Id)
							newItem.StructureId = _newItem2Id;

						newItem.NewString1 = "New" + oldItem.String1;

						return UpdateManyModifierStatus.Keep;
					});
				}
			};

			It should_have_updated_all_items_and_allowed_new_id = () =>
			{
				TestContext.Database.StructureSchemas.Clear();

				IList<ModelNew.ItemForPropChange> items;

				using (var q = TestContext.Database.BeginReadSession())
				{
					items = q.Query<ModelNew.ItemForPropChange>().ToList();
				}

				items.Count.ShouldEqual(2);

				items[0].StructureId.ShouldEqual(_orgItem1Id);
				items[0].NewString1.ShouldEqual("NewA");

				items[1].StructureId.ShouldEqual(_newItem2Id);
				items[1].NewString1.ShouldEqual("NewB");
			};

			private static int _orgItem1Id, _orgItem2Id;
			private static int _newItem2Id;
		}

		[Subject(typeof(IWriteSession), "Update many")]
		public class when_the_second_of_two_new_structures_does_get_a_new_guid_id : SpecificationBase
		{
			Establish context = () =>
			{
				var orgItem1 = new ModelOld.GuidItemForPropChange { Int1 = 142, String1 = "A" };
				var orgItem2 = new ModelOld.GuidItemForPropChange { Int1 = 242, String1 = "B" };

				var testContext = TestContextFactory.Create();
				using (var session = testContext.Database.BeginWriteSession())
				{
					session.InsertMany(new[] { orgItem1, orgItem2 });
				}

				_orgItem1Id = orgItem1.StructureId;
				_orgItem2Id = orgItem2.StructureId;
				_newItem2Id = Guid.Parse("ED563B06-7D30-4136-BE05-B12A0D5B9798");

				TestContext = TestContextFactory.Create();
			};

			Because of = () =>
			{
				using (var session = TestContext.Database.BeginWriteSession())
				{
					session.UpdateMany<ModelOld.GuidItemForPropChange, ModelNew.GuidItemForPropChange>((oldItem, newItem) =>
					{
						if (oldItem.StructureId == _orgItem2Id)
							newItem.StructureId = _newItem2Id;

						newItem.NewString1 = "New" + oldItem.String1;

						return UpdateManyModifierStatus.Keep;
					});
				}
			};

			It should_have_updated_all_items_and_allowed_new_id = () =>
			{
				TestContext.Database.StructureSchemas.Clear();

				IList<ModelNew.GuidItemForPropChange> items;

				using (var q = TestContext.Database.BeginReadSession())
				{
					items = q.Query<ModelNew.GuidItemForPropChange>().ToList();
				}

				items.Count.ShouldEqual(2);

				items[0].StructureId.ShouldEqual(_orgItem1Id);
				items[0].NewString1.ShouldEqual("NewA");

				items[1].StructureId.ShouldEqual(_newItem2Id);
				items[1].NewString1.ShouldEqual("NewB");
			};

			private static Guid _orgItem1Id, _orgItem2Id;
			private static Guid _newItem2Id;
		}

		class ModelComplexUpdates
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

		class ModelOld
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

		class ModelNew
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