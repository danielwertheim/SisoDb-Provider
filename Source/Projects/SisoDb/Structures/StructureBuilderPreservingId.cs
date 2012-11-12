using System.Threading.Tasks;
using SisoDb.NCore;
using SisoDb.Serialization;
using SisoDb.Structures.IdGenerators;
using SisoDb.Structures.Schemas;

namespace SisoDb.Structures
{
    public class StructureBuilderPreservingId : StructureBuilder
    {
        public StructureBuilderPreservingId()
        {
            IndexesFactory = new StructureIndexesFactory();
            StructureSerializer = new EmptyStructureSerializer();
            StructureIdGenerator = new EmptyStructureIdGenerator();
        }

        public override IStructure CreateStructure<T>(T item, IStructureSchema structureSchema)
        {
            var structureId = structureSchema.IdAccessor.GetValue(item);
            
            if (structureSchema.HasTimeStamp)
                structureSchema.TimeStampAccessor.SetValue(item, SysDateTime.Now);

            return new Structure(
                structureSchema.Name,
                structureId,
                IndexesFactory.CreateIndexes(structureSchema, item, structureId),
                StructureSerializer.Serialize(item, structureSchema));
        }

		protected override IStructure[] CreateStructuresInParallel<T>(T[] items, IStructureSchema structureSchema)
		{
			var structures = new IStructure[items.Length];
            var timeStamp = SysDateTime.Now;

			Parallel.For(0, items.Length, i =>
			{
				var itm = items[i];
				var structureId = structureSchema.IdAccessor.GetValue(itm);

                if (structureSchema.HasTimeStamp)
                    structureSchema.TimeStampAccessor.SetValue(itm, timeStamp);

				structures[i] = new Structure(
					structureSchema.Name,
					structureId,
					IndexesFactory.CreateIndexes(structureSchema, itm, structureId),
					StructureSerializer.Serialize(itm, structureSchema));
			});

			return structures;
		}

		protected override IStructure[] CreateStructuresInSerial<T>(T[] items, IStructureSchema structureSchema)
		{
			var structures = new IStructure[items.Length];
            var timeStamp = SysDateTime.Now;

			for (var i = 0; i < structures.Length; i++)
			{
				var itm = items[i];
				var structureId = structureSchema.IdAccessor.GetValue(itm);

                if (structureSchema.HasTimeStamp)
                    structureSchema.TimeStampAccessor.SetValue(itm, timeStamp);

				structures[i] = new Structure(
					structureSchema.Name,
					structureId,
					IndexesFactory.CreateIndexes(structureSchema, itm, structureId),
					StructureSerializer.Serialize(itm, structureSchema));
			}

			return structures;
		}
    }
}