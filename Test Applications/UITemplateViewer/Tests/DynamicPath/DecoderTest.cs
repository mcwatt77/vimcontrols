using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;
using NodeMessaging;
using NUnit.Framework;
using UITemplateViewer.DynamicPath;

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
            Expression<Func<IParentNode, IEnumerable<INode>>> expr;

            var masterDoc = new XDocument(new XElement("doc")).Root;

            decode = Decoder.FromPath("[*]");
            masterDoc.Add(decode.Element);
            expr = parentNode => parentNode.Nodes().Cast<INode>();

            decode = Decoder.FromPath("[*]{/note}");
            masterDoc.Add(decode.Element);
            expr = parentNode => parentNode.Nodes().Cast<INode>();
//            expr = parentNode => parentNode.Root.Nodes("note").Cast<INode>();

            decode = Decoder.FromPath("{@descr}");
            masterDoc.Add(decode.Element);

            decode = Decoder.FromPath("{/dyn::rowSelector}");
            masterDoc.Add(decode.Element);

            decode = Decoder.FromPath("[:noteList/@rows]");
            masterDoc.Add(decode.Element);

            decode = Decoder.FromPath("[../@rowSelector]{@body}");
            masterDoc.Add(decode.Element);

            int debug = 0;
        }
// ReSharper restore JoinDeclarationAndInitializer
    }
}