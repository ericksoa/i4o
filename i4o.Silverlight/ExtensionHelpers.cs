using System.Collections.Generic;
using System.Globalization;

namespace i4o2
{
    using System;
    using System.Linq.Expressions;

    namespace i4o
    {
        internal static class ExtensionHelpers
        {
            public static string GetMemberName<T, TProperty>(this Expression<Func<T, TProperty>> propertyExpression)
            {
                return ((MemberExpression)(propertyExpression.Body)).Member.Name;
            }

            public static string FormatWith(this string format, params object[] args)
            {
                return string.Format(CultureInfo.CurrentCulture, format, args);
            }

            public static void Each<T>(this IEnumerable<T> items, Action<T> action)
            {
                if (items == null) throw new ArgumentNullException("items");
                if (action == null) throw new ArgumentNullException("action");

                foreach (var item in items)
                {
                    action(item);
                }
            }
        }
    }
}
