using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using SisoDb.DbSchema;
using SisoDb.EnsureThat;
using SisoDb.PineCone.Structures.Schemas;
using SisoDb.Querying.Lambdas;
using SisoDb.Querying.Lambdas.Parsers;

namespace SisoDb.Querying
{
	public class QueryBuilder<T> : IQueryBuilder<T> where T : class
	{
	    protected readonly IQueryBuilder InnerQueryBuilder;

        public virtual bool IsEmpty
	    {
            get { return InnerQueryBuilder.IsEmpty; }
	    }

		public QueryBuilder(IStructureSchemas structureSchemas, IExpressionParsers expressionParsers)
		{
            InnerQueryBuilder = new QueryBuilder(typeof(T), structureSchemas, expressionParsers);
		}

		public virtual void Clear()
		{
			InnerQueryBuilder.Clear();
		}

		public virtual IQuery Build()
		{
		    return InnerQueryBuilder.Build();
		}

		public virtual IQueryBuilder<T> Take(int numOfStructures)
		{
		    InnerQueryBuilder.Take(numOfStructures);

		    return this;
		}

		public virtual IQueryBuilder<T> Page(int pageIndex, int pageSize)
		{
		    InnerQueryBuilder.Page(pageIndex, pageSize);

			return this;
		}

		public virtual IQueryBuilder<T> Include<TInclude>(params Expression<Func<T, object>>[] expressions) where TInclude : class
		{
		    InnerQueryBuilder.Include(typeof (TInclude), expressions);

			return this;
		}

		public virtual IQueryBuilder<T> Where(params Expression<Func<T, bool>>[] expressions)
		{
		    InnerQueryBuilder.Where(expressions);

			return this;
		}

		public virtual IQueryBuilder<T> OrderBy(params Expression<Func<T, object>>[] expressions)
		{
		    InnerQueryBuilder.OrderBy(expressions);

			return this;
		}

		public virtual IQueryBuilder<T> OrderByDescending(params Expression<Func<T, object>>[] expressions)
		{
            InnerQueryBuilder.OrderByDescending(expressions);

			return this;
		}
	}

    public class QueryBuilder : IQueryBuilder
    {
        protected readonly Type StructureType;
        protected readonly IStructureSchemas StructureSchemas;
        protected readonly IStructureSchema StructureSchema;
        protected readonly IExpressionParsers ExpressionParsers;

        protected IQuery Query;
        protected readonly Dictionary<string, List<LambdaExpression>> BufferedIncludes;
        protected readonly List<LambdaExpression> BufferedWheres;
        protected readonly List<OrderByExpression> BufferedSortings;

        public virtual bool IsEmpty
        {
            get
            {
                return Query.IsEmpty
                       && BufferedIncludes.Count == 0
                       && BufferedSortings.Count == 0
                       && BufferedWheres.Count == 0;
            }
        }

        public QueryBuilder(Type structureType, IStructureSchemas structureSchemas, IExpressionParsers expressionParsers)
        {
            Ensure.That(structureType, "structureType").IsNotNull();
            Ensure.That(structureSchemas, "structureSchemas").IsNotNull();
            Ensure.That(expressionParsers, "expressionParsers").IsNotNull();

            StructureType = structureType;
            StructureSchemas = structureSchemas;
            StructureSchema = StructureSchemas.GetSchema(structureType);
            ExpressionParsers = expressionParsers;

            Query = new Query(StructureSchema);
            BufferedIncludes = new Dictionary<string, List<LambdaExpression>>();
            BufferedWheres = new List<LambdaExpression>();
            BufferedSortings = new List<OrderByExpression>();
        }

        public virtual void Clear()
        {
            Query = new Query(StructureSchema);
        }

        public virtual IQuery Build()
        {
            if (BufferedIncludes.Count > 0)
                Query.Includes = ParseIncludeLambdas(BufferedIncludes.ToArray());

            if (BufferedWheres.Count > 0)
                Query.Where = ParseWhereLambdas(BufferedWheres.ToArray());

            if (BufferedSortings.Count > 0)
                Query.Sortings = ParseSortingLambdas(BufferedSortings.ToArray());

            return Query;
        }

        protected virtual IList<IParsedLambda> ParseIncludeLambdas(IEnumerable<KeyValuePair<string, List<LambdaExpression>>> includes)
        {
            return includes.Select(keyValuePair =>
                ExpressionParsers.IncludeParser.Parse(keyValuePair.Key, keyValuePair.Value.ToArray())).ToList();
        }

        protected virtual IParsedLambda ParseWhereLambdas(IEnumerable<LambdaExpression> wheres)
        {
            var chainedWhere = new WhereExpressionChainer();

            foreach (var @where in wheres)
                chainedWhere.Chain(@where);

            return ExpressionParsers.WhereParser.Parse(chainedWhere);
        }

        protected virtual IParsedLambda ParseSortingLambdas(OrderByExpression[] sortings)
        {
            return ExpressionParsers.OrderByParser.Parse(sortings);
        }

        public virtual IQueryBuilder Take(int numOfStructures)
        {
            Ensure.That(numOfStructures, "numOfStructures").IsGt(0);

            Query.TakeNumOfStructures = numOfStructures;

            return this;
        }

        public virtual IQueryBuilder Page(int pageIndex, int pageSize)
        {
            Ensure.That(pageIndex, "pageIndex").IsGte(0);
            Ensure.That(pageSize, "pageSize").IsGt(0);

            Query.Paging = new Paging(pageIndex, pageSize);

            return this;
        }

        public virtual IQueryBuilder Include(Type includeType, params LambdaExpression[] expressions)
        {
            Ensure.That(expressions, "expressions").HasItems();

            var key = StructureSchemas.GetSchema(includeType).GetStructureTableName();

            if (!BufferedIncludes.ContainsKey(key))
                BufferedIncludes.Add(key, new List<LambdaExpression>(new LambdaExpression[0]));

            BufferedIncludes[key].AddRange(expressions);

            return this;
        }

        public virtual IQueryBuilder Where(params LambdaExpression[] expressions)
        {
            Ensure.That(expressions, "expressions").HasItems();

            BufferedWheres.AddRange(expressions);

            return this;
        }

        public virtual IQueryBuilder OrderBy(params LambdaExpression[] expressions)
        {
            Ensure.That(expressions, "expressions").HasItems();

            BufferedSortings.AddRange(expressions.Select(e => new OrderByAscExpression(e)));

            return this;
        }

        public virtual IQueryBuilder OrderByDescending(params LambdaExpression[] expressions)
        {
            Ensure.That(expressions, "expressions").HasItems();

            BufferedSortings.AddRange(expressions.Select(e => new OrderByDescExpression(e)));

            return this;
        }
    }
}