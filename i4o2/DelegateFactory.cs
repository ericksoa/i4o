using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace i4o2
{
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    namespace i4o
    {
        /*
         * The DelegateFactory was pulled from this great post by Nate Kohari
         * http://kohari.org/2009/03/06/fast-late-bound-invocation-with-expression-trees/
         */

        internal delegate object LateBoundMethod(object target, object[] arguments);

        internal static class DelegateFactory
        {
            public static LateBoundMethod Create(MethodInfo method)
            {
                var instanceParameter = Expression.Parameter(typeof(object), "target");
                var argumentsParameter = Expression.Parameter(typeof(object[]), "arguments");

                var call = Expression.Call(
                    Expression.Convert(instanceParameter, method.DeclaringType),
                    method,
                    CreateParameterExpressions(method, argumentsParameter));

                var lambda = Expression.Lambda<LateBoundMethod>(
                    Expression.Convert(call, typeof(object)),
                    instanceParameter,
                    argumentsParameter);

                return lambda.Compile();
            }

            private static Expression[] CreateParameterExpressions(MethodInfo method, Expression argumentsParameter)
            {
                return method.GetParameters().Select((parameter, index) =>
                    Expression.Convert(
                        Expression.ArrayIndex(argumentsParameter, Expression.Constant(index)),
                        parameter.ParameterType)).ToArray();
            }
        }
    }
}
