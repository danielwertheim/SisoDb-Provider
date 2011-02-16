using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using SisoDb.Resources;

namespace SisoDb.Querying
{
    public class GetCommand<T> : IGetCommand<T> where T : class
    {
        private ReadOnlyCollection<LambdaExpression> _sortings;

        public IEnumerable<LambdaExpression> Sortings
        {
            get { return _sortings; }
        }

        public bool HasSortings
        {
            get { return _sortings != null && _sortings.Count > 0; }
        }

        public IGetCommand<T> SortBy(params Expression<Func<T, object>>[] sortings)
        {
            if (Sortings != null)
                throw new SisoDbException(ExceptionMessages.GetCommand_SortingsAllreadyInitialized);

            _sortings = new ReadOnlyCollection<LambdaExpression>(sortings.AssertNotNull("sortings").Cast<LambdaExpression>().ToArray());

            return this;
        }
    }
}