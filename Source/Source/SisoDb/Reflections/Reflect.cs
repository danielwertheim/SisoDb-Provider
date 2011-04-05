using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using SisoDb.Querying.Lambdas;

namespace SisoDb.Reflections
{
    public static class Reflect
    {
        public static Func<T, TOut> GetterFor<T, TOut>(PropertyInfo prop)
            where T : class
        {
            return Reflect<T>.GetterFor<TOut>(prop);
        }

        public static Func<T, TOut?> GetterForNullable<T, TOut>(PropertyInfo prop)
            where T : class
            where TOut : struct
        {
            return Reflect<T>.GetterForNullable<TOut>(prop);
        }

        public static Action<T, TIn> SetterFor<T, TIn>(PropertyInfo prop)
            where T : class
        {
            return Reflect<T>.SetterFor<TIn>(prop);
        }

        public static Action<T, TIn?> SetterForNullable<T, TIn>(PropertyInfo prop)
            where T : class
            where TIn : struct
        {
            return Reflect<T>.SetterForNullable<TIn>(prop);
        }
    }

    public static class Reflect<T> where T : class
    {
        public static readonly Type Type = typeof(T);
        private static readonly Dictionary<int, Delegate> CompiledGetters = new Dictionary<int, Delegate>();
        private static readonly Dictionary<int, Delegate> CompiledSetters = new Dictionary<int, Delegate>();

        public static Func<T, TOut> GetterFor<TOut>(PropertyInfo prop)
        {
            var key = prop.GetHashCode();

            if (CompiledGetters.ContainsKey(key))
                return (Func<T, TOut>)CompiledGetters[key];

            var itemParameter = Expression.Parameter(Type);
            var getter = Expression.Property(itemParameter, prop);
            var lambda = Expression.Lambda<Func<T, TOut>>(getter, string.Empty, new[] { itemParameter });

            var compiled = lambda.Compile();
            CompiledGetters.Add(key, compiled);

            return compiled;
        }

        public static Func<T, TOut?> GetterForNullable<TOut>(PropertyInfo prop)
            where TOut : struct
        {
            var key = prop.GetHashCode();

            if (CompiledGetters.ContainsKey(key))
                return (Func<T, TOut?>)CompiledGetters[key];

            var itemParameter = Expression.Parameter(Type);
            var getter = Expression.Property(itemParameter, prop);
            var lambda = Expression.Lambda<Func<T, TOut?>>(getter, string.Empty, new[] { itemParameter });

            var compiled = lambda.Compile();
            CompiledGetters.Add(key, compiled);

            return compiled;
        }

        public static Action<T, TIn> SetterFor<TIn>(PropertyInfo prop)
        {
            var key = prop.GetHashCode();

            if (CompiledSetters.ContainsKey(key))
                return (Action<T, TIn>)CompiledSetters[key];

            var itemParameter = Expression.Parameter(Type);
            var valueParameter = Expression.Parameter(typeof(TIn));
            var getter = Expression.Property(itemParameter, prop);
            var setter = Expression.Assign(getter, valueParameter);
            var lambda = Expression.Lambda<Action<T, TIn>>(setter, string.Empty, new[] { itemParameter, valueParameter });

            var compiled = lambda.Compile();
            CompiledSetters.Add(key, compiled);

            return compiled;
        }

        public static Action<T, TIn?> SetterForNullable<TIn>(PropertyInfo prop) where TIn : struct
        {
            var key = prop.GetHashCode();

            if (CompiledSetters.ContainsKey(key))
                return (Action<T, TIn?>)CompiledSetters[key];

            var itemParameter = Expression.Parameter(Type);
            var valueParameter = Expression.Parameter(typeof(TIn?));
            var getter = Expression.Property(itemParameter, prop);
            var setter = Expression.Assign(getter, valueParameter);
            var lambda = Expression.Lambda<Action<T, TIn?>>(setter, string.Empty, new[] { itemParameter, valueParameter });

            var compiled = lambda.Compile();
            CompiledSetters.Add(key, compiled);

            return compiled;
        }

        public static Expression<Func<T, bool>> BoolExpressionFrom(Expression<Func<T, bool>> e)
        {
            return e;
        }

        public static Expression<Func<T, TProp>> ExpressionFrom<TProp>(Expression<Func<T, TProp>> e)
        {
            return e;
        }

        public static LambdaExpression LambdaFrom(Expression<Action<T>> e)
        {
            return e;
        }

        public static LambdaExpression LambdaFrom<TProp>(Expression<Func<T, TProp>> e)
        {
            return e;
        }

        public static MemberExpression MemberFrom<TProp>(Expression<Func<T, TProp>> e)
        {
            return Expressions.GetRightMostMember(e.Body);
        }
    }
}