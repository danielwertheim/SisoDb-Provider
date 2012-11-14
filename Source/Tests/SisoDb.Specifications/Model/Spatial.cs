using System;

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
}