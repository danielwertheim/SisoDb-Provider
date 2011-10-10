using System;

namespace SisoDb.Tests.IntegrationTests.Sql2008.UnitOfWork.TestModel
{
    internal class Root
    {
        public Guid StructureId { get; set; }

        public string RootString1 { get; set; }

        public int RootInt1 { get; set; }

        public Nested Nested { get; set; }
    }
}