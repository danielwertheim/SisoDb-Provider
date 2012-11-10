using System.Threading;
using SisoDb.Structures;
using SisoDb.Structures.IdGenerators;
using SisoDb.Structures.Schemas;

namespace SisoDb.UnitTests.Structures.StructureBuilderTests
{
    internal class GuidStructureIdGeneratorProxy : GuidStructureIdGenerator
    {
        internal int CallsToGenerateSingle;
        internal int CallsToGenerateMany;

        public override IStructureId Generate(IStructureSchema structureSchema)
        {
            Interlocked.Increment(ref CallsToGenerateSingle);
            return base.Generate(structureSchema);
        }

        public override IStructureId[] Generate(IStructureSchema structureSchema, int numOfIds)
        {
            Interlocked.Increment(ref CallsToGenerateMany);
            return base.Generate(structureSchema, numOfIds);
        }
    }
}