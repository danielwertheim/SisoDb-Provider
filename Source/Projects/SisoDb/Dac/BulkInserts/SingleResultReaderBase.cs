using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using SisoDb.DbSchema;

namespace SisoDb.Dac.BulkInserts
{
    public abstract class SingleResultReaderBase<T> : DbDataReader where T : class 
    {
        public StorageSchemaBase StorageSchema { get; private set; }

        protected IEnumerable<T> Items { get; private set; }

        protected IEnumerator<T> Enumerator { get; private set; }

        protected SingleResultReaderBase(StorageSchemaBase storageSchema, IEnumerable<T> items)
        {
            StorageSchema = storageSchema;
            Items = items;
            Enumerator = Items.GetEnumerator();
        }

        public override bool IsClosed
        {
            get { return Enumerator == null; }
        }

        public override int RecordsAffected
        {
            get { return Items.Count(); }
        }

        public override int FieldCount
        {
            get { return StorageSchema.GetFieldCount(); }
        }

        public override bool HasRows
        {
            get { return Items != null && Items.Count() > 0; }
        }

        public override void Close()
        {
            Enumerator = null;
            Items = null;
        }
        
        public override bool Read()
        {
            return Enumerator.MoveNext();
        }

        public override IEnumerator GetEnumerator()
        {
            return Enumerator;
        }

        public override int GetOrdinal(string name)
        {
            return StorageSchema[name].Ordinal;
        }

        public override object this[int ordinal]
        {
            get { throw new NotSupportedException(); }
        }

        public override object this[string name]
        {
            get { throw new NotSupportedException(); }
        }

        public override int Depth
        {
            get { throw new NotSupportedException(); }
        }

        public override string GetName(int ordinal)
        {
            throw new NotSupportedException();
        }

        public override DataTable GetSchemaTable()
        {
            throw new NotSupportedException();
        }

        public override bool NextResult()
        {
            throw new NotSupportedException();
        }

        public override bool GetBoolean(int ordinal)
        {
            throw new NotSupportedException();
        }

        public override byte GetByte(int ordinal)
        {
            throw new NotSupportedException();
        }

        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            throw new NotSupportedException();
        }

        public override char GetChar(int ordinal)
        {
            throw new NotSupportedException();
        }

        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            throw new NotSupportedException();
        }

        public override Guid GetGuid(int ordinal)
        {
            throw new NotSupportedException();
        }

        public override short GetInt16(int ordinal)
        {
            throw new NotSupportedException();
        }

        public override int GetInt32(int ordinal)
        {
            throw new NotSupportedException();
        }

        public override long GetInt64(int ordinal)
        {
            throw new NotSupportedException();
        }

        public override DateTime GetDateTime(int ordinal)
        {
            throw new NotSupportedException();
        }

        public override string GetString(int ordinal)
        {
            throw new NotSupportedException();
        }

        public override int GetValues(object[] values)
        {
            throw new NotSupportedException();
        }

        public override bool IsDBNull(int ordinal)
        {
            throw new NotSupportedException();
        }

        public override decimal GetDecimal(int ordinal)
        {
            throw new NotSupportedException();
        }

        public override double GetDouble(int ordinal)
        {
            throw new NotSupportedException();
        }

        public override float GetFloat(int ordinal)
        {
            throw new NotSupportedException();
        }

        public override string GetDataTypeName(int ordinal)
        {
            throw new NotSupportedException();
        }

        public override Type GetFieldType(int ordinal)
        {
            throw new NotSupportedException();
        }
    }
}