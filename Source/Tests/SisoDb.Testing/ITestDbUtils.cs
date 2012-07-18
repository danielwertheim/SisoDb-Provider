using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
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
        T ExecuteScalar<T>(CommandType commandType, string sql);
        T? ExecuteNullableScalar<T>(CommandType commandType, string sql) where T : struct;
        int RowCount(string tableName, string where = null);
        void DeleteQueryIndexesFor(IStructureSchema structureSchema, IEnumerable<Guid> structureIds);
        bool AnyIndexesTableHasMember<T>(IStructureSchema structureSchema, ValueType id, Expression<Func<T, object>> member) where T : class;
        bool UniquesTableHasMember<T>(IStructureSchema structureSchema, ValueType id, Expression<Func<T, object>> member) where T : class;
        bool IdentityRowExistsForSchema(IStructureSchema structureSchemaForSetTwo);
    }
}