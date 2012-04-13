using System;
using SisoDb.Querying;

namespace SisoDb.DbSchema
{
    public interface INamedQueryGenerator<T> where T : class 
    {
        string Generate(string name, Action<IQueryBuilder<T>> spec);
    }
}