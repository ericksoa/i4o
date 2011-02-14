using System;
using System.Linq.Expressions;
using System.Reflection;

namespace i4o
{
    namespace i4o
    {
        /*
         * The DelegateFactory was pulled from this great post by Nate Kohari
         * http://kohari.org/2009/03/06/fast-late-bound-invocation-with-expression-trees/
         */

        internal delegate object LateBoundProperty(object target);

        internal static class DelegateFactory
        {
            public static LateBoundProperty Create<T>(PropertyInfo property)
            {
                if (property == null)
                    throw new ArgumentNullException("property");

                var method = typeof(T).GetMethod("get_" + property.Name, Type.EmptyTypes);

                return Create(method);
            }

            private static LateBoundProperty Create(MethodInfo method)
            {
                ParameterExpression instanceParameter = Expression.Parameter(typeof(object), "target");

                MethodCallExpression call = Expression.Call(
                    Expression.Convert(instanceParameter, method.DeclaringType),
                    method);

                Expression<LateBoundProperty> lambda = Expression.Lambda<LateBoundProperty>(
                    Expression.Convert(call, typeof(object)),
                    instanceParameter);

                return lambda.Compile();
            }
        }
    }
}
