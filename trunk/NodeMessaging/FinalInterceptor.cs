using Castle.Core.Interceptor;

namespace NodeMessaging
{
    public class FinalInterceptor : IInterceptor
    {
        private readonly RootNode _rootNode;
        private readonly IEndNode _node;

        public FinalInterceptor(RootNode rootNode, IEndNode node)
        {
            _rootNode = rootNode;
            _node = node;
        }

        public void Intercept(IInvocation invocation)
        {
            invocation.Proceed();
            _rootNode.Intercept(_node, invocation);
        }
    }
}