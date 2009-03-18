using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Xml.Linq;
using ActionDictionary;
using ActionDictionary.Interfaces;
using NUnit.Framework;

namespace DataProcessors.Tests
{
    [TestFixture]
    public class SpecialMessageTest
    {
        private RootNode _rootNode;
        private IParentNode _node;

        [Test]
        public void TestRegisterGet()
        {
            _rootNode.Register("test");
            Assert.AreEqual("test", _rootNode.Get<string>());
        }

        [Test]
        public void TestRegisterINode()
        {
            _rootNode.Register(new TestNode());
            Assert.AreEqual(typeof (ResultNode), _rootNode.NodeAt(0).GetType());
        }

        [Test]
        public void TestDataRetrieval()
        {
            var result = _node.Get<IStringProvider>();
            Assert.AreEqual("one", result.Text);

            //TODO: what if I want to send a message to all endPoints, IE, all INodes that aren't IParentNodes?
            //then those nodes would have a reference to their parent, where they could create an XDocument
        }

        [SetUp]
        public void setup()
        {
            _rootNode = new RootNode();
            _rootNode.Register(XmlNode.FromString("<note descr=\"1\" body=\"one\"/>"));
            _node = _rootNode.Nodes("note").First().Attribute("body");
        }

        [Test]
        public void TestMessageHooking()
        {
            Func<IStringProvider, string> fnFilter = a => a.Text;
            var hooktest = new HookTest();
            _node.InstallHook(fnFilter, hooktest);
            var provider = _node.Get<IStringProvider>();
                    //should this work in the reverse order?  If I get the interface first, then add the hook?
                    //Should I be explicitly assured that it fails?
            provider.Text = "New Text";
            Assert.AreEqual("New Text", hooktest.Text);

            //TODO:  So once the above is solid, then I can add code that builds up matching handlers, and it will be responsible
            //for maintaining child parent relationships and what not.
            //It will add complementary Message handlers and UI elements that respond to the messages.
        }

        public class HookTest : IStringProvider
        {
            private string _text = "Not called";

            public string Text
            {
                get { return _text; }
                set { _text = value; }
            }
        }

        [Test]
        public void CopyTest()
        {
            //add the data provider
            //add filters that correspond to properties and for-each type elements
            //  The property and for-each elements trap ICopy.Copy messages
            //Send the ICopy.Copy message to all EndPoint nodes


            //For what I'm doing below, ICopy.Copy copies the initial condition data to the UIElements that display them.

            var fileName = @"..\..\noteviewer\ui.xml";

            _rootNode.Register(XmlNode.FromString("<note descr=\"1\" body=\"one\"/>"));
            _rootNode.Register(new AggregateNode(
                                   XmlNode.FromString("<note descr=\"1\" body=\"one\"/>"),
                                   MetadataNode.FromString(fileName)
                                   ));



            var doc = XDocument.Load(fileName);



            var path = doc.Element("entityList").Attribute("path").Value;
            var expr = (Expression<Action<ICopy, INode>>)((obj, node) => obj.Copy(node));

            //path parsing looks something like this:
            /*
             * //n*ote -> if element.name == "note"
             * if you just passed in n*ote, you've got to pass that in relative to a specific node
             *      I'll probably unwind it
             * @descr will become Attribute == "descr", and Parent.Name == "note"
             * So you need some sort of Path chain... or PathCondition object, where you can add
             *      strings with respect to other objects
             *      
             * 
             * And do something like INode.MeetsCondition(PathCondition)
             * PathCondition.FromString("//n*ote")
             * 
             * You could do something like...  PathCondition.FromCurrent("@descr");
             * That will return a PathCondition object that has Name == "descr" and Parent.Name == "note"
             */

            var sb = new StringBuilder();
            _rootNode.InstallHook(path, new MissingTest(sb), expr);
            _rootNode.NodeAt(0).Send(Message.Create(expr, typeof(ICopy)));

            Assert.AreEqual("hit it", sb.ToString());




            sb.Length = 0;
            path = doc.Element("entityList").Element("navigation").Attribute("display").Value;
            _rootNode.InstallHook(path, new MissingTest(sb), expr);
            _rootNode.NodeAt(0).Attribute("descr").Send(Message.Create(expr, typeof (ICopy)));
            Assert.AreEqual("hit it", sb.ToString());


            //This wouldn't be part of copy... this is part of UI display
            //This implies the creation of a watchable data object at:
            //metadata::ui[@id = 'noteViewer']/entityList[1]/navigation[1]/@selectedIndex

            var attr = _rootNode
                .Nodes("metadata::ui")
                .Where(node => node.Attribute("id").Get<IStringProvider>().Text == "noteViewer").Single()
                .Nodes("entityList").First()
                .Nodes("navigation").First()
                .Attribute("selectedIndex");

            //Should actually install two hooks here... one to update navigation list, one to update text data
            attr.InstallHook(expr, new MissingTest(sb));
            attr.Register<IStringProvider>(new StringProvider("1"));
        }


        //I feel like copy should be receiving the data, plus the INode... or maybe just the INode
        public interface ICopy
        {
            void Copy(INode node);
        }

        public class MissingTest : ICopy
        {
            private readonly StringBuilder _sb;

            public MissingTest(StringBuilder sb)
            {
                _sb = sb;
            }

            public void Copy(INode node)
            {
                _sb.Append("hit it");
            }
        }
    }

    public class MetadataNode : IParentNode
    {
        public static MetadataNode FromString(string s)
        {
            return null;
        }

        public T Get<T>()
        {
            throw new System.NotImplementedException();
        }

        public void Register<T>(T t)
        {
            throw new System.NotImplementedException();
        }

        public Message Send(Message message)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Type> RegisteredTypes
        {
            get { throw new System.NotImplementedException(); }
        }

        public void InstallHook<T>(T tHook, object recipient)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<IParentNode> Nodes(string nameFilter)
        {
            throw new System.NotImplementedException();
        }

        public IParentNode NodeAt(int index)
        {
            throw new System.NotImplementedException();
        }

        public IParentNode Attribute(string name)
        {
            throw new System.NotImplementedException();
        }
    }

    public class AggregateNode : IParentNode
        {
            public AggregateNode(params IParentNode[] nodes)
            {}

            public T Get<T>()
            {
                throw new System.NotImplementedException();
            }

            public void Register<T>(T t)
            {
                throw new System.NotImplementedException();
            }

            public Message Send(Message message)
            {
                throw new System.NotImplementedException();
            }

            public IEnumerable<Type> RegisteredTypes
            {
                get { throw new System.NotImplementedException(); }
            }

            public void InstallHook<T>(T tHook, object recipient)
            {
                throw new System.NotImplementedException();
            }

            public IEnumerable<IParentNode> Nodes(string nameFilter)
            {
                throw new System.NotImplementedException();
            }

            public IParentNode NodeAt(int index)
            {
                throw new System.NotImplementedException();
            }

            public IParentNode Attribute(string name)
            {
                throw new System.NotImplementedException();
            }
        }

    public interface IStringProvider
    {
        string Text { get; set; }
    }

    public class StringProvider : IStringProvider
    {
        public StringProvider(string src)
        {
            Text = src;
        }

        public string Text { get; set; }
    }

    public class StringProviderWrapper : IStringProvider
    {
        private readonly IStringProvider _src;
        private readonly IStringProvider _hooked;

        public StringProviderWrapper(IStringProvider src, IStringProvider hooked)
        {
            _src = src;
            _hooked = hooked;
        }

        public string Text
        {
            get { return _src.Text; }
            set
            {
                _src.Text = value;
                _hooked.Text = value;
            }
        }
    }

    public class XmlNode : IParentNode
    {
        private class FilterHookPair
        {
            private readonly object _filter;
            private readonly object _hook;

            public FilterHookPair(object filter, object hook)
            {
                _filter = filter;
                _hook = hook;
            }

            public bool Matches(object message)
            {
                var fnType = _filter.GetType();
                var args = fnType.GetGenericArguments();
                var messageType = args.First();
                return messageType == message;
            }

            public object Wrap(object src)
            {
                return new StringProviderWrapper((IStringProvider)src, (IStringProvider)_hook);
            }
        }

        private readonly XObject _obj;
        private readonly List<FilterHookPair> _filters = new List<FilterHookPair>();

        public static XmlNode FromString(string src)
        {
            return new XmlNode(XDocument.Parse(src));
        }

        private XmlNode(XObject obj)
        {
            _obj = obj;
        }

        public IEnumerable<IParentNode> Nodes(string nameFilter)
        {
            if (_obj.GetType() != typeof(XDocument)) throw new Exception("Needed to be an element, yo.");
            var result = ((XDocument)_obj).Elements(nameFilter).Select(elem => (IParentNode)(new XmlNode(elem)));
            return result;
        }

        public IParentNode NodeAt(int index)
        {
            throw new System.NotImplementedException();
        }

        public IParentNode Attribute(string name)
        {
            if (_obj.GetType() != typeof(XElement)) throw new Exception("Needed to be an element, yo.");
            return new XmlNode(((XElement) _obj).Attribute(name));
        }

        public T Get<T>()
        {
            object obj = new StringProvider(((XAttribute) _obj).Value);

            if (_filters.Count > 0)
            {
                var f = _filters.Where(filter => filter.Matches(typeof (T))).ToList();
                if (f.Count == 1)
                {
                    return (T) f.First().Wrap(obj);
                }
            }
            if (typeof(T) == typeof(IStringProvider))
            {
                return (T) obj;
            }

            if (typeof(T) != typeof(string)) throw new Exception("I really don't know what you're talking about.");
            object obj2 = ((XAttribute) _obj).Value;
            return (T) obj2;
        }

        public void Register<T>(T t)
        {
            throw new System.NotImplementedException();
        }

        public Message Send(Message message)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Type> RegisteredTypes
        {
            get { throw new System.NotImplementedException(); }
        }

        public void InstallHook<T>(T tHook, object recipient)
        {
            _filters.Add(new FilterHookPair(tHook, recipient));
        }
    }

    public class ResultNode : IParentNode
    {
        public IEnumerable<IParentNode> Nodes(string nameFilter)
        {
            throw new System.NotImplementedException();
        }

        public IParentNode NodeAt(int index)
        {
            throw new System.NotImplementedException();
        }

        public IParentNode Attribute(string name)
        {
            throw new System.NotImplementedException();
        }

        public T Get<T>()
        {
            throw new System.NotImplementedException();
        }

        public void Register<T>(T t)
        {
            throw new System.NotImplementedException();
        }

        public Message Send(Message message)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Type> RegisteredTypes
        {
            get { throw new System.NotImplementedException(); }
        }

        public void InstallHook<T>(T tHook, object recipient)
        {
            throw new System.NotImplementedException();
        }
    }


    public class TestNode : IParentNode
    {
        public IEnumerable<IParentNode> Nodes(string nameFilter)
        {
            throw new System.NotImplementedException();
        }

        public IParentNode NodeAt(int index)
        {
            return new ResultNode();
        }

        public IParentNode Attribute(string name)
        {
            throw new System.NotImplementedException();
        }

        public T Get<T>()
        {
            throw new System.NotImplementedException();
        }

        public void Register<T>(T t)
        {
            throw new System.NotImplementedException();
        }

        public Message Send(Message message)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Type> RegisteredTypes
        {
            get { throw new System.NotImplementedException(); }
        }

        public void InstallHook<T>(T tHook, object recipient)
        {
            throw new System.NotImplementedException();
        }
    }
}