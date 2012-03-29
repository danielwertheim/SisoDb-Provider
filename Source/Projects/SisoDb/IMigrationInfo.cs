using System;

namespace SisoDb
{
    public interface IMigrationInfo
    {
        Type From { get; }
        Type To { get; }
    }
}