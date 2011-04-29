using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SisoDb.Querying
{
    [Serializable]
    public class NamedQuery : INamedQuery
    {
        private readonly IDictionary<string, IQueryParameter> _parameters;

        public string Name { get; private set; }

        public IEnumerable<IQueryParameter> Parameters
        {
            get { return new ReadOnlyCollection<IQueryParameter>(_parameters.Values.ToList()); }
        }

        public NamedQuery(string name)
        {
            if(string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");

            Name = name;

            _parameters = new Dictionary<string, IQueryParameter>();
        }

        public void Add(params IQueryParameter[] parameters)
        {
            foreach (var queryParameter in parameters)
                _parameters.Add(queryParameter.Name, queryParameter);
        }
    }
}