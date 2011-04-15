using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using SisoDb.Core;
using SisoDb.Reflections;

namespace SisoDb.TestUtils
{
    public static class CustomAssert
    {
        [DebuggerStepThrough()]
        public static Exception Throws<T>(Action action) where T : Exception
        {
            try
            {
                action.Invoke();
            }
            catch (T ex)
            {
                return ex;
            }
            catch (Exception ex)
            {
                throw new AssertionException("Expected exception was not thrown!", ex);
            }

            throw new AssertionException("Expected exception was not thrown!");
        }

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
