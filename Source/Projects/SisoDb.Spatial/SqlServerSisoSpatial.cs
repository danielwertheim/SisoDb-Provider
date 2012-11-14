using System.Data.SqlTypes;
using System.Globalization;
using System.Text;
using Microsoft.SqlServer.Types;
using SisoDb.Dac;
using SisoDb.NCore;
using SisoDb.DbSchema;
using SisoDb.Structures.Schemas;

namespace SisoDb.Spatial
{
    public class SqlServerSisoSpatial : ISisoSpatial
    {
        protected readonly ISessionExecutionContext ExecutionContext;
        protected ISession Session { get { return ExecutionContext.Session; } }
        protected readonly ISqlStatements SqlStatements;

        protected internal SqlServerSisoSpatial(ISessionExecutionContext executionContext)
        {
            ExecutionContext = executionContext;
            SqlStatements = SpatialSqlStatements.Instance;
        }

        public virtual bool PointExistsInPolygonFor<T>(object id, Coordinates coords, int srid = SpatialReferenceId.Wsg84) where T : class
        {
            return ExecutionContext.Try(() =>
            {
                var schema = Session.GetStructureSchema<T>();
                var sidParam = CreateStructureIdParam<T>(id);
                var geoParam = CreatePointParam(coords, srid);
                var sql = SqlStatements.GetSql("PointExistsInPolygon").Inject(GenerateTableName(schema));

                return Session.DbClient.ExecuteScalar<int>(sql, sidParam, geoParam) > 0;
            });
        }

        public virtual void UpsertFor<T>() where T : class
        {
            ExecutionContext.Try(() =>
            {
                var schema = Session.GetStructureSchema<T>();
                var sql = SqlStatements.GetSql("UpsertSpatialTable").Inject(GenerateTableName(schema), schema.GetStructureTableName());
                Session.DbClient.ExecuteNonQuery(sql);
            });
        }

        public virtual void DropFor<T>() where T : class
        {
            ExecutionContext.Try(() =>
            {
                var schema = Session.GetStructureSchema<T>();
                var sql = SqlStatements.GetSql("DropSpatialTable").Inject(GenerateTableName(schema));
                Session.DbClient.ExecuteNonQuery(sql);
            });
        }

        public virtual void DeleteGeoIn<T>(object id) where T : class
        {
            ExecutionContext.Try(() =>
            {
                var schema = Session.GetStructureSchema<T>();
                var sidParam = CreateStructureIdParam<T>(id);
                var sql = SqlStatements.GetSql("DeleteGeo").Inject(GenerateTableName(schema));
                Session.DbClient.ExecuteNonQuery(sql, sidParam);
            });
        }

        public virtual void InsertPolygonTo<T>(object id, Coordinates[] coords, int srid = SpatialReferenceId.Wsg84) where T : class
        {
            ExecutionContext.Try(() =>
            {
                var schema = Session.GetStructureSchema<T>();
                var sidParam = CreateStructureIdParam<T>(id);
                var geoParam = CreatePolygonParam(coords, srid);
                var sql = SqlStatements.GetSql("InsertGeo").Inject(GenerateTableName(schema));
                Session.DbClient.ExecuteNonQuery(sql, sidParam, geoParam);
            });
        }

        public virtual void UpdatePolygonIn<T>(object id, Coordinates[] coords, int srid = SpatialReferenceId.Wsg84) where T : class
        {
            ExecutionContext.Try(() =>
            {
                var schema = Session.GetStructureSchema<T>();
                var sidParam = CreateStructureIdParam<T>(id);
                var geoParam = CreatePolygonParam(coords, srid);
                var sql = SqlStatements.GetSql("UpdateGeo").Inject(GenerateTableName(schema));
                Session.DbClient.ExecuteNonQuery(sql, sidParam, geoParam);
            });
        }

        public virtual void SetPolygonIn<T>(object id, Coordinates[] coords, int srid = SpatialReferenceId.Wsg84) where T : class
        {
            ExecutionContext.Try(() =>
            {
                var schema = Session.GetStructureSchema<T>();
                var sidParam = CreateStructureIdParam<T>(id);
                var geoParam = CreatePolygonParam(coords, srid);
                var sql = SqlStatements.GetSql("SetGeo").Inject(GenerateTableName(schema));
                Session.DbClient.ExecuteNonQuery(sql, sidParam, geoParam);
            });
        }

        protected virtual string GenerateTableName(IStructureSchema schema)
        {
            return string.Concat(DbSchemaNamingPolicy.GenerateFor(schema.Name), "Spatials");
        }

        protected virtual DacParameter CreateStructureIdParam<T>(object id) where T : class
        {
            return new DacParameter("id", id);
        }

        protected virtual GeographyDacParameter CreatePointParam(Coordinates coords, int srid)
        {
            return new GeographyDacParameter("geo", SqlGeography.Point(coords.Latitude, coords.Longitude, srid));
        }

        protected virtual GeographyDacParameter CreatePolygonParam(Coordinates[] coords, int srid)
        {
            var s = BuildPolygonString(coords);
            var polygon = new SqlChars(s.ToCharArray());

            return new GeographyDacParameter("geo", SqlGeography.STPolyFromText(polygon, srid));
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