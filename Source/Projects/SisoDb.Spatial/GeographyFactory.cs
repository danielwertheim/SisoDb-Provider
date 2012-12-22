using System.Linq;
using Microsoft.SqlServer.Types;
using SisoDb.NCore;
using SisoDb.Spatial.Resources;

namespace SisoDb.Spatial
{
    public static class GeographyFactory
    {
        private static readonly string PolygonTypeName = OpenGisGeometryType.Polygon.ToString();

        public static SqlGeography CreatePolygon(Coordinates[] coordinates, int srid)
        {
            var list = coordinates.Distinct().ToList();
            
            var b = new SqlGeographyBuilder();
            b.SetSrid(srid);
            b.BeginGeography(OpenGisGeographyType.Polygon);
            b.BeginFigure(list[0].Latitude, list[0].Longitude);

            for (var i = 1; i < list.Count; i++)
                b.AddLine(list[i].Latitude, list[i].Longitude);

            b.AddLine(list[0].Latitude, list[0].Longitude);
            b.EndFigure();
            b.EndGeography();

            //Fix left-hand rule. We always want left-hand.
            var g = b.ConstructedGeography;
            if (!g.STIsValid())
                throw new SisoDbException(ExceptionMessages.NotAValidPolygon.Inject(g.IsValidDetailed()));

            if (g.EnvelopeAngle() > 90)
                g = g.ReorientObject(); 

            return g;
        }
    }
}