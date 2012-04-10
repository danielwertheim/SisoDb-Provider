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
        /// Lets you build a <see cref="LambdaExpression"/> predicate from sent type
        /// and string. Example: <example>BuildPredicate&lt;Person&gt;("p => p.Name == \"Daniel\"")</example>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="formattingArgs"></param>
        /// <returns></returns>
        Expression<Func<T, bool>> BuildPredicate<T>(string expression, params object[] formattingArgs) where T : class;

        /// <summary>
        /// Lets you build a <see cref="LambdaExpression"/> predicate from sent Type
        /// and string. The type will be used as the generic type in the
        /// predicate. Example: <example>BuildPredicate(typeof(Person), "p => p.Name == \"Daniel\"")</example>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="expression"></param>
        /// <param name="formattingArgs"></param>
        /// <returns></returns>
        LambdaExpression BuildPredicate(Type type, string expression, params object[] formattingArgs);

        /// <summary>
        /// Lets you build a <see cref="LambdaExpression"/> predicate from sent type
        /// and string. Example: <example>BuildMember&lt;Person&gt;("p => p.Name")</example>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="formattingArgs"></param>
        /// <returns></returns>
        Expression<Func<T, bool>> BuildMember<T>(string expression, params object[] formattingArgs) where T : class;

        /// <summary>
        /// Lets you build a <see cref="LambdaExpression"/> predicate from sent Type
        /// and string. The type will be used as the generic type in the
        /// predicate. Example: <example>BuildMember(typeof(Person), "p => p.Name")</example>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="expression"></param>
        /// <param name="formattingArgs"></param>
        /// <returns></returns>
        LambdaExpression BuildMember(Type type, string expression, params object[] formattingArgs);
    }
}