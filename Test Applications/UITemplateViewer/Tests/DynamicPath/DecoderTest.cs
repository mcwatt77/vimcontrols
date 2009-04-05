using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;
using NodeMessaging;
using NUnit.Framework;
using UITemplateViewer.DynamicPath;
using Utility.Core;

using ManyToOnePath = System.Linq.Expressions.Expression<System.Func<System.Collections.Generic.IEnumerable<NodeMessaging.IParentNode>, System.Collections.Generic.IEnumerable<NodeMessaging.IParentNode>>>;
using OneToManyPath = System.Linq.Expressions.Expression<System.Func<NodeMessaging.IParentNode, System.Collections.Generic.IEnumerable<NodeMessaging.IParentNode>>>;
using ManyToManyPath = System.Linq.Expressions.Expression<System.Func<NodeMessaging.IParentNode, System.Collections.Generic.IEnumerable<NodeMessaging.IParentNode>>>;
using OneToOnePath = System.Linq.Expressions.Expression<System.Func<NodeMessaging.IParentNode, NodeMessaging.IParentNode>>;

namespace UITemplateViewer.Tests.DynamicPath
{
    [TestFixture]
    public class DecoderTest
    {

        private void doit(Decoder decoder)
        {
            OneToOnePath expr = a => a;

            var element = decoder.Element;
            var localDocPathCapture = element.Element("local_doc_path_capture");
            var dataCapture = element.Element("std_xpath_capture");
            if (localDocPathCapture != null)
            {
                var result = localDocPathCapture
                    .Elements("elementGroup")
                    .Select(elem => ProcessElementGroup(elem))
                    .ToList();
                decoder.Local = result.First();
            }
            else decoder.Local = expr;
            if (dataCapture != null)
            {
                var result = dataCapture
                    .Elements("elementGroup")
                    .Select(elem => ProcessElementGroup(elem))
                    .ToList();
                decoder.Data = result.First();
            }
            else decoder.Data = expr;
        }

        private Expression ProcessElementGroup(XElement element)
        {
            bool goToRoot = false;
            var hierarchy = element.Element("hierarchy");
            if (hierarchy != null)
            {
                if (hierarchy.Attribute("data").Value == "/")
                {
                    if (element.PreviousNode == null)
                        goToRoot = true;
                }
            }

            var node = element.Element("node");
            var expr = ProcessNode(node);

            if (goToRoot)
            {
                var e = ((LambdaExpression) expr).Body;
                var m = ((MethodCallExpression) e).Method;
                OneToOnePath path = a => a.Root;
                var lBody = path.Body;


                var parameter = Expression.Parameter(typeof(IParentNode), "a");
                OneToManyPath pe = a => a.Root.Nodes("note");

                var l = MemberExpression.MakeMemberAccess(parameter, typeof (INode).GetProperty("Root"));
                var le = Expression.Call(l, m, ConstantExpression.Constant("note"));

                return Expression.Lambda(le, parameter);

                //TODO:  Now I need to use this as an argument in building the ProcessNode expression
                int debug = 0;
            }

            return expr;
        }

        private Expression ProcessNode(XElement element)
        {
            var innerNode = element.Elements().First();
            if (innerNode.Name.LocalName == "wildcard")
            {
                OneToManyPath expr = a => a.Nodes();
                return expr;
            }
            if (innerNode.Name.LocalName == "element_capture")
            {
                var name = innerNode.Element("element_name").Attribute("data").Value;
                OneToManyPath expr = node => node.Nodes(name);
                var methodInfo = typeof (IParentNode).GetMethods().Single(
                    method => method.Name == "Nodes" && method.GetParameters().Count() == 1);
                return methodInfo.BuildLambda(name);
            }

            return null;
        }

// ReSharper disable JoinDeclarationAndInitializer
        [Test]
        public void Test()
        {
            //TODO: Make the parser add exception nodes when strings don't fully match

            Decoder decode;
            Expression expr;

            var masterDoc = new XDocument(new XElement("doc")).Root;

            decode = Decoder.FromPath("[*]");
            masterDoc.Add(decode.Element);
            doit(decode);
            Assert.AreEqual("a => a.Nodes()", decode.Local.ToString());

            decode = Decoder.FromPath("[*]{/note}");
            masterDoc.Add(decode.Element);
            doit(decode);
            Assert.AreEqual("a => a.Nodes()", decode.Local.ToString());
            Assert.AreEqual("a => a.Root.Nodes(\"note\")", decode.Data.ToString());

            decode = Decoder.FromPath("{@descr}");
            masterDoc.Add(decode.Element);

            decode = Decoder.FromPath("{/dyn::rowSelector}");
            masterDoc.Add(decode.Element);

            decode = Decoder.FromPath("[:noteList/@rows]");
            masterDoc.Add(decode.Element);

            decode = Decoder.FromPath("[../@rowSelector]{@body}");
            masterDoc.Add(decode.Element);
        }
// ReSharper restore JoinDeclarationAndInitializer
    }
}