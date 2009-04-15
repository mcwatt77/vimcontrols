using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Interceptor;

namespace NodeMessaging
{
    public class SuperCast : IInterceptor
    {
        private readonly object _obj;

        public SuperCast(object obj)
        {
            _obj = obj;
        }

        public void Intercept(IInvocation invocation)
        {
            var method = _obj.GetType().GetMethod(invocation.Method.Name);
            var parameters = method.GetParameters();
            var newArgs = invocation.Arguments.Select((arg, i) => TranslateArgument(arg, parameters[i].ParameterType)).ToArray();
            var ret = method.Invoke(_obj, newArgs);
            invocation.ReturnValue = TranslateArgument(ret, invocation.Method.ReturnType);
        }

        public object TranslateArgument(object obj, Type typeTarget)
        {
            if (obj == null)
            {
                int debug = 0;
            }
            if (typeTarget.IsGenericType)
            {
                if (typeTarget.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    var enumCastType = typeTarget.GetGenericArguments().Single();
                    var castMethod = typeof (Enumerable).GetMethod("Cast");
                    castMethod = castMethod.MakeGenericMethod(enumCastType);
                    return castMethod.Invoke(null, new[] {obj});
                }
            }
            return obj;
        }
    }
}