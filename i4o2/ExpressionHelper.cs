using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace i4o2
{
    using System;
    using System.Linq.Expressions;

    namespace i4o
    {
        internal static class ExpressionHelper
        {
            public static string GetMemberName<T, TProperty>(this Expression<Func<T, TProperty>> propertyExpression)
            {
                return ((MemberExpression)(propertyExpression.Body)).Member.Name;
            }
        }

    }
}
