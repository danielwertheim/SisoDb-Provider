using System;
using System.Linq.Expressions;

namespace SisoDb.Dynamic
{
    public interface IDynamicLambdaBuilder 
    {
        /// <summary>
        /// Contains precompiled and cached <see cref="LambdaExpression"/>.
        /// </summary>
        IDynamicLambdaBuilderCache Cache { get; set; }

        /// <summary>
        /// Lets you generate a <see cref="LambdaExpression"/> predicate from sent type
        /// and string. Example: <example>Build&lt;Person&gt;("p => p.Name == \"Daniel\"")</example>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="formattingArgs"></param>
        /// <returns></returns>
        Expression<Func<T, bool>> Build<T>(string expression, params object[] formattingArgs) where T : class;

        /// <summary>
        /// Lets you generate a <see cref="LambdaExpression"/> predicate from sent Type
        /// and string. The type will be used as the generic type in the
        /// predicate. Example: <example>Build(typeof(Person), "p => p.Name == \"Daniel\"")</example>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="expression"></param>
        /// <param name="formattingArgs"></param>
        /// <returns></returns>
        LambdaExpression Build(Type type, string expression, params object[] formattingArgs);
    }
}