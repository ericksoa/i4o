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

        protected void SetupIndices(IEnumerable<T> source)
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
            if (
                BodyIsBinary(predicate) && 
                BodyTypeIsEqual(predicate) &&
                LeftSideIsMemberExpression(predicate) &&
                LeftSideMemberIsIndexed(predicate)
               )
               return IndexDictionary[LeftSide(predicate).Member.Name].WhereThroughIndex(predicate);
            return IndexDictionary.First().Value.Where(predicate.Compile());
        }

        private static MemberExpression LeftSide(Expression<Func<T, bool>> predicate)
        {
            return ((MemberExpression)((BinaryExpression)predicate.Body).Left);
        }

        private bool LeftSideMemberIsIndexed(Expression<Func<T, bool>> predicate)
        {
            return (IndexSpecification.IndexedProperties.Contains(
                ((MemberExpression)((BinaryExpression)predicate.Body).Left
                ).Member.Name));
        }

        private static bool LeftSideIsMemberExpression(Expression<Func<T, bool>> predicate)
        {
            return ((((BinaryExpression)predicate.Body)).Left is MemberExpression);
        }

        private static bool BodyTypeIsEqual(Expression<Func<T, bool>> predicate)
        {
            return (predicate.Body.NodeType == ExpressionType.Equal);
        }

        private static bool BodyIsBinary(Expression<Func<T, bool>> predicate)
        {
            return (predicate.Body is BinaryExpression);
        }
    }
}