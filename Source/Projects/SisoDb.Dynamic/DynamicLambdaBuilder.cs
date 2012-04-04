using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using EnsureThat;
using Mono.CSharp;
using NCore;
using SisoDb.Resources;

namespace SisoDb.Dynamic
{
    public class DynamicLambdaBuilder : IDisposable, IDynamicLambdaBuilder
    {
        protected const string QueryFormat = "ExpressionFactory.Create<{0}>({1});";

        protected readonly Evaluator Evaluator;
        protected readonly StringBuilder EvaluatorResult;
        private readonly ISet<int> _referencedAssemblyHashCodes; 
        private TextWriter _writer;
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

            _writer = new IndentedTextWriter(new StringWriter(EvaluatorResult));
            Evaluator = new Evaluator(new CompilerSettings { Target = Target.Module }, new Report(new StreamReportPrinter(_writer)));

            var expressionFactoryType = typeof(ExpressionFactory);
            ReferenceAssembly(expressionFactoryType.Assembly);
            Evaluator.Run(string.Format("using {0};", expressionFactoryType.Namespace));
        }

        public virtual void Dispose()
        {
            if (_writer != null)
            {
                _writer.Dispose();
                _writer = null;
            }
        }

        public virtual System.Linq.Expressions.LambdaExpression Build(Type type, string expression)
        {
            Ensure.That(type, "type").IsNotNull();
            Ensure.That(expression, "expression").IsNotNullOrWhiteSpace();

            var typeFullName = type.FullName;
            if (string.IsNullOrWhiteSpace(typeFullName))
                throw new SisoDbException(ExceptionMessages.DynamicLambdaBuilder_Build_TypeFullNameMissing.Inject(type.Name));

            ReferenceAssembly(type.Assembly);

            return OnBuild(typeFullName, expression);
        }

        protected virtual System.Linq.Expressions.LambdaExpression OnBuild(string typeFullName, string expression)
        {
            var query = string.Format(QueryFormat, typeFullName.Replace('+', '.'), expression);

            try
            {
                return Cache.GetOrAddExpression(query, () => (System.Linq.Expressions.LambdaExpression)Evaluator.Evaluate(query));
            }
            catch (Exception ex)
            {
                throw new SisoDbException(ExceptionMessages.DynamicLambdaBuilder_Build_Error.Inject(EvaluatorResult), new[] { ex });
            }
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