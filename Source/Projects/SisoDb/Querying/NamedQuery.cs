using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SisoDb.Dac;

namespace SisoDb.Querying
{
    [Serializable]
    public class NamedQuery : INamedQuery
    {
        private readonly IDictionary<string, IDacParameter> _parameters;

        public string Name { get; private set; }

        public IEnumerable<IDacParameter> Parameters
        {
            get { return new ReadOnlyCollection<IDacParameter>(_parameters.Values.ToList()); }
        }

        public NamedQuery(string name)
        {
            if(string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");

            Name = name;

            _parameters = new Dictionary<string, IDacParameter>();
        }

        public void Add(params IDacParameter[] parameters)
        {
            foreach (var queryParameter in parameters)
                _parameters.Add(queryParameter.Name, queryParameter);
        }
    }
}