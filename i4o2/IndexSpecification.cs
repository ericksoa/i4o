using System;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using i4o2.i4o;

namespace i4o2
{
    public class IndexSpecification<T>
    {
        public Collection<string> IndexedProperties { get; private set; }

        public IndexSpecification()
        {
            IndexedProperties = new Collection<string>();
        }

        public IndexSpecification<T> Add<TProperty>(Expression<Func<T, TProperty>> propertyExpressions)
        {
            var value = propertyExpressions.GetMemberName();

            // Should only add property once
            if (!IndexedProperties.Contains(value))
                IndexedProperties.Add(value);

            return this;
        }

        public IndexSpecification<T> Remove<TProperty>(Expression<Func<T, TProperty>> propertyExpressions)
        {
            var value = propertyExpressions.GetMemberName();

            IndexedProperties.Remove(value);

            return this;
        }
    }

}
