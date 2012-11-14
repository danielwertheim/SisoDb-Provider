using System;

namespace SisoDb.Spatial
{
    [Serializable]
    public class Coordinates : IEquatable<Coordinates>
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as Coordinates);
        }

        public bool Equals(Coordinates other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Latitude.Equals(other.Latitude) && Longitude.Equals(other.Longitude);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Latitude.GetHashCode()*397) ^ Longitude.GetHashCode();
            }
        }
    }
}