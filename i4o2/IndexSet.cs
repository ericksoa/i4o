using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace i4o2
{
    public class IndexSet<T> : IEnumerable<T>
    {
        protected readonly IndexSpecification<T> IndexSpecification;
        protected readonly Dictionary<string, IIndex<T>> IndexDictionary 
            = new Dictionary<string, IIndex<T>>();

        public IndexSet(IEnumerable<T> source, IndexSpecification<T> indexSpecification)
        {
            IndexSpecification = indexSpecification;
            SetupIndices(source);
        }

        private void SetupIndices(IEnumerable<T> source)
        {
            IndexSpecification.IndexedProperties.ToList().ForEach( 
                propName =>
                  IndexDictionary.Add(propName, IndexBuilder.GetIndexFor(source, typeof (T).GetProperty(propName)))
            );
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (IndexSpecification.IndexedProperties.Count > 0)
                return IndexDictionary[IndexSpecification.IndexedProperties[0]].GetEnumerator();
            throw new InvalidOperationException("Can't enumerate without at least one index");
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        internal IEnumerable<T> WhereUsingIndex(Expression<Func<T,bool>> predicate)
        {
            if (!(predicate.Body is BinaryExpression)) throw new NotSupportedException();
            var binaryBody = (BinaryExpression) predicate.Body;
            if (binaryBody.NodeType == ExpressionType.Equal)
            {
                if (!(binaryBody.Left is MemberExpression)) throw new NotSupportedException();
                var memberLeft = (MemberExpression) binaryBody.Left;
                if (IndexSpecification.IndexedProperties.Contains(memberLeft.Member.Name))
                    return IndexDictionary[memberLeft.Member.Name].WhereThroughIndex(predicate);
                return IndexDictionary.First().Value.Where(predicate.Compile());
            }
            throw new NotSupportedException();
        }
    }
}