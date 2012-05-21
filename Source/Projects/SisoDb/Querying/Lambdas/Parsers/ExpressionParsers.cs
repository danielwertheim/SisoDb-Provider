using EnsureThat;
using PineCone.Structures.Schemas;

namespace SisoDb.Querying.Lambdas.Parsers
{
	public class ExpressionParsers : IExpressionParsers
	{
        private IIncludeParser _includeParser;
        private IWhereParser _whereParser;
        private IOrderByParser _orderByParser;

		public ExpressionParsers(IDataTypeConverter dataTypeConverter)
		{
			IncludeParser = new IncludeParser(dataTypeConverter);
			WhereParser = new WhereParser(dataTypeConverter);
			OrderByParser = new OrderByParser(dataTypeConverter);
		}

	    public IIncludeParser IncludeParser
	    {
	        get
	        {
	            return _includeParser;
	        }
            set
            {
                Ensure.That(value, "IncludeParser").IsNotNull();
                _includeParser = value;
            }
	    }

	    
	    public IWhereParser WhereParser
	    {
	        get
	        {
	            return _whereParser;
	        }
	        set
	        {
	            Ensure.That(value, "WhereParser").IsNotNull();
	            _whereParser = value;
	        }
	    }

	    
	    public IOrderByParser OrderByParser
	    {
	        get
	        {
	            return _orderByParser;
	        }
	        set
	        {
	            Ensure.That(value, "OrderByParser").IsNotNull();
	            _orderByParser = value;
	        }
	    }
	}
}