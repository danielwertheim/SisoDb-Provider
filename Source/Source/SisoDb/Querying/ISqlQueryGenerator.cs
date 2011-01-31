using System;
using System.Linq.Expressions;
using SisoDb.Structures.Schemas;

namespace SisoDb.Querying
{
    internal interface ISqlQueryGenerator
    {
        ISqlQuery Generate<T>(Expression<Func<T, bool>> e, IStructureSchema schema) where T : class;
    }
}