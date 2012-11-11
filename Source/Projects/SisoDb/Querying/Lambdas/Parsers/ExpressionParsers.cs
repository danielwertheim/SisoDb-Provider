using SisoDb.EnsureThat;
using SisoDb.Structures.Schemas;

namespace SisoDb.Querying.Lambdas.Parsers
{
	public class ExpressionParsers : IExpressionParsers
	{
        private IWhereParser _whereParser;
        private IOrderByParser _orderByParser;

		public ExpressionParsers(IDataTypeConverter dataTypeConverter)
		{
			WhereParser = new WhereParser(dataTypeConverter);
			OrderByParser = new OrderByParser(dataTypeConverter);
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