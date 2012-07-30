using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SisoDb.Dac;
using SisoDb.PineCone.Structures.Schemas;

namespace SisoDb.Testing
{
    public interface ITestDbUtils
    {
        bool TableExists(string name);
        bool TablesExists(string[] names);
        bool TypeExists(string name);
        IList<DbColumn> GetColumns(string tableName, params string[] namesToSkip);
        void CreateProcedure(string spSql);
        void DropProcedure(string spName);
        T ExecuteScalar<T>(string sql, params IDacParameter[] parameters);
        T? ExecuteNullableScalar<T>(string sql) where T : struct;
        int RowCount(string tableName, string where = null, params IDacParameter[] parameters);
        void DeleteQueryIndexesFor(IStructureSchema structureSchema, IEnumerable<Guid> structureIds);
        bool AnyIndexesTableHasMember<T>(IStructureSchema structureSchema, object id, Expression<Func<T, object>> member) where T : class;
        bool UniquesTableHasMember<T>(IStructureSchema structureSchema, object id, Expression<Func<T, object>> member) where T : class;
        bool IdentityRowExistsForSchema(IStructureSchema structureSchemaForSetTwo);
    }
}