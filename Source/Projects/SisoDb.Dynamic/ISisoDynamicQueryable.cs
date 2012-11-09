using System;
using System.Collections.Generic;

namespace SisoDb.Dynamic
{
    public interface ISisoDynamicQueryable
    {
        bool Any();
        bool Any(string expression);
        int Count();
        int Count(string expression);
        bool Exists(object id);

        object First();
        string FirstAsJson();
        object FirstOrDefault();
        string FirstOrDefaultAsJson();

        object Single();
        string SingleAsJson();
        object SingleOrDefault();
        string SingleOrDefaultAsJson();

        object[] ToArray();
        string[] ToArrayOfJson();
        
        IEnumerable<object> ToEnumerable();
        IEnumerable<string> ToEnumerableOfJson();

        IList<object> ToList();
        IList<string> ToListOfJson();

        ISisoDynamicQueryable Take(int numOfStructures);
        ISisoDynamicQueryable Page(int pageIndex, int pageSize);
        //TODO: Rem for v16.0.0 final
        //ISisoDynamicQueryable Include(Type includeType, params string[] expression);
        ISisoDynamicQueryable Where(params string[] expression);
        ISisoDynamicQueryable OrderBy(params string[] expressions);
        ISisoDynamicQueryable OrderByDescending(params string[] expressions);
    }
}