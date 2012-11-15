using System;
using SisoDb.Spatial;

namespace SisoDb.Specifications.Model
{
    public class SpatialGuidItem : SpatialItem<Guid>
    { }

    public class SpatialIdentityItem : SpatialItem<int>
    { }

    public class SpatialBigIdentityItem : SpatialItem<long>
    { }

    public class SpatialStringItem : SpatialItem<string>
    { }

    public class SpatialItem<T>
    {
        public T StructureId { get; set; }
    }

    public static class SpatialDataFactory
    {
        public static readonly Coordinates Circle1 = new Coordinates { Latitude = 47.653, Longitude = -122.358 };
        public static readonly Coordinates Point1 = new Coordinates { Latitude = 47.653, Longitude = -122.358 };
        public static readonly Coordinates Point2 = new Coordinates { Latitude = 47.649, Longitude = -122.348 };
        public static readonly Coordinates PointWithinDefaultPolygon = new Coordinates { Latitude = 47.657, Longitude = -122.357 };
        public static readonly Coordinates PointOutsideDefaultPolygon = new Coordinates { Latitude = 47.657, Longitude = -122.4 };
        
        public static Coordinates[] DefaultPolygon()
        {
            return new[]
            {
                new Coordinates { Latitude = 47.653, Longitude = -122.358 },
                new Coordinates { Latitude = 47.649, Longitude = -122.348 },
                new Coordinates { Latitude = 47.658, Longitude = -122.348 },
                new Coordinates { Latitude = 47.658, Longitude = -122.358 },
                new Coordinates { Latitude = 47.653, Longitude = -122.358 },
            };
        }

        public static Coordinates[] SmallerPolygon()
        {
            return new[]
            {
                new Coordinates { Latitude = 47.653, Longitude = -122.358 },
                new Coordinates { Latitude = 47.649, Longitude = -122.348 },
                new Coordinates { Latitude = 47.658, Longitude = -122.348 },
                new Coordinates { Latitude = 47.653, Longitude = -122.358 },
            };
        }
    }
}