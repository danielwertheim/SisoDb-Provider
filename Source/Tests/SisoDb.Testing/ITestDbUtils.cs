namespace SisoDb.Testing
{
    public interface ITestDbUtils
    {
        void DropDatabaseIfExists(string name);
        void EnsureDbExists(string name);
        bool TableExists(string name);
        bool TypeExists(string name);
        //void CreateProcedure(string spSql);
        //void DropProcedure(string spName);
        //void AddColumns(string tableName, params string[] columns);
        //void DropColumns(string tableName, params string[] columnNames);
        //bool ColumnsExist(string tableName, params string[] columnNames);
        //DataTable GetTableBySql(string sql);
        //void ExecuteSql(CommandType commandType, string sql);
        //T ExecuteScalar<T>(CommandType commandType, string sql);
        //T? ExecuteNullableScalar<T>(CommandType commandType, string sql) where T : struct;
        //bool IndexExists(string tableName, string indexName);
        int RowCount(string tableName, string where = null);
    }
}