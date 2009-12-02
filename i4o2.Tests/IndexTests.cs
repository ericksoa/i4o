using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
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
        public void index_recognizes_change_in_a_child_property()
        {
            var someObservableObject = new ObservableObject {SomeMutable = 6};
            var someCollection = new ObservableCollection<ObservableObject>(
                new List<ObservableObject>{ someObservableObject });
            var indexSpec = new IndexSpecification<ObservableObject>();
            indexSpec.Add(child => child.SomeMutable);
            var someIndex = new ObservingIndex<ObservableObject>(someCollection, indexSpec);
            someObservableObject.SomeMutable = 3;
            Assert.AreEqual( (from v in someIndex where v.SomeMutable == 3 select v).Count(), 1 );
            Assert.Throws<IndexLookupFailedException>(
                () => { (from v in someIndex where v.SomeMutable == 6 select v).First(); });
        }
    }
}
