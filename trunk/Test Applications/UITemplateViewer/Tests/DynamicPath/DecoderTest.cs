using System.Collections.Generic;
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
using EndNodePath = System.Linq.Expressions.Expression<System.Func<NodeMessaging.IParentNode, NodeMessaging.IEndNode>>;

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
                if (result.Count() == 2)
                    decoder.Local = CombineCalls(result);
                else
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

        private Expression CombineCalls(List<Expression> expressions)
        {
/*                var e = (MethodCallExpression)(((LambdaExpression) expr).Body);
                OneToOnePath path = a => a.Root;
                var parameter = Expression.Parameter(typeof(IParentNode), "a");
                var le = Expression.Call(path.Body, e.Method, e.Arguments);
                return Expression.Lambda(le, parameter);*/
            
            var e = (MethodCallExpression)(((LambdaExpression) expressions.Skip(1).First()).Body);
//            OneToOnePath path = a => a.Root;
            var path = (LambdaExpression)expressions.First();
            var parameter = Expression.Parameter(typeof(IParentNode), "a");
            var le = Expression.Call(path.Body, e.Method, e.Arguments);
            return Expression.Lambda(le, parameter);
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
                var e = (MethodCallExpression)(((LambdaExpression) expr).Body);
                OneToOnePath path = a => a.Root;
                var parameter = Expression.Parameter(typeof(IParentNode), "a");
                var le = Expression.Call(path.Body, e.Method, e.Arguments);
                return Expression.Lambda(le, parameter);
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
                var nameSpace =  innerNode.Element("namespace");
                if (nameSpace == null)
                {
                    var methodInfo = typeof (IParentNode).GetMethods().Single(
                        method => method.Name == "Nodes" && method.GetParameters().Count() == 1);
                    return methodInfo.BuildLambda(name);
                }
                else
                {
                    var methodInfo = typeof (IParentNode).GetMethods().Single(
                        method => method.Name == "Nodes" && method.GetParameters().Count() == 2);
                    return methodInfo.BuildLambda(nameSpace.Attribute("data").Value, name);
                }
            }
            if (innerNode.Name.LocalName == "attribute")
            {
                return BuildDynamicCall("Attribute", innerNode.Attribute("data").Value);
            }
            if (innerNode.Name.LocalName == "element_lookup")
            {
                return BuildDynamicCall("NodeById", innerNode.Attribute("data").Value);
            }

            return null;
        }

        private Expression BuildDynamicCall(string methodName, params object[] @params)
        {
            var methodInfo = typeof (IParentNode).GetMethods().Single(
                method => method.Name == methodName && method.GetParameters().Count() == @params.Length);
            return methodInfo.BuildLambda(@params);
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
            doit(decode);
            Assert.AreEqual("a => a.Attribute(\"descr\")", decode.Data.ToString());

            decode = Decoder.FromPath("{/dyn::rowSelector}");
            masterDoc.Add(decode.Element);
            doit(decode);
            //TODO: This is pretty sketchy.  I should consider a different approach to handling namespaces.
            //And how will I remember to update Decoder when I change the interfaces around?
            //I need to get decode to build itself from hardcoded interface calls, so they'll break when I change the interface
            //  and not jus during testing
            Assert.AreEqual("a => a.Root.Nodes(\"dyn\", \"rowSelector\")", decode.Data.ToString());

            decode = Decoder.FromPath("[:noteList/@rows]");
            masterDoc.Add(decode.Element);
            doit(decode);
            Assert.AreEqual("a => a.Root.NodeById(\"noteList\").Attribute(\"rows\")", decode.Local.ToString());

            decode = Decoder.FromPath("[../@rowSelector]{@body}");
            masterDoc.Add(decode.Element);
            doit(decode);
            Assert.AreEqual("a => a.Parent.Attribute(\"rowSelector\")", decode.Local.ToString());
            Assert.AreEqual("a => a.Attribute(\"body\")", decode.Data.ToString());
        }
// ReSharper restore JoinDeclarationAndInitializer
    }
}