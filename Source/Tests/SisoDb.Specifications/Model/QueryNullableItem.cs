using System;

namespace SisoDb.Specifications.Model
{
	public class QueryNullableItem
	{
		public Guid StructureId { get; set; }

		public bool BoolValue { get; set; }

		public int? NullableInt { get; set; }

        public DateTime? NullableDateTime { get; set; }

        public Decimal? NullableDecimal { get; set; }

		public string StringValue { get; set; }
	}
}