using System.Collections.Generic;
using NodeMessaging;
using NUnit.Framework;

namespace UITemplateViewer.Tests
{
    [TestFixture]
    public class NodeBaseTest
    {
        [Test]
        public void Test()
        {
            var tc = new TestClass();
            var etc = new List<TestClass>();
            var otc = new List<object>();

            var nodeBase = new NodeBase(null, null);
            nodeBase.Register(otc);
            nodeBase.Get<IEnumerable<TestClass>>();
            //TODO: this is the case needed by my immediate code
        }

        [Test]
        public void TestFromEnumObjToEnumInst()
        {}
    }

    public class TestClass
    {}
}
