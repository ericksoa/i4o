using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Drawing;
using NUnit.Framework;

namespace i4o2.Tests
{
    public class ObservableObject : INotifyPropertyChanged
    {

        private int _someMutable;
        public int SomeMutable
        {
            get { return _someMutable; }
            set
            {
                if (_someMutable == value) return;
                _someMutable = value;
                OnPropertyChanged("SomeMutable");
            }
        }

        protected void OnPropertyChanged(string name)
        {
            var handler = PropertyChanged;

            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
    
    [TestFixture]
    public class IndexTests
    {
        [Test]
        public void IndexRecognizesChangeInAChildProperty()
        {
            var someObservableObject = new ObservableObject {SomeMutable = 6};
            var someCollection = new ObservableCollection<ObservableObject>(
                new List<ObservableObject>{ someObservableObject });
            var indexSpec = IndexSpecification<ObservableObject>.Build()
                .With(child => child.SomeMutable);
            var someIndex = IndexBuilder.BuildIndicesFor(someCollection, indexSpec);
            someObservableObject.SomeMutable = 3;
            Assert.AreEqual( (from v in someIndex where v.SomeMutable == 3 select v).Count(), 1 );
        }

        [Test]
        public void IndexRecognizesItemAddedToSourceCollection()
        {
            var someObservableObject = new ObservableObject { SomeMutable = 6 };
            var someCollection = new ObservableCollection<ObservableObject>(
                new List<ObservableObject> { someObservableObject });
            var indexSpec = IndexSpecification<ObservableObject>.Build()
                .With(child => child.SomeMutable);
            var someIndex = IndexBuilder.BuildIndicesFor(someCollection, indexSpec);
            someCollection.Add( new ObservableObject { SomeMutable = 3 });
            Assert.AreEqual((from v in someIndex where v.SomeMutable == 3 select v).Count(), 1);
        }

        public class SimpleClass
        {
            public string Name{ get; set; } 
            public int Age { get; set; }
            public Color FavoriteColor { get; set; }
         }

        public int ResolvesToZero() {
            return 2 - 2; }

        [Test]
        public void EquatableIndexLookupResolves()
        {
            SimpleClass[] someItems = {
                                new SimpleClass {Name = "Jason", Age = 25},
                                new SimpleClass {Name = "Aaron", Age = 37}
                            };
            var indexOnSomeItems = 
                new EqualityIndex<SimpleClass>(
                    someItems,
                    typeof (SimpleClass).GetProperty("Age"));
            var jason = indexOnSomeItems.WhereThroughIndex(item => item.Age == 25).First();
            Assert.AreEqual("Jason",jason.Name);
        }

        [Test]
        public void EquatableIndexLookupWithComplexRightConditionResolves()
        {
            SimpleClass[] someItems = {
                                new SimpleClass {Name = "Jason", Age = 25},
                                new SimpleClass {Name = "Aaron", Age = 37}
                            };
            var indexOnSomeItems =
                new EqualityIndex<SimpleClass>(
                    someItems,
                    typeof(SimpleClass).GetProperty("Age"));
            var jason = indexOnSomeItems.WhereThroughIndex(item => item.Age == (someItems[0].Age + ResolvesToZero())).First();
            Assert.AreEqual("Jason", jason.Name);
        }

        [Test]
        public void ComparableIndexLookupWithLessThan()
        {
            SimpleClass[] someItems = {
                                new SimpleClass {Name = "Jason", Age = 25},
                                new SimpleClass {Name = "Aaron", Age = 37},
                                new SimpleClass {Name = "Erin", Age=34},
                                new SimpleClass {Name = "Adriana", Age=13}, 
                            };
            var indexOnSomeItems =
                new ComparisonIndex<SimpleClass, int>(
                    someItems,
                    typeof(SimpleClass).GetProperty("Age"));
            var youngerThan34 = indexOnSomeItems.WhereThroughIndex(item => item.Age < 34);
            Assert.AreEqual(2, youngerThan34.Count());
        }

        [Test]
        public void ComparableIndexLookupWithLessThanOrEqualTo()
        {
            SimpleClass[] someItems = {
                                new SimpleClass {Name = "Jason", Age = 25},
                                new SimpleClass {Name = "Aaron", Age = 37},
                                new SimpleClass {Name = "Erin", Age=34},
                                new SimpleClass {Name = "Adriana", Age=13}, 
                            };
            var indexOnSomeItems =
                new ComparisonIndex<SimpleClass, int>(
                    someItems,
                    typeof(SimpleClass).GetProperty("Age"));
            var youngerThan34Or34 = indexOnSomeItems.WhereThroughIndex(item => item.Age <= 34);
            Assert.AreEqual(3, youngerThan34Or34.Count());
        }

        [Test]
        public void ComparableIndexLookupWithGreaterThan()
        {
            SimpleClass[] someItems = {
                                new SimpleClass {Name = "Jason", Age = 25},
                                new SimpleClass {Name = "Aaron", Age = 37},
                                new SimpleClass {Name = "Erin", Age=34},
                                new SimpleClass {Name = "Adriana", Age=13}, 
                            };
            var indexOnSomeItems =
                new ComparisonIndex<SimpleClass, int>(
                    someItems,
                    typeof(SimpleClass).GetProperty("Age"));
            var olderThan34 = indexOnSomeItems.WhereThroughIndex(item => item.Age > 34);
            Assert.AreEqual(1, olderThan34.Count());
        }

        [Test]
        public void ComparableIndexLookupWithGreaterThanOrEqualTo()
        {
            SimpleClass[] someItems = {
                                new SimpleClass {Name = "Jason", Age = 25},
                                new SimpleClass {Name = "Aaron", Age = 37},
                                new SimpleClass {Name = "Erin", Age=34},
                                new SimpleClass {Name = "Adriana", Age=13}, 
                            };
            var indexOnSomeItems =
                new ComparisonIndex<SimpleClass, int>(
                    someItems,
                    typeof(SimpleClass).GetProperty("Age"));
            var olderThan34Or34 = indexOnSomeItems.WhereThroughIndex(item => item.Age >= 34);
            Assert.AreEqual(2, olderThan34Or34.Count());
        }

        [Test]
        public void BuilderReturnsComparisonIndexForComparable()
        {
            SimpleClass[] someItems = {
                                new SimpleClass {Name = "Jason", Age = 25},
                                new SimpleClass {Name = "Aaron", Age = 37},
                                new SimpleClass {Name = "Erin", Age=34},
                                new SimpleClass {Name = "Adriana", Age=13}, 
                            };
            var theRightIndex 
                = IndexBuilder.GetIndexFor(
                    someItems, 
                    typeof (SimpleClass).GetProperty("Age")
                );
            Assert.AreEqual(typeof(ComparisonIndex<SimpleClass,int>),theRightIndex.GetType());
        }

        [Test]
        public void BuilderReturnsEqualityIndexForNotComparable()
        {
            SimpleClass[] someItems = {
                                new SimpleClass {Name = "Jason", Age = 25},
                                new SimpleClass {Name = "Aaron", Age = 37, FavoriteColor=Color.Green},
                                new SimpleClass {Name = "Erin", Age=34},
                                new SimpleClass {Name = "Adriana", Age=13}, 
                            };
            var theRightIndex
                = IndexBuilder.GetIndexFor(
                    someItems,
                    typeof(SimpleClass).GetProperty("FavoriteColor")
                );
            Assert.AreEqual(typeof(EqualityIndex<SimpleClass>), theRightIndex.GetType());
        }

        [Test]
        public void ComplexQuery()
        {
            SimpleClass[] someItems = {
                                          new SimpleClass {Name = "Jason", Age = 25},
                                          new SimpleClass {Name = "Aaron", Age = 37, FavoriteColor = Color.Green},
                                          new SimpleClass {Name = "Erin", Age = 34},
                                          new SimpleClass {Name = "Adriana", Age = 13},
                                      };
            var indexSpec = IndexSpecification<SimpleClass>.Build()
                .With(person => person.FavoriteColor)
                .And(person => person.Age);
            var theIndexSet = new IndexSet<SimpleClass>(someItems, indexSpec);
            var twoResults = 
                from item in theIndexSet 
                where item.FavoriteColor == Color.Green || item.Age == 13 && item.Name == "Adriana"
                select item;
            Assert.AreEqual(2, twoResults.Count());
        }

        [Test]
        public void SuperComplexQuery()
        {
            SimpleClass[] someItems = {
                                          new SimpleClass {Name = "Jason Jarett", Age = 25, FavoriteColor = Color.Aqua},
                                          new SimpleClass {Name = "Aaron Erickson", Age = 37, FavoriteColor = Color.Green},
                                          new SimpleClass {Name = "Erin Erickson", Age = 34, FavoriteColor = Color.Green},
                                          new SimpleClass {Name = "Adriana Erickson", Age = 13, FavoriteColor = Color.Aqua},
                                      };
            var indexSpec = IndexSpecification<SimpleClass>.Build()
                .With(person => person.FavoriteColor)
                .And(person => person.Age)
                .And(person => person.Name);
            var theIndexSet = new IndexSet<SimpleClass>(someItems, indexSpec);
            var oneResult =
                from item in theIndexSet
                where 
                (
                  (item.FavoriteColor == Color.Green && item.Age == 37) || 
                  (item.Name == "Adriana Erickson" && item.Age == 13) ||
                  (item.Name == "Jason Jarett" && item.Age == 25)
                ) && item.Name == "Aaron Erickson"
                select item;
            Assert.AreEqual(1, oneResult.Count());
        }
    }
}
