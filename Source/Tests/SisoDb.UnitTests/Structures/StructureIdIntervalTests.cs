using NUnit.Framework;
using SisoDb.Structures;

namespace SisoDb.UnitTests.Structures
{
	[TestFixture]
	public class StructureIdIntervalTests : UnitTestBase
	{
		[Test]
		public void DefaultCtor_From_IsNull()
		{
			var interval = new StructureIdInterval();

			Assert.IsNull(interval.From);
		}

		[Test]
		public void DefaultCtor_To_IsNull()
		{
			var interval = new StructureIdInterval();

			Assert.IsNull(interval.To);
		}

		[Test]
		public void DefaultCtor_IsComplete_IsFalse()
		{
			var interval = new StructureIdInterval();

			Assert.IsFalse(interval.IsComplete);
		}

		[Test]
		public void Clear_WhenFromAndToHasValue_BothBecomesNull()
		{
			var interval = new StructureIdInterval();
			interval.Set(StructureId.Create(1));
			interval.Set(StructureId.Create(2));

			interval.Clear();

			Assert.IsNull(interval.From);
			Assert.IsNull(interval.To);
		}

		[Test]
		public void Clear_WhenFromAndToHasValue_IsCompleteReturnsFalse()
		{
			var interval = new StructureIdInterval();
			interval.Set(StructureId.Create(1));
			interval.Set(StructureId.Create(2));

			interval.Clear();

			Assert.IsFalse(interval.IsComplete);
		}

		[Test]
		public void Set_WhenFromIsNull_FromBecomesNewId()
		{
			var from = StructureId.Create(1);
			var interval = new StructureIdInterval();

			interval.Set(from);

			Assert.AreEqual(from, interval.From);
		}

		[Test]
		public void Set_WhenFromIsNotNullButToIs_FromRemainsUnchanged()
		{
			var from = StructureId.Create(1);
			var to = StructureId.Create(2);
			var interval = new StructureIdInterval();
			interval.Set(from);

			interval.Set(to);

			Assert.AreEqual(from, interval.From);
		}

		[Test]
		public void Set_WhenFromIsNotNullButToIs_ToGainsNewId()
		{
			var from = StructureId.Create(1);
			var to = StructureId.Create(2);
			var interval = new StructureIdInterval();
			interval.Set(from);

			interval.Set(to);

			Assert.AreEqual(to, interval.To);
		}
		
		[Test]
		public void Set_WhenFromAndToHasValue_ToGainsNewId()
		{
			var from = StructureId.Create(1);
			var to = StructureId.Create(2);
			var newTo = StructureId.Create(3);
			var interval = new StructureIdInterval();
			interval.Set(from);
			interval.Set(to);

			interval.Set(newTo);

			Assert.AreEqual(newTo, interval.To);
		}
	}
}