using System;
using SisoDb.NCore;
using SisoDb.PineCone.Structures;
using SisoDb.Resources;

namespace SisoDb.Querying.Lambdas.Nodes
{
    [Serializable]
    public class StringExactMemberNode : MemberNode, IStringOperationMemberNode
    {
        public string Value { get; private set; }

        public StringExactMemberNode(string memberPath, Type dataType, DataTypeCode dataTypeCode, string value)
            : base(memberPath, dataType, dataTypeCode)
        {
            if (DataTypeCode == DataTypeCode.Text)
                throw new SisoDbNotSupportedException(ExceptionMessages.QxIsExactly_NotSupportedForTexts.Inject(Path));

            Value = value;
        }
    }
}