using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace i4o2
{
    public class ObservingIndex<T> : Index<T> where T : INotifyPropertyChanged
    {
        private readonly ObservableCollection<T> _observableCollection;
        private readonly IndexSpecification<T> _indexSpecification;
        private void SetupEventHandlers()
        {
            _observableCollection.CollectionChanged += CollectionChanged;
            _observableCollection.ToList()
                .ForEach( child => child.PropertyChanged += PropertyChanged);
        }

        void PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_indexSpecification.IndexedProperties.Contains(e.PropertyName))
            {
                var x = "foo";
                //TODO: Handle item change
            }
        }

        void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //TODO: handle add/update/remove from the index
        }

        public ObservingIndex(ObservableCollection<T> observableCollection, IndexSpecification<T> indexSpecification)
            : base(observableCollection)
        {
            _observableCollection = observableCollection;
            _indexSpecification = indexSpecification;
            SetupEventHandlers();
        }
    }
}
