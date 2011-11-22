using System;
using EnsureThat;

namespace SisoDb.Querying.Sql
{
    [Serializable]
    public class SqlWhereMember
    {
        private readonly string _memberPath;
        private readonly string _alias;
        private readonly bool _isEmpty;

        public virtual string MemberPath
        {
            get { return _memberPath; }
        }

        public virtual string Alias
        {
            get { return _alias; }
        }

        public virtual bool IsEmpty
        {
            get { return _isEmpty; }
        }

        public SqlWhereMember(string memberPath, string alias)
        {
            Ensure.That(memberPath, "memberPath").IsNotNullOrWhiteSpace();
            Ensure.That(alias, "alias").IsNotNullOrWhiteSpace();

            _isEmpty = false;
            _memberPath = memberPath;
            _alias = alias;
        }

        private SqlWhereMember()
        {
            _isEmpty = true;
            _memberPath = string.Empty;
            _alias = string.Empty;
        }

        public static SqlWhereMember Empty()
        {
            return new SqlWhereMember();
        }
    }
}