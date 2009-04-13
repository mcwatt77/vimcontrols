using System;
using System.Linq;
using Castle.Core.Interceptor;
using NUnit.Framework;

namespace NodeMessaging.Tests
{
    [TestFixture]
    public class NodeTest
    {
        [Test]
        public void Test()
        {
            var rootNode = new RootNode();
            var xmlNode = XmlNode.Parse("<root><note desc=\"1\" body=\"One!\"/><note desc=\"2\" body=\"Two?\"/></root>");
            rootNode.Register<IParentNodeImplementor>(xmlNode);

            Predicate<INode> nodePredicate = node => node.Name == "desc" && node.Parent.Name == "note";
            Predicate<IInvocation> messagePredicate = invocation => invocation.Method.DeclaringType == typeof (IAccessor<string>);
            var testRecipient = new TestRecipient();
            var nodeMessage = new NodeMessage
                                  {
                                      NodePredicate = nodePredicate,
                                      MessagePredicate = messagePredicate,
                                      Target = testRecipient
                                  };
            rootNode.InstallHook(nodeMessage);

            var stringProvider = rootNode.Nodes("note").First().Attribute("desc").Get<IAccessor<string>>();
            stringProvider.Value = "New Text";

            Assert.AreEqual("New Text", testRecipient.Value);
        }

        public class TestRecipient : IAccessor<string>
        {
            private string _text = "Not yet called";

            public string Value
            {
                get { return _text; }
                set { _text = value; }
            }
        }
    }
}