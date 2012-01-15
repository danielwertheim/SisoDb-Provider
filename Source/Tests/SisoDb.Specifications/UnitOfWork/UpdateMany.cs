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
		public class when_the_second_of_two_new_structures_does_not_get_an_identity_id : SpecificationBase
		{
			Establish context = () =>
			{
				var orgItem1 = new ItemForPropChange { Int1 = 142, String1 = "A" };
				var orgItem2 = new ItemForPropChange { Int1 = 242, String1 = "B" };

				var testContext = TestContextFactory.Create();
				using (var session = testContext.Database.BeginWriteSession())
					session.InsertMany(new[] { orgItem1, orgItem2 });

				_orgItem1Id = orgItem1.StructureId;
				_orgItem2Id = orgItem2.StructureId;

				TestContext = TestContextFactory.Create();
			};

			Because of = () => CaughtException = Catch.Exception(() =>
			{
				using (var session = TestContext.Database.BeginWriteSession())
				{
					session.UpdateMany<ItemForPropChange>(
						i => i.StructureId == _orgItem1Id || i.StructureId == _orgItem2Id,
						item =>
						{
							if (item.StructureId == _orgItem2Id)
								item.StructureId = 0;

							item.String1 = "New" + item.String1;
						});
				}
			});

			It should_have_failed = () => CaughtException.ShouldNotBeNull();

			It should_have_descriptive_message = () =>
				CaughtException.Message.ShouldEqual(ExceptionMessages.WriteSession_UpdateMany_NewIdDoesNotMatchOldId.Inject(0, _orgItem2Id));

			It should_have_kept_old_items_untouched = () =>
			{
				TestContext.Database.StructureSchemas.Clear();

				IList<ItemForPropChange> items;

				using (var q = TestContext.Database.BeginReadSession())
				{
					items = q.Query<ItemForPropChange>().ToList();
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

		[Subject(typeof(IWriteSession), "Update many")]
		public class when_the_second_of_two_new_structures_does_not_get_a_guid_id : SpecificationBase
		{
			Establish context = () =>
			{
				var orgItem1 = new GuidItemForPropChange { Int1 = 142, String1 = "A" };
				var orgItem2 = new GuidItemForPropChange { Int1 = 242, String1 = "B" };

				var testContext = TestContextFactory.Create();
				using (var session = testContext.Database.BeginWriteSession())
				{
					session.InsertMany(new[] { orgItem1, orgItem2 });
				}

				_orgItem1Id = orgItem1.StructureId;
				_orgItem2Id = orgItem2.StructureId;

				TestContext = TestContextFactory.Create();
			};

			Because of = () => CaughtException = Catch.Exception(() =>
			{
				using (var session = TestContext.Database.BeginWriteSession())
				{
					session.UpdateMany<GuidItemForPropChange>(
						i => i.StructureId == _orgItem1Id || i.StructureId == _orgItem2Id,
						item =>
						{
							if (item.StructureId == _orgItem2Id)
								item.StructureId = Guid.Empty;

							item.String1 = "New" + item.String1;
						});
				}
			});

			It should_have_failed = () => CaughtException.ShouldNotBeNull();

			It should_have_descriptive_message = () =>
				CaughtException.Message.ShouldEqual(ExceptionMessages.WriteSession_UpdateMany_NewIdDoesNotMatchOldId.Inject(Guid.Empty, _orgItem2Id));

			It should_have_kept_old_items_untouched = () =>
			{
				TestContext.Database.StructureSchemas.Clear();

				IList<GuidItemForPropChange> items;

				using (var q = TestContext.Database.BeginReadSession())
				{
					items = q.Query<GuidItemForPropChange>().ToList();
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

		[Subject(typeof(IWriteSession), "Update many")]
		public class when_the_second_of_two_new_structures_does_get_a_new_identity_id : SpecificationBase
		{
			Establish context = () =>
			{
				var orgItem1 = new ItemForPropChange { Int1 = 142, String1 = "A" };
				var orgItem2 = new ItemForPropChange { Int1 = 242, String1 = "B" };

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

			Because of = () => CaughtException = Catch.Exception(() =>
			{
				using (var session = TestContext.Database.BeginWriteSession())
				{
					session.UpdateMany<ItemForPropChange>(
						i => i.StructureId == _orgItem1Id || i.StructureId == _orgItem2Id,
						item =>
						{
							if (item.StructureId == _orgItem2Id)
								item.StructureId = _newItem2Id;

							item.String1 = "New" + item.String1;
						});
				}
			});

			It should_have_failed = () => CaughtException.ShouldNotBeNull();

			It should_have_descriptive_message = () =>
				CaughtException.Message.ShouldEqual(ExceptionMessages.WriteSession_UpdateMany_NewIdDoesNotMatchOldId.Inject(_newItem2Id, _orgItem2Id));

			It should_have_kept_old_items_untouched = () =>
			{
				TestContext.Database.StructureSchemas.Clear();

				IList<ItemForPropChange> items;

				using (var q = TestContext.Database.BeginReadSession())
				{
					items = q.Query<ItemForPropChange>().ToList();
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

		[Subject(typeof(IWriteSession), "Update many")]
		public class when_the_second_of_two_new_structures_does_get_a_new_guid_id : SpecificationBase
		{
			Establish context = () =>
			{
				var orgItem1 = new GuidItemForPropChange { Int1 = 142, String1 = "A" };
				var orgItem2 = new GuidItemForPropChange { Int1 = 242, String1 = "B" };

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

			Because of = () => CaughtException = Catch.Exception(() =>
			{
				using (var session = TestContext.Database.BeginWriteSession())
				{
					session.UpdateMany<GuidItemForPropChange>(
						i => i.StructureId == _orgItem1Id || i.StructureId == _orgItem2Id,
						item =>
						{
							if (item.StructureId == _orgItem2Id)
								item.StructureId = _newItem2Id;

							item.String1 = "New" + item.String1;
						});
				}
			});

			It should_have_failed = () => CaughtException.ShouldNotBeNull();

			It should_have_descriptive_message = () =>
				CaughtException.Message.ShouldEqual(ExceptionMessages.WriteSession_UpdateMany_NewIdDoesNotMatchOldId.Inject(_newItem2Id, _orgItem2Id));

			It should_have_kept_old_items_untouched = () =>
			{
				TestContext.Database.StructureSchemas.Clear();

				IList<GuidItemForPropChange> items;

				using (var q = TestContext.Database.BeginReadSession())
				{
					items = q.Query<GuidItemForPropChange>().ToList();
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

		[Subject(typeof(IWriteSession), "Update many")]
		public class when_all_identity_items_are_updated : SpecificationBase
		{
			Establish context = () =>
			{
				var orgItem1 = new ItemForPropChange { Int1 = 142, String1 = "A" };
				var orgItem2 = new ItemForPropChange { Int1 = 242, String1 = "B" };

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
					session.UpdateMany<ItemForPropChange>(
						i => i.StructureId == _orgItem1Id || i.StructureId == _orgItem2Id,
						item =>
						{
							item.String1 = "New" + item.String1;
							item.Int1 *= 10;
						});
				}
			};

			It should_have_updated_all_items = () =>
			{
				TestContext.Database.StructureSchemas.Clear();

				IList<ItemForPropChange> items;

				using (var q = TestContext.Database.BeginReadSession())
				{
					items = q.Query<ItemForPropChange>().ToList();
				}

				items.Count.ShouldEqual(2);

				items[0].StructureId.ShouldEqual(_orgItem1Id);
				items[0].String1.ShouldEqual("NewA");

				items[1].StructureId.ShouldEqual(_orgItem2Id);
				items[1].String1.ShouldEqual("NewB");
			};

			private static int _orgItem1Id, _orgItem2Id;
			private static int _newItem2Id;
		}

		[Subject(typeof(IWriteSession), "Update many")]
		public class when_all_guid_items_are_updated : SpecificationBase
		{
			Establish context = () =>
			{
				var orgItem1 = new GuidItemForPropChange { Int1 = 142, String1 = "A" };
				var orgItem2 = new GuidItemForPropChange { Int1 = 242, String1 = "B" };

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
					session.UpdateMany<GuidItemForPropChange>(
						i => i.StructureId == _orgItem1Id || i.StructureId == _orgItem2Id,
						item =>
						{
							item.String1 = "New" + item.String1;
							item.Int1 *= 10;
						});
				}
			};

			It should_have_updated_all_items = () =>
			{
				TestContext.Database.StructureSchemas.Clear();

				IList<GuidItemForPropChange> items;

				using (var q = TestContext.Database.BeginReadSession())
				{
					items = q.Query<GuidItemForPropChange>().ToList();
				}

				items.Count.ShouldEqual(2);

				items[0].StructureId.ShouldEqual(_orgItem1Id);
				items[0].String1.ShouldEqual("NewA");
				items[0].Int1.ShouldEqual(1420);

				items[1].StructureId.ShouldEqual(_orgItem2Id);
				items[1].String1.ShouldEqual("NewB");
				items[1].Int1.ShouldEqual(2420);
			};

			private static Guid _orgItem1Id, _orgItem2Id;
			private static Guid _newItem2Id;
		}

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
}