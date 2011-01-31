using System;

namespace SisoDb.Tests.IntegrationTests.Providers.SqlProvider.UnitOfWork.TestModel
{
    internal class Root
    {
        public Guid Id { get; set; }

        public string RootString1 { get; set; }

        public int RootInt1 { get; set; }

        public Nested Nested { get; set; }
    }
}