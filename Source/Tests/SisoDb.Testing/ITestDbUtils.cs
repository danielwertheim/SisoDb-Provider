using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using PineCone.Structures.Schemas;

namespace SisoDb.Testing
{
    public interface ITestDbUtils
    {
        void DropDatabaseIfExists(string name);
        void EnsureDbExists(string name);
        bool TableExists(string name);
        bool TypeExists(string name);
        IList<DbColumn> GetColumns(string tableName, params string[] namesToSkip);
        void CreateProcedure(string spSql);
        void DropProcedure(string spName);
        T ExecuteScalar<T>(CommandType commandType, string sql);
        T? ExecuteNullableScalar<T>(CommandType commandType, string sql) where T : struct;
        int RowCount(string tableName, string where = null);
        bool IndexesTableHasMember<T>(IStructureSchema structureSchema, ValueType id, Expression<Func<T, object>> member) where T : class;
        bool UniquesTableHasMember<T>(IStructureSchema structureSchema, ValueType id, Expression<Func<T, object>> member) where T : class;
    }
}