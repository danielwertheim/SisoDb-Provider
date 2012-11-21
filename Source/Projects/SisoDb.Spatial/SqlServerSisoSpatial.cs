using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Types;
using SisoDb.Dac;
using SisoDb.NCore;
using SisoDb.DbSchema;
using SisoDb.Spatial.Resources;
using SisoDb.Structures;
using SisoDb.Structures.Schemas;

namespace SisoDb.Spatial
{
    public class SqlServerSisoSpatial : ISisoSpatial
    {
        private const string GeoParamName = "geo";

        protected readonly ISessionExecutionContext ExecutionContext;
        protected ISession Session { get { return ExecutionContext.Session; } }
        protected readonly ISqlStatements SqlStatements;

        protected internal SqlServerSisoSpatial(ISessionExecutionContext executionContext)
        {
            ExecutionContext = executionContext;
            SqlStatements = SpatialSqlStatements.Instance;
        }

        public virtual void EnableFor<T>() where T : class
        {
            ExecutionContext.Try(() =>
            {
                if (!Session.Db.Settings.AllowDynamicSchemaCreation) return;
                var schema = Session.GetStructureSchema<T>();
                var sql = GetUpsertTableSql(schema);
                Session.DbClient.ExecuteNonQuery(sql);
            });
        }

        public virtual void RemoveFor<T>() where T : class
        {
            ExecutionContext.Try(() =>
            {
                var schema = Session.GetStructureSchema<T>();
                var sql = SqlStatements.GetSql("DropSpatialTable").Inject(schema.GetSpatialTableName());
                Session.DbClient.ExecuteNonQuery(sql);
            });
        }

        public virtual void DeleteGeoFor<T>(object id) where T : class
        {
            ExecutionContext.Try(() =>
            {
                var schema = Session.GetStructureSchema<T>();
                var sidParam = CreateStructureIdParam<T>(id);
                var sql = SqlStatements.GetSql("DeleteGeo").Inject(schema.GetSpatialTableName());
                Session.DbClient.ExecuteNonQuery(sql, sidParam);
            });
        }

        public virtual void InsertPoint<T>(object id, Coordinates coords, int srid = SpatialReferenceId.Wsg84) where T : class
        {
            ExecutionContext.Try(() =>
            {
                var schema = Session.GetStructureSchema<T>();
                var sidParam = CreateStructureIdParam<T>(id);
                var geoParam = CreatePointParam(coords, srid);
                var sql = SqlStatements.GetSql("InsertGeo").Inject(schema.GetSpatialTableName());
                Session.DbClient.ExecuteNonQuery(sql, sidParam, geoParam);
            });
        }

        public virtual void UpdatePoint<T>(object id, Coordinates coords, int srid = SpatialReferenceId.Wsg84) where T : class
        {
            ExecutionContext.Try(() =>
            {
                var schema = Session.GetStructureSchema<T>();
                var sidParam = CreateStructureIdParam<T>(id);
                var geoParam = CreatePointParam(coords, srid);
                var sql = SqlStatements.GetSql("UpdateGeo").Inject(schema.GetSpatialTableName());
                Session.DbClient.ExecuteNonQuery(sql, sidParam, geoParam);
            });
        }

        public virtual void SetPoint<T>(object id, Coordinates coords, int srid = SpatialReferenceId.Wsg84) where T : class
        {
            ExecutionContext.Try(() =>
            {
                var schema = Session.GetStructureSchema<T>();
                var sidParam = CreateStructureIdParam<T>(id);
                var geoParam = CreatePointParam(coords, srid);
                var sql = SqlStatements.GetSql("SetGeo").Inject(schema.GetSpatialTableName());
                Session.DbClient.ExecuteNonQuery(sql, sidParam, geoParam);
            });
        }

        public virtual void InsertCircle<T>(object id, Coordinates center, double radiusInMetres, int srid = SpatialReferenceId.Wsg84) where T : class
        {
            ExecutionContext.Try(() =>
            {
                var schema = Session.GetStructureSchema<T>();
                var sidParam = CreateStructureIdParam<T>(id);
                var geoParam = CreateCirlceParam(center, radiusInMetres, srid);
                var sql = SqlStatements.GetSql("InsertGeo").Inject(schema.GetSpatialTableName());
                Session.DbClient.ExecuteNonQuery(sql, sidParam, geoParam);
            });
        }

        public virtual void UpdateCircle<T>(object id, Coordinates center, double radiusInMetres, int srid = SpatialReferenceId.Wsg84) where T : class
        {
            ExecutionContext.Try(() =>
            {
                var schema = Session.GetStructureSchema<T>();
                var sidParam = CreateStructureIdParam<T>(id);
                var geoParam = CreateCirlceParam(center, radiusInMetres, srid);
                var sql = SqlStatements.GetSql("UpdateGeo").Inject(schema.GetSpatialTableName());
                Session.DbClient.ExecuteNonQuery(sql, sidParam, geoParam);
            });
        }

        public virtual void SetCircle<T>(object id, Coordinates center, double radiusInMetres, int srid = SpatialReferenceId.Wsg84) where T : class
        {
            ExecutionContext.Try(() =>
            {
                var schema = Session.GetStructureSchema<T>();
                var sidParam = CreateStructureIdParam<T>(id);
                var geoParam = CreateCirlceParam(center, radiusInMetres, srid);
                var sql = SqlStatements.GetSql("SetGeo").Inject(schema.GetSpatialTableName());
                Session.DbClient.ExecuteNonQuery(sql, sidParam, geoParam);
            });
        }

        public virtual void InsertPolygon<T>(object id, Coordinates[] coords, int srid = SpatialReferenceId.Wsg84) where T : class
        {
            ExecutionContext.Try(() =>
            {
                var schema = Session.GetStructureSchema<T>();
                var sidParam = CreateStructureIdParam<T>(id);
                var geoParam = CreatePolygonParam(coords, srid);
                var sql = SqlStatements.GetSql("InsertGeo").Inject(schema.GetSpatialTableName());
                Session.DbClient.ExecuteNonQuery(sql, sidParam, geoParam);
            });
        }

        public virtual void UpdatePolygon<T>(object id, Coordinates[] coords, int srid = SpatialReferenceId.Wsg84) where T : class
        {
            ExecutionContext.Try(() =>
            {
                var schema = Session.GetStructureSchema<T>();
                var sidParam = CreateStructureIdParam<T>(id);
                var geoParam = CreatePolygonParam(coords, srid);
                var sql = SqlStatements.GetSql("UpdateGeo").Inject(schema.GetSpatialTableName());
                Session.DbClient.ExecuteNonQuery(sql, sidParam, geoParam);
            });
        }

        public virtual void SetPolygon<T>(object id, Coordinates[] coords, int srid = SpatialReferenceId.Wsg84) where T : class
        {
            ExecutionContext.Try(() =>
            {
                var schema = Session.GetStructureSchema<T>();
                var sidParam = CreateStructureIdParam<T>(id);
                var geoParam = CreatePolygonParam(coords, srid);
                var sql = SqlStatements.GetSql("SetGeo").Inject(schema.GetSpatialTableName());
                Session.DbClient.ExecuteNonQuery(sql, sidParam, geoParam);
            });
        }

        public virtual void MakeValid<T>(object id, int srid = SpatialReferenceId.Wsg84) where T : class 
        {
            ExecutionContext.Try(() =>
            {
                var schema = Session.GetStructureSchema<T>();
                var sidParam = CreateStructureIdParam<T>(id);
                if(Session.Db.ConnectionInfo.ProviderType == StorageProviders.Sql2008)
                    OnMakeGeoValid2008(schema, sidParam);
                else
                    OnMakeGeoValid(schema, sidParam);
            });
        }

        protected virtual void OnMakeGeoValid2008(IStructureSchema schema, IDacParameter sidParam)
        {
            var sql = SqlStatements.GetSql("GetGeo").Inject(schema.GetSpatialTableName());
            var geo = GetGeograpy(sql, sidParam);
            if(geo.STIsValid())
                return;

            geo = geo.MakeValid();
            
            var geoParam = new GeographyDacParameter(GeoParamName, geo);
            sql = SqlStatements.GetSql("UpdateGeo").Inject(schema.GetSpatialTableName());
            Session.DbClient.ExecuteNonQuery(sql, sidParam, geoParam);
        }

        protected virtual void OnMakeGeoValid(IStructureSchema schema, IDacParameter sidParam)
        {
            var sql = SqlStatements.GetSql("MakeGeoValid").Inject(schema.GetSpatialTableName());
            Session.DbClient.ExecuteNonQuery(sql, sidParam);
        }

        public virtual Coordinates[] GetCoordinatesIn<T>(object id) where T : class
        {
            return ExecutionContext.Try(() =>
            {
                var schema = Session.GetStructureSchema<T>();
                var sidParam = CreateStructureIdParam<T>(id);
                var sql = SqlStatements.GetSql("GetGeo").Inject(schema.GetSpatialTableName());
                
                return ExtractPoints(GetGeograpy(sql, sidParam)).Select(p => new Coordinates
                {
                    Latitude = p.Lat.Value,
                    Longitude = p.Long.Value
                }).ToArray();
            });
        }

        protected virtual SqlGeography GetGeograpy(string sql, params IDacParameter[] parameters)
        {
            //I know, ugly. Thank Microsoft for that: http://msdn.microsoft.com/en-us/library/ms143179.aspx
            SqlGeography geo = null;
            Session.DbClient.SingleResultSequentialReader(sql, dr =>
            {
                var d = (SqlDataReader)dr;
                geo = SqlGeography.Deserialize(d.GetSqlBytes(0));
            }, parameters);
            return geo;
        }

        protected virtual SqlGeography[] ExtractPoints(SqlGeography geo)
        {
            if(geo == null || geo.IsNull)
                return new SqlGeography[0];

            var numOfPoints = geo.STNumPoints();
            if (numOfPoints.IsNull || numOfPoints.Value == 0)
                return new SqlGeography[0];

            var points = new SqlGeography[numOfPoints.Value];
            for (var i = 0; i < numOfPoints.Value; i++)
                points[i] = geo.STPointN(i+1);

            return points;
        }

        public virtual bool ContainsPoint<T>(object id, Coordinates coords, int srid = SpatialReferenceId.Wsg84) where T : class
        {
            return ExecutionContext.Try(() =>
            {
                var schema = Session.GetStructureSchema<T>();
                var sidParam = CreateStructureIdParam<T>(id);
                var geoParam = CreatePointParam(coords, srid);

                var sql = (Session.Db.ConnectionInfo.ProviderType == StorageProviders.Sql2008) 
                    ? SqlStatements.GetSql("ContainsPoint2008").Inject(schema.GetSpatialTableName())
                    : SqlStatements.GetSql("ContainsPoint").Inject(schema.GetSpatialTableName());

                return Session.DbClient.ExecuteScalar<int>(sql, sidParam, geoParam) == 1;
            });
        }

        public virtual bool ContainsPointAfterExpand<T>(object id, Coordinates coords, double expandWithMetres, int srid = SpatialReferenceId.Wsg84) where T : class
        {
            return ExecutionContext.Try(() =>
            {
                var schema = Session.GetStructureSchema<T>();
                var sidParam = CreateStructureIdParam<T>(id);
                var geoParam = CreatePointParam(coords, srid);
                var bufferParam = CreateBufferParam(expandWithMetres);
                var sql = (Session.Db.ConnectionInfo.ProviderType == StorageProviders.Sql2008)
                    ? SqlStatements.GetSql("ContainsPointWithBuffer2008").Inject(schema.GetSpatialTableName())
                    : SqlStatements.GetSql("ContainsPointWithBuffer").Inject(schema.GetSpatialTableName());

                return Session.DbClient.ExecuteScalar<int>(sql, sidParam, geoParam, bufferParam) == 1;
            });
        }

        protected virtual string GetUpsertTableSql(IStructureSchema structureSchema)
        {
            var tableName = structureSchema.GetSpatialTableName();
            var structureTableName = structureSchema.GetStructureTableName();

            if (structureSchema.IdAccessor.IdType.IsIdentity())
                return SqlStatements.GetSql("UpsertSpatialTableWithIdentity").Inject(tableName, structureTableName);

            if (structureSchema.IdAccessor.IdType.IsGuid())
                return SqlStatements.GetSql("UpsertSpatialTableWithGuid").Inject(tableName, structureTableName);

            if (structureSchema.IdAccessor.IdType.IsString())
                return SqlStatements.GetSql("UpsertSpatialTableWithString").Inject(tableName, structureTableName);

            throw new SisoDbException(ExceptionMessages.IdTypeNotSupported.Inject(structureSchema.IdAccessor.IdType));
        }

        protected virtual IDacParameter CreateStructureIdParam<T>(object id) where T : class
        {
            return new DacParameter("id", id);
        }

        protected virtual IDacParameter CreateBufferParam(double metres)
        {
            return new DacParameter("buffer", metres);
        }

        protected virtual GeographyDacParameter CreatePointParam(Coordinates coords, int srid)
        {
            return new GeographyDacParameter(GeoParamName, SqlGeography.Point(coords.Latitude, coords.Longitude, srid));
        }

        protected virtual GeographyDacParameter CreateCirlceParam(Coordinates coords, double radiusInMetres, int srid)
        {
            return new GeographyDacParameter(GeoParamName, SqlGeography.Point(coords.Latitude, coords.Longitude, srid).STBuffer(radiusInMetres));
        }

        protected virtual GeographyDacParameter CreatePolygonParam(Coordinates[] coords, int srid)
        {
            var s = BuildPolygonString(coords);
            var polygon = new SqlChars(s.ToCharArray());

            return new GeographyDacParameter(GeoParamName, SqlGeography.STPolyFromText(polygon, srid));
        }

        protected virtual string BuildPolygonString(Coordinates[] coords)
        {
            var s = new StringBuilder();
            s.Append("POLYGON((");
            for (var i = 0; i < coords.Length; i++)
            {
                var coord = coords[i];
                s.Append(coord.Longitude.ToString(CultureInfo.InvariantCulture));
                s.Append(" ");
                s.Append(coord.Latitude.ToString(CultureInfo.InvariantCulture));

                if (i < coords.Length - 1)
                    s.Append(",");
            }
            s.Append("))");

            return s.ToString();
        }
    }
}