using System;
using NCore;
using SisoDb.Resources;

namespace SisoDb
{
    [Serializable]
    public class SisoDbNotSupportedByProviderException : SisoDbException
    {
        public SisoDbNotSupportedByProviderException(StorageProviders storageProvider, string operation)
            : base(ExceptionMessages.SisoDbNotSupportedByProviderException.Inject(storageProvider, operation))
        {
        }
    }
}