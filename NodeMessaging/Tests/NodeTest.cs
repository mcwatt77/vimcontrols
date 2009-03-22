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
            rootNode.Register<IParentNode>(xmlNode);

            Predicate<INode> nodePredicate = node => node.Name == "desc" && node.Parent.Name == "note";
            Predicate<IInvocation> messagePredicate = invocation => invocation.Method.DeclaringType == typeof (IStringProvider);
            var testRecipient = new TestRecipient();
            var nodeMessage = new NodeMessage
                                  {
                                      NodePredicate = nodePredicate,
                                      MessagePredicate = messagePredicate,
                                      Target = testRecipient
                                  };
            rootNode.InstallHook(nodeMessage);

            var stringProvider = rootNode.Nodes("note").First().Attribute("desc").Get<IStringProvider>();
            stringProvider.Text = "New Text";

            Assert.AreEqual("New Text", testRecipient.Text);
        }

        public class TestRecipient : IStringProvider
        {
            private string _text = "Not yet called";

            public string Text
            {
                get { return _text; }
                set { _text = value; }
            }
        }
    }
}