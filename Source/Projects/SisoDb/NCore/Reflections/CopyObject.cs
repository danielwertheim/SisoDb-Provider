using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace NCore.Reflections
{
    public static class CopyObject
    {
        public static T Deep<T>(T source)
        {
            var t = typeof(T);
            if (!t.IsSerializable)
                throw new ArgumentException("DeepCopy of Type '{0}' can not be performed. Type is not serializable.".Inject(t.Name));

            if (ReferenceEquals(source, null))
                return default(T);

            T clone;

            using (var buff = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(buff, source);

                buff.Seek(0, SeekOrigin.Begin);
                clone = (T)formatter.Deserialize(buff);
            }

            return clone;
        }
    }
}