using System;

namespace SisoDb.Spatial
{
    public interface ISisoSpatial 
    {
        /// <summary>
        /// Creates a table for the structure <typeparam name="T"></typeparam> holding spatial data, but only if it does not exist.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void UpsertFor<T>() where T : class;
        /// <summary>
        /// Will drop any existing table for the structure <typeparam name="T"></typeparam>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void DropFor<T>() where T : class;
        /// <summary>
        /// Returns bool indicating if a stored Polygon identified by <paramref name="id"/> in structure <typeparam name="T"></typeparam> contains a Point or not.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="coords"></param>
        /// <param name="srid"></param>
        /// <returns></returns>
        bool PointExistsInPolygonFor<T>(object id, Coordinates coords, int srid = SpatialReferenceId.Wsg84) where T : class;
        /// <summary>
        /// Deletes any geo-records for structure <typeparam name="T"></typeparam> identified by <paramref name="id"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        void DeleteGeoIn<T>(object id) where T : class;
        /// <summary>
        /// Inserts geo as polygon for structure <typeparam name="T"></typeparam> identified by <paramref name="id"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="coords"></param>
        /// <param name="srid"></param>
        void InsertPolygonTo<T>(object id, Coordinates[] coords, int srid = SpatialReferenceId.Wsg84) where T : class;
        /// <summary>
        /// Updates geo as polygon for structure <typeparam name="T"></typeparam> identified by <paramref name="id"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="coords"></param>
        /// <param name="srid"></param>
        void UpdatePolygonIn<T>(object id, Coordinates[] coords, int srid = SpatialReferenceId.Wsg84) where T : class;
        /// <summary>
        /// Inserts or Updates geo as polygon for structure <typeparam name="T"></typeparam> identified by <paramref name="id"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="coords"></param>
        /// <param name="srid"></param>
        void SetPolygonIn<T>(object id, Coordinates[] coords, int srid = SpatialReferenceId.Wsg84) where T : class;
    }
}