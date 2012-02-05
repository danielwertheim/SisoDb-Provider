using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;
using SisoDb.Resources;
using SisoDb.Specifications.Model;
using SisoDb.Testing;

namespace SisoDb.Specifications.UnitOfWork.Queries
{
	class GetByIdInterval
	{
		[Subject(typeof(ISession), "Get by Id interval")]
		public class when_getting_for_guids : SpecificationBase
		{
			Establish context = () =>
			{
				TestContext = TestContextFactory.Create();
				_structures = QueryGuidItem.CreateFourItems<QueryGuidItem>();
			};

			Because of = () =>
			{
				CaughtException = Catch.Exception(() =>
				{
					using (var session = TestContext.Database.BeginSession())
					{
						_fetchedStructures = session.GetByIdInterval<QueryGuidItem>(_structures[1].StructureId, _structures[2].StructureId).ToList();
					}
				});
			};

			It should_have_failed = () =>
			{
				CaughtException.ShouldNotBeNull();
				CaughtException.ShouldBeOfType<SisoDbException>();

				var ex = (SisoDbException)CaughtException;
				ex.Message.ShouldContain(ExceptionMessages.ReadSession_GetByIdInterval_WrongIdType);
			};

			private static IList<QueryGuidItem> _structures;
			private static IList<QueryGuidItem> _fetchedStructures;
		}
	}
}