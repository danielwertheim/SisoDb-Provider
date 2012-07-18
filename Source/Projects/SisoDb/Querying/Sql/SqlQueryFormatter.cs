using System.Collections.Generic;
using System.Text;
using SisoDb.NCore;

namespace SisoDb.Querying.Sql
{
    public class SqlQueryFormatter
    {
        private const string StartPart = "Start";
        private const string EndPart = "End";

        private const string TakePart = "Take";
        private const string IncludedJsonMembersPart = "IncludedJsonMembers";
        private const string OrderByMembersPart = "OrderByMembers";
        private const string MainStructureTablePart = "MainStructureTable";
        private const string WhereAndSortingJoinsPart = "WhereAndSortingJoins";
        private const string WhereCriteriaPart = "WhereCriteria";
        private const string IncludesJoinsPart = "IncludesJoins";
        private const string OrderByPart = "OrderBy";
        private const string PagingPart = "Paging";

        private readonly Dictionary<string, string> _parts;

        public SqlQueryFormatter()
        {
            _parts = new Dictionary<string, string>
            {
                {StartPart, ""},
                {TakePart, ""},
                {IncludedJsonMembersPart, ""},
                {OrderByMembersPart, ""},
                {MainStructureTablePart, ""},
                {WhereAndSortingJoinsPart, ""},
                {IncludesJoinsPart, ""},
                {OrderByPart, ""},
                {PagingPart, ""},
                {WhereCriteriaPart, ""},
                {EndPart, ""},
            };
        }

        public string Start
        {
            set { SetValue(StartPart, value.AppendWith(" ")); }
        }

        public string End
        {
            set { SetValue(EndPart, value.PrependWith(" ")); }
        }

        public string Take
        {
            set { SetValue(TakePart, value.AppendWith(" ")); }
        }

        public string IncludedJsonMembers
        {
            set { SetValue(IncludedJsonMembersPart, value.PrependWith(", ")); }
        }

        public string OrderByMembers
        {
            set { SetValue(OrderByMembersPart, value.PrependWith(", ")); }
        }

        public string MainStructureTable
        {
            set { SetValue(MainStructureTablePart, value); }
        }

        public string WhereAndSortingJoins
        {
            set { SetValue(WhereAndSortingJoinsPart, value.PrependWith(" ")); }
        }

        public string WhereCriteria
        {
            set { SetValue(WhereCriteriaPart, value.PrependWith(" where ")); }
        }

        public string IncludesJoins
        {
            set { SetValue(IncludesJoinsPart, value.PrependWith(" ")); }
        }

        public string OrderBy
        {
            set { SetValue(OrderByPart, value.PrependWith(" order by ")); }
        }

        public string Paging
        {
            set { SetValue(PagingPart, value.PrependWith(" ")); }
        }

        private void SetValue(string key, string value)
        {
            _parts[key] = value ?? string.Empty;
        }

        public string Format(string template)
        {
            var t = new StringBuilder(template);

            foreach (var part in _parts)
                t = t.Replace(string.Format("[%{0}%]", part.Key), part.Value);

            return t.ToString().Replace("\r\n", "").Trim();
        }
    }
}