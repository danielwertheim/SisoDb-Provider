using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using SisoDb.Resources;

namespace SisoDb.Querying
{
    internal class QueryCommand<T> : IQueryCommand<T> where T : class
    {
        private ReadOnlyCollection<LambdaExpression> _sortings;

        public Expression<Func<T, bool>> Selector { get; private set; }

        public IEnumerable<LambdaExpression> Sortings
        {
            get { return _sortings; }
        }

        public bool HasSelector
        {
            get { return Selector != null; }
        }

        public bool HasSortings
        {
            get { return _sortings != null && _sortings.Count > 0; }
        }

        public QueryCommand()
        {
        }

        public QueryCommand(IEnumerable<LambdaExpression> sortings)
        {
            _sortings = new ReadOnlyCollection<LambdaExpression>(sortings.AssertNotNull("sortings").ToArray());
        }

        public IQueryCommand<T> Where(Expression<Func<T, bool>> selector)
        {
            if(Selector != null)
                throw new SisoDbException(ExceptionMessages.QueryCommand_WhereAllreadyInitialized);

            Selector = selector.AssertNotNull("selector");

            return this;
        }

        public IQueryCommand<T> SortBy(params Expression<Func<T, dynamic>>[] sortings)
        {
            if (Sortings != null)
                throw new SisoDbException(ExceptionMessages.QueryCommand_SortingsAllreadyInitialized);

            _sortings = new ReadOnlyCollection<LambdaExpression>(sortings.AssertNotNull("sortings").Cast<LambdaExpression>().ToArray());

            return this;
        }
    }
}