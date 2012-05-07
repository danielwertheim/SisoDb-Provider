using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Glimpse.Core.Extensibility;
using SisoDb.Diagnostics;

namespace SisoDb.Glimpse
{
    [GlimpsePlugin]
    public class SisoDbGlimpsePlugin : IGlimpsePlugin
    {
        public string Name
        {
            get { return "SisoDb"; }
        }

        public static Func<IEnumerable<ISisoDatabase>> ResolveDatabasesUsing;

        protected List<object[]> NonChangingContextData { get; set; }

        public virtual void SetupInit() { }

        public virtual object GetData(HttpContextBase context)
        {
            var data = new List<object[]> { new[] { "Context", " " } };

            AppendDbContextData(data);

            return data;
        }

        protected virtual void AppendDbContextData(List<object[]> output)
        {
            var contexts = OnResolveDatabases()
                .Select(db => new DbDiagnosticsContextBuilder(db).Build());

            AppendContextsData(output, contexts);
        }

        protected virtual IEnumerable<ISisoDatabase> OnResolveDatabases()
        {
            return ResolveDatabasesUsing == null
                ? Enumerable.Empty<ISisoDatabase>()
                : ResolveDatabasesUsing.Invoke();
        }

        protected void AppendContextsData(List<object[]> output, IEnumerable<DiagnosticsContext> contexts)
        {
            foreach (var diagnosticsContext in contexts)
            {
                var sections = new List<object[]> { new[] { diagnosticsContext.Name + ", contains", string.Empty } };
                foreach (var section in diagnosticsContext.Sections)
                {
                    var groups = new List<object[]> { new object[] { section.Name + ", contains", string.Empty } };
                    foreach (var group in section.Groups)
                    {
                        var groupNodes = new List<object[]> { new[] { group.Name + ", contains", "Value" } };
                        foreach (var node in group.Nodes)
                            groupNodes.Add(new[] { node.Name, node.Value });

                        if (groupNodes.Count > 1)
                            groups.Add(new object[] { section.Name == group.Name ? null : group.Name, groupNodes });
                    }

                    if (groups.Count > 1)
                        sections.Add(new object[] { section.Name, groups });

                    var sectionNodes = new List<object[]> { new object[] { section.Name + ", contains", "Value" } };
                    foreach (var node in section.Nodes)
                        sectionNodes.Add(new[] { node.Name, node.Value });

                    if (sectionNodes.Count > 1)
                        sections.Add(new object[] { section.Name, sectionNodes });
                }

                if (sections.Count > 1)
                    output.Add(new object[] { diagnosticsContext.Name, sections });
            }
        }
    }
}