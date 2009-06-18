using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Utility.Core
{
    public static class MethodInfoExtensions
    {
        public static TDelegateType CreateDelegate<TDelegateType>(this MethodInfo method)
        {
            object o = Delegate.CreateDelegate(typeof (TDelegateType), method);
            return (TDelegateType) o;
        }

        public static LambdaExpression BuildLambda(this MethodInfo method, params object[] @params)
        {
            var cExprs = @params.Select(o => Expression.Constant(o));
            var parameter = Expression.Parameter(method.ReflectedType, "a");
            var call = cExprs.Count() > 0
                           ? Expression.Call(parameter, method, cExprs.ToArray())
                           : Expression.Call(parameter, method);
            var l = Expression.Lambda(call, parameter);

            return l;
        }
    }
}
