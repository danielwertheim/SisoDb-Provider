using System;
using System.Collections.Generic;
using System.Linq;
using Glimpse.Core.Extensibility;
using SisoDb.Diagnostics;
using SisoDb.Diagnostics.Builders;

namespace SisoDb.Glimpse
{
    public class SisoDbGlimpsePlugin : TabBase
    {
        public override string Name
        {
            get { return "SisoDb"; }
        }

        public static Func<IEnumerable<ISisoDatabase>> ResolveDatabasesUsing;

        protected List<object[]> NonChangingContextData { get; set; }

        public virtual void SetupInit() { }

        public override object GetData(ITabContext context)
        {
            var data = new List<object[]>();

            AppendDbDiagnostics(data);

            return data;
        }

        protected virtual void AppendDbDiagnostics(List<object[]> output)
        {
            var sections = OnResolveDatabases()
                .Select(db => new DbDiagnosticsBuilder(db).Build());

            output.Add(new[] { "Section", string.Empty });
            foreach (var section in sections)
                AppendSection(output, section);
        }

        protected virtual IEnumerable<ISisoDatabase> OnResolveDatabases()
        {
            return ResolveDatabasesUsing == null
                ? Enumerable.Empty<ISisoDatabase>()
                : ResolveDatabasesUsing.Invoke();
        }

        protected void AppendSection(List<object[]> output, DiagnosticsInfo section)
        {
            var sectionContents = new List<object[]> { new object[] { "Contains", "" } };
            foreach (var node in section.Nodes)
                sectionContents.Add(new object[] { node.Name, node.Value });

            foreach (var group in section.Groups)
            {
                var groupContents = GetGroupContents(group);
                if(groupContents.Count > 1)
                    sectionContents.Add(new object[] { group.Name, groupContents });
            }

            if(sectionContents.Count > 1)
                output.Add(new object[] { section.Name, sectionContents });
        }

        public List<object[]> GetGroupContents(DiagnosticsGroup group)
        {
            var groupContents = new List<object[]> { new object[] { "Contains", "" } };

            foreach (var node in group.Nodes)
                groupContents.Add(new object[] { node.Name, node.Value });

            foreach (var childGroup in group.Groups)
            {
                var childGroupContents = GetGroupContents(childGroup);
                if(childGroupContents.Count > 1)
                    groupContents.Add(new object[] { childGroup.Name, childGroupContents });
            }

            

            return groupContents;
        }
    }
}