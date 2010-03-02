using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace i4o2
{
    public class EqualityIndex<TChild> : IIndex<TChild>
    {
        private readonly Dictionary<int,List<TChild>> _index = new Dictionary<int, List<TChild>>();
        private readonly PropertyInfo _property;
        
        public EqualityIndex(
            IEnumerable<TChild> collectionToIndex, 
            PropertyInfo property)
        {
            _property = property;
            collectionToIndex.ToList().ForEach(Add);
        }

        public IEnumerator<TChild> GetEnumerator()
        {
            return _index.Values.SelectMany(list => list).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(TChild item)
        {            
            var propValue = _property.GetValue(item,null).GetHashCode();

            if (_index.ContainsKey(propValue))
                _index[propValue].Add(item);
            else
                _index.Add(propValue, new List<TChild> {item});
        }

        public void Clear()
        {
            _index.Clear();
        }

        public bool Contains(TChild item)
        {
            var propValue = _property.GetValue(item, null).GetHashCode();
            return _index.ContainsKey(propValue) && _index[propValue].Contains(item);
        }

        public void CopyTo(TChild[] array, int arrayIndex)
        {
            var listOfAll = this.ToList();
            listOfAll.CopyTo(array, arrayIndex);
        }

        public bool Remove(TChild item)
        {
            var propValue = _property.GetValue(item, null).GetHashCode();
            return _index.ContainsKey(propValue) && _index[propValue].Remove(item);
        }

        public int Count
        {
            get { return this.Count(); }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public string PropertyName
        {
            get { return _property.Name; }
        }

        public IEnumerable<TChild> WhereThroughIndex(Expression<Func<TChild, bool>> predicate)
        {
            if (!(predicate.Body is BinaryExpression)) 
                throw new NotSupportedException();
            var equalityExpression = predicate.Body as BinaryExpression;
            if (equalityExpression.NodeType != ExpressionType.Equal)
                throw new NotImplementedException("Equality Indexes do not work with non equality binary expressions");
            var rightSide = Expression.Lambda(equalityExpression.Right);
            var valueToCheck = rightSide.Compile().DynamicInvoke(null).GetHashCode();
            if (_index.ContainsKey(valueToCheck))
                foreach (var item in _index[valueToCheck])
                {
                    var matchingFromBucket = _index[valueToCheck].Where(predicate.Compile());
                    foreach (var bucketItem in matchingFromBucket) yield return bucketItem;
                }
            else
                yield break;
        }
    }
}