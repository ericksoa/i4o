using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;

namespace i4o.Tests
{
    [TestFixture]
    public class PropertyReaderTests
    {

        [Test]
        public void Should_throw_ArgumentNullException_if_given_null_propertyName()
        {
            typeof(ArgumentNullException)
                .ShouldBeThrownBy(() => { new PropertyReader<Type>(null); });
        }

        [Test]
        public void Should_throw_ArgumentException_if_given_empty_propertyName()
        {
            typeof(ArgumentException)
                .ShouldBeThrownBy(() => { new PropertyReader<Type>(string.Empty); });
        }

        [Test]
        public void Should_throw_exception_if_given_propertyName_not_existing_on_object()
        {
            typeof(ArgumentException)
                .ShouldBeThrownBy(() => { new PropertyReader<Type>("someRandomPropertyNameHEREEEEEE"); });
        }

        [Test]
        public void Should_be_able_to_read_the_value_of_a_type_using_the_DelegateMethod_strategy()
        {
            var t = new TestClass { TestValue = "Hi" };
            var p = new PropertyReader<TestClass>(PropertyReadStrategy.DelegateMethod, "TestValue");
            p.ReadValue(t).ShouldEqual("Hi");
        }

        [Test]
        public void Should_be_able_to_read_the_value_of_a_type_using_the_Reflection_strategy()
        {
            var t = new TestClass { TestValue = "Hi" };
            var p = new PropertyReader<TestClass>(PropertyReadStrategy.Reflection, "TestValue");
            p.ReadValue(t).ShouldEqual("Hi");
        }

        [Test]
        public void Should_return_zero_when_trying_to_get_a_null_objects_HashCode()
        {
            var @class = new TestClass();
            var reader = new PropertyReader<TestClass>("TestValue");
            reader.GetItemHashCode(@class).ShouldEqual(0);
        }

        [Test]
        public void Should_return_actual_objects_HashCode()
        {
            const string testString = "Hello World!";
            var @class = new TestClass { TestValue = testString };
            var reader = new PropertyReader<TestClass>("TestValue");
            reader.GetItemHashCode(@class).ShouldEqual(testString.GetHashCode());
        }

        public class TestClass
        {
            public string TestValue { get; set; }
        }

#if !SILVERLIGHT
        [Test]
        [TestCase(10, PropertyReadStrategy.Reflection)]
        [TestCase(250, PropertyReadStrategy.Reflection)]
        [TestCase(450, PropertyReadStrategy.DelegateMethod)]
        [TestCase(1000, PropertyReadStrategy.DelegateMethod)]
        [Explicit]
        public void Property_read_strategy_perf_test(int max, PropertyReadStrategy expectedToBeFaster)
        {
            var random = new Random();
            var stats = new List<IndexReadTimingResult>();

            for (int i = 0; i < 1000; i++)
            {
                var numberOfItems = random.Next() % max;
                if (numberOfItems == 0)
                    continue;
                var stat = GetIndexReadTimingResult(numberOfItems);
                stats.Add(stat);
            }

            IndexReadTimingResult.
                WhoWasGenerallyFaster(stats).ShouldEqual(expectedToBeFaster);
        }
#endif

        public class IndexReadTimingResult
        {
            public long DelegateMethodTime { get; set; }
            public long ReflectionTime { get; set; }
            public int Iterations { get; set; }

            public PropertyReadStrategy WhoWon()
            {
                if (DelegateMethodTime > ReflectionTime)
                    return PropertyReadStrategy.Reflection;
                return PropertyReadStrategy.DelegateMethod;
            }

            public static PropertyReadStrategy WhoWasGenerallyFaster(IEnumerable<IndexReadTimingResult> testSamples)
            {

                var statSummary = new Dictionary<PropertyReadStrategy, int>
                                      {
                                          {PropertyReadStrategy.Reflection, 0},
                                          {PropertyReadStrategy.DelegateMethod, 0}
                                      };

                foreach (var stat in testSamples)
                {
                    statSummary[stat.WhoWon()]++;
                }

                var totalItmes = statSummary.Select(s => s.Value).Sum();

                // If "generally" faster is too close co call. Fail...
                var percentageDiff = Math.Abs(.5d - (statSummary[PropertyReadStrategy.Reflection] / (double)totalItmes));
                if (percentageDiff < .05)
                    throw new Exception(
                        string.Format(
                            "Item stats were too close to call Reflection={0}, DelegateMethod={1}, percentage={2}",
                            statSummary[PropertyReadStrategy.Reflection],
                            statSummary[PropertyReadStrategy.DelegateMethod],
                            percentageDiff));

                PropertyReadStrategy winner;
                if (statSummary[PropertyReadStrategy.Reflection] > statSummary[PropertyReadStrategy.DelegateMethod])
                    winner = PropertyReadStrategy.Reflection;
                else
                    winner = PropertyReadStrategy.DelegateMethod;

                return winner;
            }
        }

        private static IndexReadTimingResult GetIndexReadTimingResult(int numberOfItems)
        {
            var dummyCollectionOfData = (from x in Enumerable.Range(1, numberOfItems)
                                         select new TestClass { TestValue = Guid.NewGuid().ToString() })
                .ToList();

            Func<PropertyReadStrategy, int, long> timeFor = (readerStrategy, iterations) =>
                                                                {
                                                                    var stopwatch = Stopwatch.StartNew();
                                                                    var reader = new PropertyReader<TestClass>(readerStrategy, "TestValue");
                                                                    for (int i = 0; i < iterations; i++)
                                                                    {
                                                                        reader.ReadValue(dummyCollectionOfData[i]);
                                                                    }
                                                                    stopwatch.Stop();
                                                                    return stopwatch.ElapsedTicks;
                                                                };

            // Run each once to initialize any system level (possibly jitting???) or eronous code...
            timeFor(PropertyReadStrategy.Reflection, 1);
            timeFor(PropertyReadStrategy.DelegateMethod, 1);

            var reflectionTime = timeFor(PropertyReadStrategy.Reflection, numberOfItems);
            var delegateMethodTime = timeFor(PropertyReadStrategy.DelegateMethod, numberOfItems);

            return new IndexReadTimingResult
                    {
                        DelegateMethodTime = delegateMethodTime,
                        ReflectionTime = reflectionTime,
                        Iterations = numberOfItems,
                    };
        }
    }
}