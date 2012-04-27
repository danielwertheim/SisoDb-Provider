using System;
using System.Collections.Generic;
using System.Linq;
using EnsureThat;
using SisoDb.Querying;

namespace SisoDb.Dynamic
{
    public class SisoDynamicQueryable : ISisoDynamicQueryable
    {
        protected readonly Type StructureType;
        protected readonly IQueryEngine QueryEngine;
        protected readonly IQueryBuilder QueryBuilder;
        protected readonly IDynamicLambdaBuilder LambdaBuilder;

        public SisoDynamicQueryable(Type structureType, IQueryBuilder queryBuilder, IQueryEngine queryEngine, IDynamicLambdaBuilder lambdaBuilder)
        {
            Ensure.That(structureType, "structureType").IsNotNull();
            Ensure.That(queryBuilder, "queryBuilder").IsNotNull();
            Ensure.That(queryEngine, "queryEngine").IsNotNull();
            Ensure.That(lambdaBuilder, "lambdaBuilder").IsNotNull();

            StructureType = structureType;
            QueryBuilder = queryBuilder;
            QueryEngine = queryEngine;
            LambdaBuilder = lambdaBuilder;
        }

        public virtual bool Any()
        {
            return QueryBuilder.IsEmpty
                ? QueryEngine.Any(StructureType)
                : QueryEngine.Any(StructureType, QueryBuilder.Build());
        }

        public virtual bool Any(string expression)
        {
            Ensure.That(expression, "expression").IsNotNullOrWhiteSpace();
            
            QueryBuilder.Clear();
            QueryBuilder.Where(LambdaBuilder.BuildPredicate(StructureType, expression));

            return QueryEngine.Any(StructureType, QueryBuilder.Build());
        }

        public virtual int Count()
        {
            return QueryBuilder.IsEmpty
                ? QueryEngine.Count(StructureType)
                : QueryEngine.Count(StructureType, QueryBuilder.Build());
        }

        public virtual int Count(string expression)
        {
            Ensure.That(expression, "expression").IsNotNullOrWhiteSpace();

            QueryBuilder.Clear();
            QueryBuilder.Where(LambdaBuilder.BuildPredicate(StructureType, expression));

            return QueryEngine.Count(StructureType, QueryBuilder.Build());
        }

        public virtual bool Exists(object id)
        {
            Ensure.That(id, "id").IsNotNull();

            return QueryEngine.Exists(StructureType, id);
        }

        public virtual object First()
        {
            return ToEnumerable().First();
        }

        public virtual string FirstAsJson()
        {
            return ToEnumerableOfJson().First();
        }

        public virtual object FirstOrDefault()
        {
            return ToEnumerable().FirstOrDefault();
        }

        public virtual string FirstOrDefaultAsJson()
        {
            return ToEnumerableOfJson().FirstOrDefault();
        }

        public virtual object Single()
        {
            return ToEnumerable().Single();
        }

        public virtual string SingleAsJson()
        {
            return ToEnumerableOfJson().Single();
        }

        public virtual object SingleOrDefault()
        {
            return ToEnumerable().SingleOrDefault();
        }

        public virtual string SingleOrDefaultAsJson()
        {
            return ToEnumerableOfJson().SingleOrDefault();
        }

        public virtual object[] ToArray()
        {
            return ToEnumerable().ToArray();
        }

        public virtual string[] ToArrayOfJson()
        {
            return ToEnumerableOfJson().ToArray();
        }

        public virtual IEnumerable<object> ToEnumerable()
        {
            return QueryEngine.Query(QueryBuilder.Build(), StructureType);
        }

        public virtual IEnumerable<string> ToEnumerableOfJson()
        {
            return QueryEngine.QueryAsJson(QueryBuilder.Build(), StructureType);
        }

        public virtual IList<object> ToList()
        {
            return ToEnumerable().ToList();
        }

        public virtual IList<string> ToListOfJson()
        {
            return ToEnumerableOfJson().ToList();
        }

        public virtual ISisoDynamicQueryable Take(int numOfStructures)
        {
            QueryBuilder.Take(numOfStructures);

            return this;
        }

        public virtual ISisoDynamicQueryable Page(int pageIndex, int pageSize)
        {
            QueryBuilder.Page(pageIndex, pageSize);

            return this;
        }

        public virtual ISisoDynamicQueryable Include(Type includeType, params string[] expressions)
        {
            Ensure.That(expressions, "expressions").HasItems();
            
            QueryBuilder.Include(
                includeType, 
                expressions.Select(e => LambdaBuilder.BuildMember(StructureType, e)).ToArray());

            return this;
        }

        public virtual ISisoDynamicQueryable Where(params string[] expressions)
        {
            Ensure.That(expressions, "expressions").HasItems();

            QueryBuilder.Where(
                expressions.Select(e => LambdaBuilder.BuildPredicate(StructureType, e)).ToArray());

            return this;
        }

        public virtual ISisoDynamicQueryable OrderBy(params string[] expressions)
        {
            Ensure.That(expressions, "expressions").HasItems();

            QueryBuilder.OrderBy(
                expressions.Select(e => LambdaBuilder.BuildMember(StructureType, e)).ToArray());

            return this;
        }

        public virtual ISisoDynamicQueryable OrderByDescending(params string[] expressions)
        {
            Ensure.That(expressions, "expressions").HasItems();

            QueryBuilder.OrderByDescending(
                expressions.Select(e => LambdaBuilder.BuildMember(StructureType, e)).ToArray());

            return this;
        }
    }
}