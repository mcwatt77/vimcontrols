using System;
using System.Xml.Linq;
using NUnit.Framework;
using UITemplateViewer.DynamicPath;

using ManyToOnePath = System.Linq.Expressions.Expression<System.Func<System.Collections.Generic.IEnumerable<NodeMessaging.IParentNode>, System.Collections.Generic.IEnumerable<NodeMessaging.IParentNode>>>;
using OneToManyPath = System.Linq.Expressions.Expression<System.Func<NodeMessaging.IParentNode, System.Collections.Generic.IEnumerable<NodeMessaging.IParentNode>>>;
using ManyToManyPath = System.Linq.Expressions.Expression<System.Func<NodeMessaging.IParentNode, System.Collections.Generic.IEnumerable<NodeMessaging.IParentNode>>>;
using OneToOnePath = System.Linq.Expressions.Expression<System.Func<NodeMessaging.IParentNode, NodeMessaging.IParentNode>>;
using EndNodePath = System.Linq.Expressions.Expression<System.Func<NodeMessaging.IParentNode, NodeMessaging.IEndNode>>;

namespace UITemplateViewer.Tests.DynamicPath
{
    [TestFixture]
    public class DecoderTest
    {

// ReSharper disable JoinDeclarationAndInitializer
        [Test]
        public void Test()
        {
            //TODO: Make the parser add exception nodes when strings don't fully match

            Decoder decode;

            var masterDoc = new XDocument(new XElement("doc")).Root;
            if (masterDoc == null) throw new Exception(".NET stopped working!");

            decode = Decoder.FromPath("[*]");
            masterDoc.Add(decode.Element);
            Assert.AreEqual("a => a.Nodes()", decode.Local.ToString());
            decode.Local.Compile();

            decode = Decoder.FromPath("[*]{/note}");
            masterDoc.Add(decode.Element);
            Assert.AreEqual("a => a.Nodes()", decode.Local.ToString());
            decode.Local.Compile();
            Assert.AreEqual("a => a.Root.Nodes(\"note\")", decode.Data.ToString());
            decode.Data.Compile();

            decode = Decoder.FromPath("{/note}[entityRow]");
            masterDoc.Add(decode.Element);
            Assert.AreEqual("a => a.Nodes(\"entityRow\")", decode.Local.ToString());
            decode.Local.Compile();
            Assert.AreEqual("a => a.Root.Nodes(\"note\")", decode.Data.ToString());
            decode.Data.Compile();

            decode = Decoder.FromPath("{@descr}");
            masterDoc.Add(decode.Element);
            Assert.AreEqual("a => a.Attribute(\"descr\")", decode.Data.ToString());
            decode.Data.Compile();

            decode = Decoder.FromPath("{/dyn::rowSelector}");
            masterDoc.Add(decode.Element);
            //TODO: This is pretty sketchy.  I should consider a different approach to handling namespaces.
            //And how will I remember to update Decoder when I change the interfaces around?
            //I need to get decode to build itself from hardcoded interface calls, so they'll break when I change the interface
            //  and not jus during testing
            Assert.AreEqual("a => a.Root.Nodes(\"dyn\", \"rowSelector\")", decode.Data.ToString());
            decode.Data.Compile();

            decode = Decoder.FromPath("[:noteList/@rows]");
            masterDoc.Add(decode.Element);
            Assert.AreEqual("a => a.Root.NodeById(\"noteList\").Attribute(\"rows\")", decode.Local.ToString());
            decode.Local.Compile();

            decode = Decoder.FromPath("[@rows[1]]");
            masterDoc.Add(decode.Element);
            Assert.AreEqual("a => a.Attribute(\"rows\").ElementAtOrDefault(0)", decode.Local.ToString());
            decode.Local.Compile();

            decode = Decoder.FromPath("[../@rowSelector]{@body}");
            masterDoc.Add(decode.Element);
            Assert.AreEqual("a => a.get_Parent().Attribute(\"rowSelector\")", decode.Local.ToString());
            decode.Local.Compile();
            Assert.AreEqual("a => a.Attribute(\"body\")", decode.Data.ToString());
            decode.Data.Compile();
        }
// ReSharper restore JoinDeclarationAndInitializer
    }
}