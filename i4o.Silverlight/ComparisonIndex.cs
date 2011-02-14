using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using C5;

namespace i4o
{
    public class ComparisonIndex<TChild, TProperty> : IIndex<TChild>
        where TProperty : IComparable
    {
        private readonly TreeDictionary<TProperty, List<TChild>> _index = new TreeDictionary<TProperty, List<TChild>>();
        private readonly PropertyReader<TChild> _propertyReader;

        public ComparisonIndex(IEnumerable<TChild> collectionToIndex, PropertyInfo property)
        {
            _propertyReader = new PropertyReader<TChild>(property.Name);

            collectionToIndex.Each(Add);
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
            var propValue = (TProperty)_propertyReader.ReadValue(item);

            if (_index.Contains(propValue))
                _index[propValue].Add(item);
            else
                _index.Add(propValue, new List<TChild> { item });
        }

        public void Clear()
        {
            _index.Clear();
        }

        public bool Contains(TChild item)
        {
            var propValue = (TProperty)_propertyReader.ReadValue(item);
            return _index.Contains(propValue);
        }

        public void CopyTo(TChild[] array, int arrayIndex)
        {
            var listOfAll = this.ToList();
            listOfAll.CopyTo(array, arrayIndex);
        }

        public bool Remove(TChild item)
        {
            var propValue = (TProperty)_propertyReader.ReadValue(item);
            return _index.Contains(propValue) && _index[propValue].Remove(item);
        }

        public int Count
        {
            get { return this.Count(); }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public IEnumerable<TChild> WhereThroughIndex(Expression<Func<TChild, bool>> predicate)
        {
            if (!(predicate.Body is BinaryExpression))
                throw new NotSupportedException();
            var equalityExpression = predicate.Body as BinaryExpression;
            if (equalityExpression == null) throw new NullReferenceException();
            var rightSide = Expression.Lambda(equalityExpression.Right);
            var valueToCheck = (TProperty)rightSide.Compile().DynamicInvoke(null);
            switch (equalityExpression.NodeType)
            {
                case ExpressionType.Equal:
                    foreach (var item in IsEqualTo(valueToCheck)) yield return item;
                    break;
                case ExpressionType.LessThan:
                    foreach (var item in GetLessThan(valueToCheck)) yield return item;
                    break;
                case ExpressionType.GreaterThan:
                    foreach (var item in GetGreaterThanOrEqualTo(valueToCheck).Except(IsEqualTo(valueToCheck))) yield return item;
                    break;
                case ExpressionType.LessThanOrEqual:
                    foreach (var item in GetLessThan(valueToCheck)) yield return item;
                    foreach (var item in IsEqualTo(valueToCheck)) yield return item;
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    foreach (var item in GetGreaterThanOrEqualTo(valueToCheck)) yield return item;
                    break;
                default:
                    throw new NotImplementedException("Unsupported operation");
            }
        }

        public void Reset(TChild changedObject)
        {
            Remove(changedObject);
            Add(changedObject);
        }

        private IEnumerable<TChild> IsEqualTo(TProperty valueToCheck)
        {
            if (_index.Contains(valueToCheck))
                foreach (var item in _index[valueToCheck])
                    yield return item;
            else
                yield break;
        }

        private IEnumerable<TChild> GetGreaterThanOrEqualTo(TProperty valueToCheck)
        {
            return _index.RangeFrom(valueToCheck).SelectMany(node => node.Value);
        }

        private IEnumerable<TChild> GetLessThan(TProperty valueToCheck)
        {
            return _index.RangeTo(valueToCheck).SelectMany(node => node.Value);
        }
    }
}