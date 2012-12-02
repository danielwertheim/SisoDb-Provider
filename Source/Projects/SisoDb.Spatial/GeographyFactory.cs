using System.Linq;
using Microsoft.SqlServer.Types;

namespace SisoDb.Spatial
{
    public static class GeographyFactory
    {
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

            return b.ConstructedGeography.EnvelopeAngle() > 90 
                ? b.ConstructedGeography.ReorientObject() 
                : b.ConstructedGeography;
        }
    }
}