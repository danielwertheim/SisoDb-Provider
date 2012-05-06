using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Glimpse.Core.Extensibility;
using SisoDb.Diagnostics;

namespace SisoDb.Glimpse
{
    [GlimpsePlugin(ShouldSetupInInit = true)]
    public class SisoDbGlimpsePlugin : IGlimpsePlugin
    {
        public string Name
        {
            get { return "SisoDb"; }
        }

        public static Func<IEnumerable<ISisoDatabase>> ResolveDatabasesUsing;

        protected List<object[]> NonChangingContextData { get; set; }

        public void SetupInit()
        {
            NonChangingContextData = BuildNonChanginContextData();
        }

        private List<object[]> BuildNonChanginContextData()
        {
            var contexts = OnResolveDatabases()
                .Select(db => new DbDiagnosticsContextBuilder(db).Build()).ToArray();

            var data = new List<object[]> { new[] { "Key", "Value" } };
            foreach (var diagnosticsContext in contexts)
            {
                data.Add(new[] { "Context", diagnosticsContext.Name });

                foreach (var section in diagnosticsContext.Sections)
                {
                    var groups = new List<object> { new[] { "Name", string.Empty } };
                    foreach (var group in section.Groups)
                    {
                        var nodes = new List<object> { new[] { "Name", "Value" } };
                        foreach (var node in group.Nodes)
                            nodes.Add(new[] { node.Name, node.Value });

                        groups.Add(new object[] { @group.Name, nodes });
                    }

                    data.Add(new object[] { section.Name, groups });
                }
            }
            return data;
        }

        public object GetData(HttpContextBase context)
        {
            return NonChangingContextData;
        }

        protected virtual IEnumerable<ISisoDatabase> OnResolveDatabases()
        {
            return ResolveDatabasesUsing == null
                ? Enumerable.Empty<ISisoDatabase>()
                : ResolveDatabasesUsing.Invoke();
        }
    }
}