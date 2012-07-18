using System.Threading.Tasks;
using PineCone.Serializers;
using PineCone.Structures.IdGenerators;
using PineCone.Structures.Schemas;
using SisoDb.EnsureThat;
using SisoDb.NCore;

namespace PineCone.Structures
{
    public class StructureBuilder : IStructureBuilder
    {
        private IStructureIndexesFactory _indexesFactory;
        private IStructureSerializer _structureSerializer;
        private IStructureIdGenerator _structureIdGenerator;

        public IStructureIndexesFactory IndexesFactory
        {
            get { return _indexesFactory; }
            set
            {
                Ensure.That(value, "IndexesFactory").IsNotNull();

                _indexesFactory = value;
            }
        }

        public IStructureSerializer StructureSerializer
        {
            get { return _structureSerializer; }
            set
            {
                Ensure.That(value, "StructureSerializer").IsNotNull();

                _structureSerializer = value;
            }
        }

        public IStructureIdGenerator StructureIdGenerator
        {
            get { return _structureIdGenerator; }
            set
            {
                Ensure.That(value, "StructureIdGenerator").IsNotNull();

                _structureIdGenerator = value;
            }
        }

        public StructureBuilder()
        {
            IndexesFactory = new StructureIndexesFactory();
            StructureSerializer = new EmptyStructureSerializer();
            StructureIdGenerator = new SequentialGuidStructureIdGenerator();
        }

        public virtual IStructure CreateStructure<T>(T item, IStructureSchema structureSchema) where T : class
        {
            var structureId = StructureIdGenerator.Generate(structureSchema);

            structureSchema.IdAccessor.SetValue(item, structureId);

            if (structureSchema.HasTimeStamp)
                structureSchema.TimeStampAccessor.SetValue(item, SysDateTime.Now);

            return new Structure(
                structureSchema.Name,
                structureId,
                IndexesFactory.CreateIndexes(structureSchema, item, structureId),
                StructureSerializer.Serialize(item));
        }

		public virtual IStructure[] CreateStructures<T>(T[] items, IStructureSchema structureSchema) where T : class
		{
			return items.Length < 100
			       	? CreateStructuresInSerial(items, structureSchema)
			       	: CreateStructuresInParallel(items, structureSchema);
		}

    	protected virtual IStructure[] CreateStructuresInParallel<T>(T[] items, IStructureSchema structureSchema) where T : class
		{
			var structureIds = StructureIdGenerator.Generate(structureSchema, items.Length);
			var structures = new IStructure[items.Length];
            var timeStamp = SysDateTime.Now;
    	    
			Parallel.For(0, items.Length, i =>
			{
				var id = structureIds[i];
				var itm = items[i];

				structureSchema.IdAccessor.SetValue(itm, id);

                if (structureSchema.HasTimeStamp)
                    structureSchema.TimeStampAccessor.SetValue(itm, timeStamp);

				structures[i] = new Structure(
					structureSchema.Name,
					id,
					IndexesFactory.CreateIndexes(structureSchema, itm, id),
					StructureSerializer.Serialize(itm));
			});

			return structures;
		}

		protected virtual IStructure[] CreateStructuresInSerial<T>(T[] items, IStructureSchema structureSchema) where T : class
		{
			var structures = new IStructure[items.Length];
            var timeStamp = SysDateTime.Now;

            for(var i = 0; i < structures.Length; i++)
			{
				var id = StructureIdGenerator.Generate(structureSchema);
				var itm = items[i];

				structureSchema.IdAccessor.SetValue(itm, id);

                if(structureSchema.HasTimeStamp)
                    structureSchema.TimeStampAccessor.SetValue(itm, timeStamp);

				structures[i] = new Structure(
					structureSchema.Name,
					id,
					IndexesFactory.CreateIndexes(structureSchema, itm, id),
					StructureSerializer.Serialize(itm));
			}

			return structures;
		}
    }
}