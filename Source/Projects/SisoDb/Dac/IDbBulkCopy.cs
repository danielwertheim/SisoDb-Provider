using System;
using System.Data;

namespace SisoDb.Dac
{
    public interface IDbBulkCopy : IDisposable
    {
        string DestinationTableName { set; }
        int BatchSize { set; }
        void AddColumnMapping(string sourceFieldName, string destinationFieldName);
        void Write(IDataReader reader);
    }
}