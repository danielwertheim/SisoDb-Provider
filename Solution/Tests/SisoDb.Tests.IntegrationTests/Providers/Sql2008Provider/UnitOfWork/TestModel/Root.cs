using System;

namespace SisoDb.Tests.IntegrationTests.Providers.Sql2008Provider.UnitOfWork.TestModel
{
    internal class Root
    {
        public Guid SisoId { get; set; }

        public string RootString1 { get; set; }

        public int RootInt1 { get; set; }

        public Nested Nested { get; set; }
    }
}