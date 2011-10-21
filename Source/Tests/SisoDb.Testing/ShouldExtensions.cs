using System;
using Machine.Specifications;
using NCore;
using NCore.Reflections;

namespace SisoDb.Testing
{
    public static class ShouldExtensions
    {
        public static void ShouldBeValueEqualTo<T>(this T x, T y)
        {
            AreValueEqual(typeof(T), x, y);
        }

        private static void AreValueEqual(Type type, object a, object b)
        {
            if (ReferenceEquals(a, b))
                return;

            if (a == null && b == null)
                return;

            if (a == null || b == null)
                a.ShouldEqual(b);

            if (type.IsEnumerableType())
            {
                if (type.GetElementType().IsSimpleType())
                {
                    var array1 = a as Array;
                    array1.ShouldNotBeNull();

                    var array2 = b as Array;
                    array2.ShouldNotBeNull();

                    for (var i = 0; i < array1.Length; i++)
                    {
                        var v1 = array1.GetValue(i);
                        var v2 = array2.GetValue(i);
                        AreValueEqual(v1.GetType(), v1, v2);
                    }
                }
                return;
            }

            if (type == typeof(object))
                throw new SpecificationException("You need to specify type to do the value equality comparision.");

            if (type.IsSimpleType())
            {
                a.ShouldEqual(b);
                return;
            }

            foreach (var propertyInfo in type.GetProperties())
            {
                var propertyType = propertyInfo.PropertyType;
                var valueForA = propertyInfo.GetValue(a, null);
                var valueForB = propertyInfo.GetValue(b, null);

                var isSimpleType = propertyType.IsSimpleType();
                if (isSimpleType)
                {
                    if(!Equals(valueForA, valueForB))
                        throw new SpecificationException("Values in property '{0}' doesn't match.".Inject(propertyInfo.Name));
                }
                else
                    AreValueEqual(propertyType, valueForA, valueForB);
            }
        }
    }
}