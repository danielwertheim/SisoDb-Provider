using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NCore;
using NCore.Reflections;
using NUnit.Framework;

namespace SisoDb.TestUtils
{
    public static class CustomAssert
    {
        public static void IsEmpty<T>(IEnumerable<T> source)
        {
            var isEmpty = source.Count() < 1;

            if (!isEmpty)
                throw new AssertionException("The enumerable was not empty");
        }

        public static void IsNotEmpty<T>(IEnumerable<T> source)
        {
            var isEmpty = source.Count() < 1;

            if (isEmpty)
                throw new AssertionException("The enumerable was empty");
        }

        public static void Count<T>(int expectedCount, IEnumerable<T> elements)
        {
            Assert.AreEqual(expectedCount, elements.Count());
        }

        public static void Exists<T>(IEnumerable<T> elements, Func<T, bool> predicate)
        {
            var element = elements.SingleOrDefault(predicate);

            if (Equals(element, null))
                throw new AssertionException("No element matches the predicate.");
        }

        public static void ForAll<T>(IEnumerable<T> elements, Func<T, bool> predicate)
        {
            var allCount = elements.Count();
            var predicateCount = elements.Count(predicate);

            if (allCount != predicateCount)
                throw new AssertionException("All items did not meet the predicate.");
        }

        public static void AreEqual<T>(IEnumerable<T> expectedItems, IEnumerable<T> actualItems, Func<T, T, bool> comparer)
        {
            Assert.AreEqual(expectedItems.Count(), actualItems.Count(), "Different numer of elements.");

            var xs = expectedItems.ToList();
            var ys = actualItems.ToList();
            for (var c = 0; c < expectedItems.Count(); c++)
            {
                var x = xs[c];
                var y = ys[c];

                var areEqual = comparer.Invoke(x, y);
                Assert.IsTrue(areEqual, "Elements at position '{0}' are not equal.".Inject(c));
            }
        }

        public static void AreValueEqual<T>(T expected, T actual) where T : class
        {
            AreValueEqual(typeof(T), expected, actual);
        }

        public static void KeyValueEquality(IDictionary<string, object> expected, IDictionary<string, object> actual)
        {
            foreach (var expectedKV in expected)
            {
                var expectedKey = expectedKV.Key;
                Assert.IsTrue(actual.ContainsKey(expectedKey), "Expected key '{0}' is missing.".Inject(expectedKey));

                var actualValue = actual[expectedKey];
                AreValueEqual(expectedKV.Value.GetType(), expectedKV.Value, actualValue);
            }
        }

        private static void AreValueEqual(Type type, object a, object b)
        {
            if (ReferenceEquals(a, b))
                return;

            if (a == null && b == null)
                return;

            if (a == null || b == null)
                Assert.AreEqual(a, b); //Force exception

            if (type.IsEnumerableType())
            {
                if(type.GetElementType().IsSimpleType())
                    CollectionAssert.AreEquivalent(a as IEnumerable, b as IEnumerable);
                else
                {
                    var array1 = a as Array;
                    Assert.IsNotNull(array1);

                    var array2 = b as Array;
                    Assert.IsNotNull(array2);
                    
                    for(var i = 0; i < array1.Length; i++)
                    {
                        var v1 = array1.GetValue(i);
                        var v2 = array2.GetValue(i);
                        AreValueEqual(v1.GetType(), v1,v2);
                    }
                }
                return;
            }

            if (type == typeof(object))
                throw new Exception("You need to specify type to do the value equality comparision.");

            if (type.IsSimpleType())
            {
                Assert.AreEqual(a, b);
                return;
            }

            foreach (var propertyInfo in type.GetProperties())
            {
                var propertyType = propertyInfo.PropertyType;
                var valueForA = propertyInfo.GetValue(a, null);
                var valueForB = propertyInfo.GetValue(b, null);

                var isSimpleType = propertyType.IsSimpleType();
                if (isSimpleType)
                    Assert.AreEqual(valueForA, valueForB, "Values in property '{0}' doesn't match.", propertyInfo.Name);
                else
                    AreValueEqual(propertyType, valueForA, valueForB);
            }
        }
    }
}
