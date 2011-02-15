using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;

namespace i4o.Tests
{
    public class TestClass
    {
        public string TestProperty1 { get; set; }
        public string TestProperty2 { get; set; }

        public static IndexSpecification<TestClass> GetIndexSpec()
        {
            return new IndexSpecification<TestClass>()
                .Add(p => p.TestProperty1)
                .Add(p => p.TestProperty2);
        }
    }

    internal class TestIndexableCollection<T> : IndexSet<T>
    {
        public TestIndexableCollection(IEnumerable<T> items, IndexSpecification<T> indexSpecification)
            : base(items, indexSpecification)
        { }

        //public Dictionary<string, Dictionary<int, List<T>>> CurrentIndexes { get { return Indexes; } }

        public int CountOfValuesIndexedFor<TProperty>(Expression<Func<T, TProperty>> propertyExpression)
        {
            var propertyName = propertyExpression.GetMemberName();
            return base.IndexDictionary[propertyName].Count();
        }

        public IIndex<T> GetIndexFor<TParameter>(Expression<Func<T, TParameter>> propertyExpression)
        {
            var propertyName = propertyExpression.GetMemberName();
            return base.IndexDictionary[propertyName];
        }
    }

    [TestFixture]
    [Ignore("Old version used to support ICollection & IList - not anymore?")]
    public class CollectionModificationTests
    {
        [Test]
        public void when_adding_an_item_using_the_base_type_does_the_index_get_updated()
        {
            var testClasses = new List<TestClass>
			                  	{
			                  		new TestClass {TestProperty1 = "Hello1"},
			                  		new TestClass {TestProperty1 = "Hello2"},
			                  		new TestClass {TestProperty1 = "Hello3"},
			                  	};

            var indexedTestClasses = new TestIndexableCollection<TestClass>(testClasses, TestClass.GetIndexSpec());

            var baseCollection = indexedTestClasses as ICollection<TestClass>;
            baseCollection.Add(new TestClass { TestProperty1 = "Hello4" });

            Assert.AreEqual(4, indexedTestClasses.CountOfValuesIndexedFor(p => p.TestProperty1));
        }

        [Test]
        public void when_inserting_an_item_at_a_particular_index_the_new_item_should_get_indexed()
        {
            var testClasses = new List<TestClass>
			                  	{
			                  		new TestClass {TestProperty1 = "Hello1"},
			                  		new TestClass {TestProperty1 = "Hello2"},
			                  		new TestClass {TestProperty1 = "Hello3"},
			                  	};

            var indexedTestClasses = new TestIndexableCollection<TestClass>(testClasses, TestClass.GetIndexSpec());

            var baseCollection = indexedTestClasses as IList<TestClass>;
            baseCollection.Insert(0, new TestClass { TestProperty1 = "Hello4" });

            Assert.AreEqual(4, indexedTestClasses.CountOfValuesIndexedFor(p => p.TestProperty1));
        }

        [Test]
        public void when_removing_an_item_at_a_particular_index_the_item_should_be_removed_from_the_index()
        {
            var testClasses = new List<TestClass>
			                  	{
			                  		new TestClass {TestProperty1 = "Hello1"},
			                  		new TestClass {TestProperty1 = "Hello2"},
			                  		new TestClass {TestProperty1 = "Hello3"},
			                  	};

            var indexedTestClasses = new TestIndexableCollection<TestClass>(testClasses, TestClass.GetIndexSpec());

            throw new NotImplementedException();
            //indexedTestClasses.RemoveAt(0);

            Assert.AreEqual(2, indexedTestClasses.CountOfValuesIndexedFor(p => p.TestProperty1));
        }

        [Test]
        public void when_changing_the_value_at_a_particular_index_the_old_item_should_be_removed_from_the_index_and_the_new_value_should_be_added_to_the_index()
        {
            var t = new TestClass { TestProperty1 = "Hello1" };
            var testClasses = new List<TestClass>
                              	{
                              		t,
                              		new TestClass {TestProperty1 = "Hello2"},
                              		new TestClass {TestProperty1 = "Hello3"},
                              	};

            var indexedTestClasses = new TestIndexableCollection<TestClass>(testClasses, TestClass.GetIndexSpec());

            var testProperty2Index = indexedTestClasses.GetIndexFor(s => s.TestProperty2);

            throw new NotImplementedException();
            //indexedTestClasses[0] = new TestClass { TestProperty1 = "Hello4", TestProperty2 = "a" };

            //testProperty2Index.ItemsWithKey(Consts.NullKeyHashCode)
            //    .Count().ShouldEqual(2);
        }


        [Test]
        public void when_clearing_the_collection_it_should_also_clear_the_internal_indicies()
        {
            var testClasses = new List<TestClass>
			                  	{
			                  		new TestClass {TestProperty1 = "Hello1"},
			                  		new TestClass {TestProperty1 = "Hello2"},
			                  		new TestClass {TestProperty1 = "Hello3"},
			                  	};

            var indexedTestClasses = new TestIndexableCollection<TestClass>(testClasses, TestClass.GetIndexSpec());

            throw new NotImplementedException();
            //indexedTestClasses.Clear();

            //Assert.AreEqual(0, indexedTestClasses.Count);
            //Assert.AreEqual(0, indexedTestClasses.CountOfValuesIndexedFor(p => p.TestProperty1));
            //Assert.AreEqual(0, indexedTestClasses.CountOfValuesIndexedFor(p => p.TestProperty2));
        }

        [Test]
        public void the_IList_Contains_method_should_work_correctly()
        {
            var testClasses = new List<TestClass>
			                  	{
			                  		new TestClass {TestProperty1 = "Hello1"},
			                  		new TestClass {TestProperty1 = "Hello2"},
			                  		new TestClass {TestProperty1 = "Hello3"},
			                  	};

            var item = testClasses.Last();

            var indexedTestClasses = new TestIndexableCollection<TestClass>(testClasses, TestClass.GetIndexSpec());
            Assert.IsTrue(indexedTestClasses.Contains(item));
        }

        [Test]
        public void the_IList_IndexOf_method_should_work_correctly()
        {
            var testClasses = new List<TestClass>
			                  	{
			                  		new TestClass {TestProperty1 = "Hello1"},
			                  		new TestClass {TestProperty1 = "Hello2"},
			                  		new TestClass {TestProperty1 = "Hello3"},
			                  	};

            var item = testClasses.Last();
            throw new NotImplementedException();
            //var indexedTestClasses = new TestIndexableCollection<TestClass>(testClasses, TestClass.GetIndexSpec());
            //Assert.AreEqual(2, indexedTestClasses.IndexOf(item));
        }

        [Test]
        public void should_be_able_to_get_an_enumerator_of_the_indexable_collection()
        {
            var indexedTestClasses = new IndexSet<TestClass>(TestClass.GetIndexSpec());

            Assert.IsNotNull((indexedTestClasses as IEnumerable<TestClass>).GetEnumerator());
            Assert.IsNotNull((indexedTestClasses as IEnumerable).GetEnumerator());
        }

    }
}