using System;
using System.Collections.Generic;
using Machine.Specifications;
using SisoDb.Testing;

namespace SisoDb.Specifications.UnitOfWork
{
	class InsertsWithCustomStructureId
	{
		[Subject(typeof(IWriteSession), "Insert (custom structure id)")]
		public class when_structure_has_structure_id_with_name_matching_interface_and_query_targets_first_and_last_of_four_items : SpecificationBase
		{
			Establish context = () =>
			{
				TestContext = TestContextFactory.Create();
				_structures = TestContext.Database.WriteOnce().InsertMany<IEvent>(new[]
				{
					new MyEvent{IntValue = 1},
					new MyEvent{IntValue = 2},
					new MyEvent{IntValue = 3},
					new MyEvent{IntValue = 4}
				});
			};

			Because of = () =>
			{
				_fetchedStructures =
					TestContext.Database.ReadOnce().Query<IEvent>().Where(i =>
						i.EventId == _structures[0].EventId ||
						i.EventId == _structures[3].EventId).ToListOf<MyEvent>();
			};

			It should_have_fetched_two_structures =
				() => _fetchedStructures.Count.ShouldEqual(2);

			It should_have_fetched_the_first_and_last_structure = () =>
			{
				_fetchedStructures[0].ShouldBeValueEqualTo((MyEvent)_structures[0]);
				_fetchedStructures[1].ShouldBeValueEqualTo((MyEvent)_structures[3]);
			};

			private static IList<IEvent> _structures;
			private static IList<MyEvent> _fetchedStructures;
		}

		[Subject(typeof(IWriteSession), "Insert (custom structure id)")]
		public class when_structure_has_type_named_structure_id_and_query_targets_first_and_last_of_four_items : SpecificationBase
		{
			Establish context = () =>
			{
				TestContext = TestContextFactory.Create();
				_structures = TestContext.Database.WriteOnce().InsertMany(new[]
				{
					new CustomAlpha{IntValue = 1},
					new CustomAlpha{IntValue = 2},
					new CustomAlpha{IntValue = 3},
					new CustomAlpha{IntValue = 4}
				});
			};

			Because of = () =>
			{
				_fetchedStructures =
					TestContext.Database.ReadOnce().Query<CustomAlpha>().Where(i => 
						i.CustomAlphaId == _structures[0].CustomAlphaId ||
						i.CustomAlphaId == _structures[3].CustomAlphaId).ToList();
			};

			It should_have_fetched_two_structures =
				() => _fetchedStructures.Count.ShouldEqual(2);

			It should_have_fetched_the_first_and_last_structure = () =>
			{
				_fetchedStructures[0].ShouldBeValueEqualTo(_structures[0]);
				_fetchedStructures[1].ShouldBeValueEqualTo(_structures[3]);
			};

			private static IList<CustomAlpha> _structures;
			private static IList<CustomAlpha> _fetchedStructures;
		}

		[Subject(typeof(IWriteSession), "Insert (custom structure id)")]
		public class when_structure_has_id_named_structure_id_and_query_targets_first_and_last_of_four_items : SpecificationBase
		{
			Establish context = () =>
			{
				TestContext = TestContextFactory.Create();
				_structures = TestContext.Database.WriteOnce().InsertMany(new[]
				{
					new CustomBeta{IntValue = 1},
					new CustomBeta{IntValue = 2},
					new CustomBeta{IntValue = 3},
					new CustomBeta{IntValue = 4}
				});
			};

			Because of = () =>
			{
				_fetchedStructures =
					TestContext.Database.ReadOnce().Query<CustomBeta>().Where(i =>
						i.Id == _structures[0].Id ||
						i.Id == _structures[3].Id).ToList();
			};

			It should_have_fetched_two_structures =
				() => _fetchedStructures.Count.ShouldEqual(2);

			It should_have_fetched_the_first_and_last_structure = () =>
			{
				_fetchedStructures[0].ShouldBeValueEqualTo(_structures[0]);
				_fetchedStructures[1].ShouldBeValueEqualTo(_structures[3]);
			};

			private static IList<CustomBeta> _structures;
			private static IList<CustomBeta> _fetchedStructures;
		}

		public class CustomAlpha
		{
			public Guid CustomAlphaId { get; set; }

			public int IntValue { get; set; }
		}

		public class CustomBeta
		{
			public Guid Id { get; set; }

			public int IntValue { get; set; }
		}

		public interface IEvent
		{
			Guid EventId { get; set; }
		}

		public class MyEvent : IEvent
		{
			public Guid EventId { get; set; }

			public int IntValue { get; set; }
		}
	}
}