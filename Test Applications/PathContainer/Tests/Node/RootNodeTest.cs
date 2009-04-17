using System;
using System.Linq;
using NUnit.Framework;
using PathContainer.Node;
using PathContainer.NodeImplementor;

namespace PathContainer.Tests.Node
{
    [TestFixture]
    public class RootNodeTest
    {
        public class Note
        {}

        [Test]
        public void Registered_constructors_return_the_same_instance_on_subsequent_calls()
        {
            //TODO:  Load the attributes before instantiating the object.
            //Later this will allow constructor injection.
            //If one of the children relies on this object, it gets a proxy that will replace itself later.
            //If something needs messaging, it might get a proxy.
            //Ideally proxies are kept to a minimum

            var xml = "<data><note desc=\"1\" body=\"One!\"/><note desc=\"2\" body=\"Two?\"/></data>";
            var rootNode = new RootNode();
            var xmlWrapper = rootNode.RegisterNodeImplementor("data", XmlImplementor.Parse(xml));
            rootNode.RegisterNodeRequestDelegate(ElementConstructor);

            var oneNode = xmlWrapper.Nodes.First();
            var desc = oneNode.Nodes.GetByName("body").DataNodes.GetBySlotName(Slot.StringData).Cast<string>();
            Assert.AreEqual("One!", desc);

            //TODO:  Ultimately I'm going to use this mechanism to instantiate the singleton objects I will use for my container
            //The attributes will fire first, and as they request objects, if they request an object that it's
            //    in the current chain, they'll get a proxy instead... it seems like I could get this working in the old line of code
            //    without much change

            var note1 = oneNode.DataNodes.GetBySlotName(Slot.ObjectInstance).Cast<Note>();

            oneNode = xmlWrapper.Nodes.First();
            var note2 = oneNode.DataNodes.GetBySlotName(Slot.ObjectInstance).Cast<Note>();

            Assert.AreSame(note1, note2);
        }

        private static IDataNode ElementConstructor(string slotName, INode parentNode, IDataNode dataNode)
        {
            if (dataNode != null) return dataNode;

            var newType = typeof (Note).Assembly.GetTypes().Single(type => type.Name.ToLower() == parentNode.Name.ToLower());
            return parentNode.DataNodes.Register(slotName, () => newType.GetConstructor(Type.EmptyTypes).Invoke(new object[]{}));
        }
    }
}