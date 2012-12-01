using ApprovalTests;
using ApprovalTests.Reporters;
using NUnit.Framework;
using SisoDb.Spatial;

namespace SisoDb.UnitTests.Spatial
{
    [TestFixture]
    [UseReporter(typeof(DiffReporter))]
    public class GeographyFactoryTests : UnitTestBase
    {
        [Test]
        public virtual void When_Coordinates_has_correct_order_and_is_closed()
        {
            var g = GeographyFactory.CreatePolygon(CoordinatesTestFactory.CorrectOrderAndClosed(), SpatialReferenceId.Wsg84);

            Approvals.Verify(g);
        }

        [Test]
        public virtual void When_Coordinates_has_correct_order_and_is_not_closed()
        {
            var g = GeographyFactory.CreatePolygon(CoordinatesTestFactory.CorrectOrderNotClosed(), SpatialReferenceId.Wsg84);

            Approvals.Verify(g);
        }

        [Test]
        public virtual void When_Coordinates_has_wrong_order_and_closed()
        {
            var g = GeographyFactory.CreatePolygon(CoordinatesTestFactory.WrongOrderAndClosed(), SpatialReferenceId.Wsg84);

            Approvals.Verify(g);
        }

        [Test]
        public virtual void When_Coordinates_has_wrong_order_and_not_closed()
        {
            var g = GeographyFactory.CreatePolygon(CoordinatesTestFactory.WrongOrderNotClosed(), SpatialReferenceId.Wsg84);

            Approvals.Verify(g);
        }

        [Test]
        public virtual void When_Coordinates_has_wrong_order_starting_south_to_north_and_many_to_west_and_back_and_is_closed()
        {
            var g = GeographyFactory.CreatePolygon(CoordinatesTestFactory.WrongSouthToNorthAndManyWestAndBackClosed(), SpatialReferenceId.Wsg84);

            Approvals.Verify(g);
        }

        [Test]
        public virtual void When_Castle()
        {
            var g = GeographyFactory.CreatePolygon(CoordinatesTestFactory.Castle(), SpatialReferenceId.Wsg84);

            Approvals.Verify(g);
        }

        [Test]
        public virtual void When_Star_that_is_wrong_from_the_beginning()
        {
            var g = GeographyFactory.CreatePolygon(CoordinatesTestFactory.StartThatDoesNotRenderWithoutFix(), SpatialReferenceId.Wsg84);

            Approvals.Verify(g);
        }

        [Test]
        public virtual void When_drawn_lower_right_counter_clockwise()
        {
            var g = GeographyFactory.CreatePolygon(CoordinatesTestFactory.TreasureIsland_drawn_lower_right_counter_clockwise(), SpatialReferenceId.Wsg84);

            Approvals.Verify(g);
        }
    }
}