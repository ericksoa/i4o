using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace i4o2
{
    public static class IndexBuilder
    {
        public static IIndex<TChild> GetIndexFor<TChild>(
            IEnumerable<TChild> enumerable,
            PropertyInfo propertyInfo)
        {
            return propertyInfo.PropertyType.Supports<IComparable>()
                       ? MakeComparisonIndex(enumerable, propertyInfo)
                       : MakeEqualityIndex(propertyInfo, enumerable);
        }

        private static IIndex<TChild> MakeEqualityIndex<TChild>(PropertyInfo propertyInfo, IEnumerable<TChild> enumerable)
        {
            return new EqualityIndex<TChild>(enumerable, propertyInfo);
        }

        private static IIndex<TChild> MakeComparisonIndex<TChild>(IEnumerable<TChild> enumerable, PropertyInfo propertyInfo)
        {
            return (IIndex<TChild>) 
                   Activator.CreateInstance(
                       Type.GetType("i4o2.ComparisonIndex`2").MakeGenericType(new Type[] {typeof (TChild), propertyInfo.PropertyType}),
                       new object[] { enumerable, propertyInfo }
                       );
        }

        public static bool Supports<T>(this Type type)
        {
            return type.GetInterfaces().Where(i => i == typeof (T)).Count() > 0;
        }
    }
}