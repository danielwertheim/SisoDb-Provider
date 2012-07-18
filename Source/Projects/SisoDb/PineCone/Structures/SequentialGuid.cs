using System;
using System.Runtime.InteropServices;
using System.Security;

namespace PineCone.Structures
{
    /// <summary>
    /// <![CDATA[http://www.shirmanov.com/2010/05/generating-newsequentialid-compatible.html]]>
    /// </summary>
    public struct SequentialGuid
    {
        [SuppressUnmanagedCodeSecurity]
        [DllImport("rpcrt4.dll", SetLastError = true)]
        private static extern int UuidCreateSequential(out Guid value);

        private enum RpcUuidCodes
        {
            RPC_S_OK = 0,
            RPC_S_UUID_LOCAL_ONLY = 1824,
            RPC_S_UUID_NO_ADDRESS = 1739
        }

        public static Guid New()
        {
            var sequentialGuid = NewGuid();

            return ConvertToSqlCompatible(sequentialGuid);
        }

        private static Guid NewGuid()
        {
            Guid sequentialGuid;

            var resultCode = UuidCreateSequential(out sequentialGuid);

            switch (resultCode)
            {
                case (int)RpcUuidCodes.RPC_S_OK:
                    // all ok
                    break;
                case (int)RpcUuidCodes.RPC_S_UUID_LOCAL_ONLY:
                    throw new Exception(@"SequentialGuid:NewGuid failed - UuidCreateSequential returned RPC_S_UUID_LOCAL_ONLY");
                case (int)RpcUuidCodes.RPC_S_UUID_NO_ADDRESS:
                    throw new Exception(@"SequentialGuid:NewGuid failed - UuidCreateSequential returned RPC_S_UUID_NO_ADDRESS");
                default:
                    throw new Exception(String.Format(@"SequentialGuid:NewGuid failed - UuidCreateSequential returned {0}", resultCode));
            }

            return sequentialGuid;
        }

        private static Guid ConvertToSqlCompatible(Guid guid)
        {
            byte[] guidBytes = guid.ToByteArray();
            Array.Reverse(guidBytes, 0, 4);
            Array.Reverse(guidBytes, 4, 2);
            Array.Reverse(guidBytes, 6, 2);

            return new Guid(guidBytes);
        }
    }
}