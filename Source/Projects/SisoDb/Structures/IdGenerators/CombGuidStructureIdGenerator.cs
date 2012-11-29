using System;
using SisoDb.Structures.Schemas;

namespace SisoDb.Structures.IdGenerators
{
    public class CombGuidStructureIdGenerator : IStructureIdGenerator 
    {
        public virtual IStructureId Generate(IStructureSchema structureSchema)
        {
            return StructureId.Create(GenerateComb());
        }

        public virtual IStructureId[] Generate(IStructureSchema structureSchema, int numOfIds)
        {
            var structureIds = new IStructureId[numOfIds];

            for (var c = 0; c < structureIds.Length; c++)
                structureIds[c] = StructureId.Create(GenerateComb());

            return structureIds;
        }

        protected virtual Guid GenerateComb()
        {
            var guidArray = Guid.NewGuid().ToByteArray();
            var baseDate = new DateTime(1900, 1, 1);
            var now = DateTime.Now;

            var days = new TimeSpan(now.Ticks - baseDate.Ticks);
            var msecs = now.TimeOfDay;

            var daysArray = BitConverter.GetBytes(days.Days);
            var msecsArray = BitConverter.GetBytes((long)(msecs.TotalMilliseconds / 3.333333));

            Array.Reverse(daysArray);
            Array.Reverse(msecsArray);

            Array.Copy(daysArray, daysArray.Length - 2, guidArray, guidArray.Length - 6, 2);
            Array.Copy(msecsArray, msecsArray.Length - 4, guidArray, guidArray.Length - 4, 4);

            return new Guid(guidArray);
        }
    }
}