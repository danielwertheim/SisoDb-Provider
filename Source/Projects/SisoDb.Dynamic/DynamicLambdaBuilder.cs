using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Mono.CSharp;
using SisoDb.EnsureThat;
using SisoDb.NCore;
using SisoDb.Resources;

namespace SisoDb.Dynamic
{
    public class DynamicLambdaBuilder : IDynamicLambdaBuilder
    {
        protected const string CreatePredicateFormat = "ExpressionFactory.CreatePredicate<{0}>({1});";
        protected const string CreateMemberFormat = "ExpressionFactory.CreateMember<{0}>({1});";

        protected CompilerContext CompilerContext;
        protected readonly Evaluator Evaluator;
        protected readonly StringBuilder EvaluatorResult;
        private readonly ISet<int> _referencedAssemblyHashCodes; 
        private IDynamicLambdaBuilderCache _cache;
        
        public IDynamicLambdaBuilderCache Cache
        {
            get { return _cache; }
            set
            {
                Ensure.That(value, "Cache").IsNotNull();
                _cache = value;
            }
        }

        public DynamicLambdaBuilder()
        {
            EvaluatorResult = new StringBuilder();
            Cache = new DynamicLambdaBuilderCache();
            _referencedAssemblyHashCodes = new HashSet<int>();

            CompilerContext = new CompilerContext(
                new CompilerSettings { Target = Target.Module },
                new StreamReportPrinter(new IndentedTextWriter(new StringWriter(EvaluatorResult))));
            Evaluator = new Evaluator(CompilerContext);

            var expressionFactoryType = typeof(ExpressionFactory);
            ReferenceAssembly(expressionFactoryType.Assembly);
            Evaluator.Run(string.Format("using {0};", expressionFactoryType.Namespace));
        }

        public virtual Expression<Func<T, bool>>  BuildPredicate<T>(string expression, params object[] formattingArgs) where T : class
        {
            return (Expression<Func<T, bool>>) BuildPredicate(typeof(T), expression, formattingArgs);
        }

        public virtual System.Linq.Expressions.LambdaExpression BuildPredicate(Type type, string expression, params object[] formattingArgs)
        {
            Ensure.That(type, "type").IsNotNull();
            Ensure.That(expression, "expression").IsNotNullOrWhiteSpace();

            var typeFullName = type.FullName;
            if (string.IsNullOrWhiteSpace(typeFullName))
                throw new SisoDbException(ExceptionMessages.DynamicLambdaBuilder_Build_TypeFullNameMissing.Inject(type.Name));

            ReferenceAssembly(type.Assembly);

            if (formattingArgs.Any())
                expression = string.Format(expression, formattingArgs);

            return OnBuild(typeFullName, expression, CreatePredicateFormat);
        }

        public virtual Expression<Func<T, bool>> BuildMember<T>(string expression, params object[] formattingArgs) where T : class
        {
            return (Expression<Func<T, bool>>)BuildMember(typeof(T), expression, formattingArgs);
        }

        public System.Linq.Expressions.LambdaExpression BuildMember(Type type, string expression, params object[] formattingArgs)
        {
            Ensure.That(type, "type").IsNotNull();
            Ensure.That(expression, "expression").IsNotNullOrWhiteSpace();

            var typeFullName = type.FullName;
            if (string.IsNullOrWhiteSpace(typeFullName))
                throw new SisoDbException(ExceptionMessages.DynamicLambdaBuilder_Build_TypeFullNameMissing.Inject(type.Name));

            ReferenceAssembly(type.Assembly);

            if (formattingArgs.Any())
                expression = string.Format(expression, formattingArgs);

            return OnBuild(typeFullName, expression, CreateMemberFormat);
        }

        protected virtual System.Linq.Expressions.LambdaExpression OnBuild(string typeFullName, string expression, string createExpressionFormat)
        {
            EnsureValidLambdaExpression(expression);

            var query = string.Format(createExpressionFormat, typeFullName.Replace('+', '.'), expression);

            try
            {
                return Cache.GetOrAddExpression(query, () => (System.Linq.Expressions.LambdaExpression)Evaluator.Evaluate(query));
            }
            catch (Exception ex)
            {
                throw new SisoDbException(ExceptionMessages.DynamicLambdaBuilder_Build_Error.Inject(EvaluatorResult), new[] { ex });
            }
        }

        protected virtual void EnsureValidLambdaExpression(string expression)
        {
            var firstSpaceIndex = expression.IndexOf(' ');
            if (firstSpaceIndex < 1)
                throw new ArgumentException(ExceptionMessages.DynamicLambdaBuilder_InvalidExpressionFormat);

            var lambdaOperatorIndex = expression.IndexOf("=> ", firstSpaceIndex, StringComparison.OrdinalIgnoreCase);
            if (lambdaOperatorIndex < 0)
                throw new ArgumentException(ExceptionMessages.DynamicLambdaBuilder_InvalidExpressionFormat);
        }

        protected void ReferenceAssembly(Assembly assembly)
        {
            OnReferenceAssemblyIfMissing(assembly);
        }

        protected virtual void OnReferenceAssemblyIfMissing(Assembly assembly)
        {
            if (!_referencedAssemblyHashCodes.Add(assembly.GetHashCode()))
                return;

            Evaluator.ReferenceAssembly(assembly);
        }
    }
}