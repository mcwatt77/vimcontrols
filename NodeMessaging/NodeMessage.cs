using System;
using Castle.Core.Interceptor;

namespace NodeMessaging
{
    public class NodeMessage
    {
        public Predicate<INode> NodePredicate { get; set; }
        public Predicate<IInvocation> MessagePredicate { get; set; }
        public object Target { get; set; }
        public object TargetDelegate { get; set; }
    }
}