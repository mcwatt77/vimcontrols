using System.Linq;
using NUnit.Framework;
using PathContainer.Node;
using PathContainer.NodeImplementor;

namespace PathContainer.Tests.NodeImplementor
{
    [TestFixture]
    public class XmlImplementorTest
    {
        [Test]
        public void Test()
        {
            var xml = "<data><note desc=\"1\" body=\"One!\"/><note desc=\"2\" body=\"Two?\"/></data>";
            var xmlImplementor = XmlImplementor.Parse(xml);
            
            var descAttributeNodeValue = xmlImplementor
                .Nodes.First()
                .Nodes.GetByName("desc")
                .DataNodes.GetBySlotName(Slot.StringData)
                .Cast<string>();

            Assert.AreEqual("1", descAttributeNodeValue);
        }
    }
}