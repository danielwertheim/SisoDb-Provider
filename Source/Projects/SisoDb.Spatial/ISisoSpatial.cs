namespace SisoDb.Spatial
{
    public interface ISisoSpatial
    {
        /// <summary>
        /// Creates a table for the structure <typeparamref name="T"></typeparamref> holding spatial data, but only if it does not exist.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void EnableFor<T>() where T : class;

        /// <summary>
        /// Will drop any existing table for the structure <typeparamref name="T"></typeparamref>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void RemoveFor<T>() where T : class;
        
        /// <summary>
        /// Deletes any geo-records for structure <typeparamref name="T"></typeparamref> identified by <paramref name="id"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        void DeleteGeoFor<T>(object id) where T : class;
        
        /// <summary>
        /// Inserts geo as point for structure <typeparamref name="T"></typeparamref> identified by <paramref name="id"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="coords"></param>
        /// <param name="srid"></param>
        void InsertPoint<T>(object id, Coordinates coords, int srid = SpatialReferenceId.Wsg84) where T : class;
        
        /// <summary>
        /// Updates geo as point for structure <typeparamref name="T"></typeparamref> identified by <paramref name="id"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="coords"></param>
        /// <param name="srid"></param>
        void UpdatePoint<T>(object id, Coordinates coords, int srid = SpatialReferenceId.Wsg84) where T : class;
        
        /// <summary>
        /// Inserts or Updates geo as point for structure <typeparamref name="T"></typeparamref> identified by <paramref name="id"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="coords"></param>
        /// <param name="srid"></param>
        void SetPoint<T>(object id, Coordinates coords, int srid = SpatialReferenceId.Wsg84) where T : class;
        
        /// <summary>
        /// Inserts geo as circle for structure <typeparamref name="T"></typeparamref> identified by <paramref name="id"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="center"></param>
        /// <param name="radiusInMetres"></param>
        /// <param name="srid"></param>
        void InsertCircle<T>(object id, Coordinates center, double radiusInMetres, int srid = SpatialReferenceId.Wsg84) where T : class;
        
        /// <summary>
        /// Updates geo as circle for structure <typeparamref name="T"></typeparamref> identified by <paramref name="id"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="center"></param>
        /// <param name="radiusInMetres"></param>
        /// <param name="srid"></param>
        void UpdateCircle<T>(object id, Coordinates center, double radiusInMetres, int srid = SpatialReferenceId.Wsg84) where T : class;
        
        /// <summary>
        /// Inserts or Updates geo as circle for structure <typeparamref name="T"></typeparamref> identified by <paramref name="id"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="center"></param>
        /// <param name="radiusInMetres"></param>
        /// <param name="srid"></param>
        void SetCircle<T>(object id, Coordinates center, double radiusInMetres, int srid = SpatialReferenceId.Wsg84) where T : class;
        
        /// <summary>
        /// Inserts geo as polygon for structure <typeparamref name="T"></typeparamref> identified by <paramref name="id"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="coords"></param>
        /// <param name="srid"></param>
        void InsertPolygon<T>(object id, Coordinates[] coords, int srid = SpatialReferenceId.Wsg84) where T : class;
        
        /// <summary>
        /// Updates geo as polygon for structure <typeparamref name="T"></typeparamref> identified by <paramref name="id"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="coords"></param>
        /// <param name="srid"></param>
        void UpdatePolygon<T>(object id, Coordinates[] coords, int srid = SpatialReferenceId.Wsg84) where T : class;
        
        /// <summary>
        /// Inserts or Updates geo as polygon for structure <typeparamref name="T"></typeparamref> identified by <paramref name="id"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="coords"></param>
        /// <param name="srid"></param>
        void SetPolygon<T>(object id, Coordinates[] coords, int srid = SpatialReferenceId.Wsg84) where T : class;

        /// <summary>
        /// Will try and make a wrongly stored object valid.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="srid"></param>
        void MakeValid<T>(object id, int srid = SpatialReferenceId.Wsg84) where T : class;
        
        /// <summary>
        /// Returns stored coordinates for the geo-object. If none exists, an empty array is returned.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        Coordinates[] GetCoordinatesIn<T>(object id) where T : class;
        
        /// <summary>
        /// Returns bool indicating if a stored geo object identified by <paramref name="id"/> in structure <typeparamref name="T"></typeparamref> contains sent Point or not.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="coords"></param>
        /// <param name="srid"></param>
        /// <returns></returns>
        bool ContainsPoint<T>(object id, Coordinates coords, int srid = SpatialReferenceId.Wsg84) where T : class;
        
        /// <summary>
        /// Returns bool indicating if a stored geo object, expanded with <paramref name="expandWithMetres"/>, identified by <paramref name="id"/> in structure <typeparamref name="T"></typeparamref> contains sent Point or not.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="coords"></param>
        /// <param name="expandWithMetres"></param>
        /// <param name="srid"></param>
        /// <returns></returns>
        bool ContainsPointAfterExpand<T>(object id, Coordinates coords, double expandWithMetres, int srid = SpatialReferenceId.Wsg84) where T : class;
    }
}