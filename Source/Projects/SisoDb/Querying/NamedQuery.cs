using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SisoDb.Dac;
using SisoDb.EnsureThat;

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
			Ensure.That(name, "name").IsNotNullOrWhiteSpace();

            Name = name;

            _parameters = new Dictionary<string, IDacParameter>();
        }

        public virtual void Add(params IDacParameter[] parameters)
        {
            foreach (var queryParameter in parameters)
                _parameters.Add(queryParameter.Name, queryParameter);
        }
    }
}