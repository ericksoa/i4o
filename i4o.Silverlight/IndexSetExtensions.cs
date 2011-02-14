using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace i4o
{
    public static class IndexSetExtensions
    {
        public static IEnumerable<T> WhereUsingIndex<T>(IndexSet<T> indexSet, Expression<Func<T, bool>> predicate)
        {
            if (!(predicate.Body is BinaryExpression)) throw new NotSupportedException();
            var binaryBody = (BinaryExpression) predicate.Body;
            switch (binaryBody.NodeType)
            {
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    {
                        var leftResults = WhereUsingIndex(indexSet, Expression.Lambda<Func<T,bool>>(binaryBody.Left, predicate.Parameters));
                        var rightResults = WhereUsingIndex(indexSet, Expression.Lambda<Func<T, bool>>(binaryBody.Right, predicate.Parameters));
                        return leftResults.Union(rightResults);
                    }
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    {
                        var leftResults = WhereUsingIndex(indexSet, Expression.Lambda<Func<T, bool>>(binaryBody.Left, predicate.Parameters ));
                        var rightResults = WhereUsingIndex(indexSet, Expression.Lambda<Func<T, bool>>(binaryBody.Right, predicate.Parameters));
                        return leftResults.Intersect(rightResults);                
                    }
                case ExpressionType.Equal:
                    return indexSet.WhereUsingIndex(predicate);
                default:
                    return Enumerable.Where(indexSet, predicate.Compile());
            }
        }
        
        public static IEnumerable<T> Where<T>(this IndexSet<T> indexSet, Expression<Func<T,bool>> predicate)
        {
            return WhereUsingIndex(indexSet, predicate);
        }
    }
}