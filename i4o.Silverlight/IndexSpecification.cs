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

        public static IndexSpecification<T> Build()
        {
            return new IndexSpecification<T>();
        }

        public IndexSpecification<T> Add<TProperty>(Expression<Func<T, TProperty>> propertyExpression)
        {
            var value = propertyExpression.GetMemberName();

            // Should only add property once
            if (!IndexedProperties.Contains(value))
                IndexedProperties.Add(value);

            return this;
        }

        public IndexSpecification<T> With<TProperty>(Expression<Func<T, TProperty>> propertyExpression)
        {
            return Add(propertyExpression);
        }

        public IndexSpecification<T> Remove<TProperty>(Expression<Func<T, TProperty>> propertyExpressions)
        {
            var value = propertyExpressions.GetMemberName();

            IndexedProperties.Remove(value);

            return this;
        }

        public IndexSpecification<T> And<TProperty>(Expression<Func<T, TProperty>> propertyExpression)
        {
            return Add(propertyExpression);
        }
    }
}
