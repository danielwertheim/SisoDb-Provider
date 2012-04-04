using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace SisoDb.Dynamic
{
    public class DynamicLambdaBuilderCache : IDynamicLambdaBuilderCache
    {
        private readonly ConcurrentDictionary<int, LambdaExpression> _state;

        public DynamicLambdaBuilderCache()
        {
            _state = new ConcurrentDictionary<int, LambdaExpression>();
        }

        public virtual LambdaExpression GetOrAddExpression(string query, Func<LambdaExpression> fn)
        {
            var key = GenerateKey(query);
            return _state.ContainsKey(key)
                       ? _state[key]
                       : _state.GetOrAdd(key, fn.Invoke());
        }

        private int GenerateKey(string query)
        {
            return query.GetHashCode();
        }
    }
}