using System;
using Castle.Core.Interceptor;

namespace NodeMessaging
{
    public class DelegateInterceptor : IInterceptor
    {
        private readonly Action<IInvocation> _fnIntercept;

        public DelegateInterceptor(Action<IInvocation> fnIntercept)
        {
            _fnIntercept = fnIntercept;
        }

        public void Intercept(IInvocation invocation)
        {
            _fnIntercept(invocation);
        }
    }
}