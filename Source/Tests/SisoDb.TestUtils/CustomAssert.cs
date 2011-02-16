using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SisoDb.Reflections;

namespace SisoDb.TestUtils
{
    public static class CustomAssert
    {
        public static Exception Throws<T>(Action action) where T : Exception
        {
            try
            {
                action.Invoke();

                throw new AssertionException("Expected exception was not thrown!");
            }
            catch (T ex)
            {
                return ex;
            }
        }

        public static void IsEmpty<T>(IEnumerable<T> source)
        {
            var isEmpty = source.Count() < 1;

            if(!isEmpty)
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

        private static void AreValueEqual(Type type, object a, object b)
        {
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
