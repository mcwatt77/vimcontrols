using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;
using NodeMessaging;
using Utility.Core;

using ManyToOnePath = System.Linq.Expressions.Expression<System.Func<System.Collections.Generic.IEnumerable<NodeMessaging.IParentNode>, NodeMessaging.IParentNode>>;
using OneToManyPath = System.Linq.Expressions.Expression<System.Func<NodeMessaging.IParentNode, System.Collections.Generic.IEnumerable<NodeMessaging.IParentNode>>>;
using ManyToManyPath = System.Linq.Expressions.Expression<System.Func<NodeMessaging.IParentNode, System.Collections.Generic.IEnumerable<NodeMessaging.IParentNode>>>;
using OneToOnePath = System.Linq.Expressions.Expression<System.Func<NodeMessaging.IParentNode, NodeMessaging.IParentNode>>;
using EndNodePath = System.Linq.Expressions.Expression<System.Func<NodeMessaging.IParentNode, NodeMessaging.IEndNode>>;

namespace UITemplateViewer.DynamicPath
{
            //TODO: This is pretty sketchy.  I should consider a different approach to handling namespaces.
            //And how will I remember to update Decoder when I change the interfaces around?
            //I need to get decode to build itself from hardcoded interface calls, so they'll break when I change the interface
            //  and not jus during testing

    public class Decoder
    {
        private Decoder(XElement element)
        {
            Element = element;
        }

        private static readonly Dictionary<string, Func<XElement, string, LambdaExpression>> _templates = InitializeTemplates();

        public LambdaExpression Local { get; private set; }
        public LambdaExpression Data { get; private set; }

        public IEnumerable<DecodedExpression> Expressions { get; private set; }

        public XElement Element { get; private set; }

        public override string ToString()
        {
            return Element.ToString();
        }

        public static Decoder FromPath(string path)
        {
            var fileInfo = new FileInfo(@"..\..\DynamicPath\syntax.p");
            var parser = new PathParser(fileInfo.OpenText());

            var doc = parser.Parse(path);
            var decoder = new Decoder(doc);
            decoder.SetExpressionFromParsedDoc();
            return decoder;
        }

        private void SetExpressionFromParsedDoc()
        {
            var expressions = new List<DecodedExpression>();
            Element.Elements().Do(element =>
                                      {
                                          var parsed = ParseElement(element);
                                          if (element.Name.LocalName == "local_doc_path_capture")
                                              expressions.Add(new DecodedExpression(Local = parsed) { Local = true });
                                          else if (element.Name.LocalName == "std_xpath_capture")
                                              expressions.Add(new DecodedExpression(Data = parsed) {Local = false});
                                      });
            Expressions = expressions;
        }

        private static LambdaExpression ParseElement(XElement element)
        {
            if (element == null) return null;

            if (element.Name.LocalName == "literal_statement")
                return BuildLiteralCall(element.Attribute("data").Value);

            var result = element
                .Elements("elementGroup")
                .Select(elem => ProcessElementGroup(elem))
                .ToList();

            return result.Count() == 2
                                ? CombineCalls(result.ToArray())
                                : result.First();
        }

        private static LambdaExpression BuildLiteralCall(string value)
        {
            var constant = Expression.Constant(value);
            return Expression.Lambda(typeof (Func<INode, string>), constant, Expression.Parameter(typeof(INode), "node"));
        }

        private static LambdaExpression CombineCalls(params Expression[] expressions)
        {
            var e = (MethodCallExpression)(((LambdaExpression) expressions.Skip(1).First()).Body);
            var path = (LambdaExpression)expressions.First();
            var le = Expression.Call(path.Body, e.Method, e.Arguments);

            ParameterExpression parameter;
            if (typeof(MemberExpression).IsAssignableFrom(le.Object.GetType()))
            {
                var me = (MemberExpression) le.Object;
                parameter = (ParameterExpression) me.Expression;
            }
            else if (typeof(MethodCallExpression).IsAssignableFrom(le.Object.GetType()))
            {
                var mce = (MethodCallExpression)le.Object;
                if (typeof(MemberExpression).IsAssignableFrom(mce.Object.GetType()))
                {
                    var me = (MemberExpression)mce.Object;
                    parameter = (ParameterExpression)me.Expression;
                }
                else if (typeof(ParameterExpression).IsAssignableFrom(mce.Object.GetType()))
                    parameter = (ParameterExpression) mce.Object;
                else
                    throw new Exception("CombineCall could not interpret expressions");
            }
            else
                throw new Exception("CombineCall could not interpret expressions");

            return Expression.Lambda(le, parameter);
        }

        private static LambdaExpression ProcessElementGroup(XContainer element)
        {
            var node = element.Element("node");
            var filter = element.Element("filter");
            var expr = ProcessNode(node);
            if (filter != null)
            {
                var filterData = filter.Attribute("data").Value;
                if (filterData == "1")
                {
                    var filterExpr = (ManyToOnePath) (a => a.ElementAtOrDefault(0));
//                    expr = CombineCalls(filterExpr, expr);
                }
                else
                    throw new Exception("Can only support filter '1'");
            }

            if (element.PreviousNode != null) return expr;
            var hierarchy = GetElementValue(element, "hierarchy");
            return hierarchy != null
                       ? CombineCalls(((OneToOnePath) (a => a.Root)), expr)
                       : expr;
        }

        private static string GetElementValue(XContainer element, string elementName)
        {
            var subElement = element.Element(elementName);
            if (subElement == null) return null;
            var attr = subElement.Attribute("data");
            return attr == null ? null : attr.Value;
        }

        private static Dictionary<string, Func<XElement, string, LambdaExpression>> InitializeTemplates()
        {
            var templates = new Dictionary<string, Func<XElement, string, LambdaExpression>>();
            templates["wildcard"] = (innerNode, data) => BuildDynamicCall("Nodes");
            templates["element_capture"] = (innerNode, data) => BuildElementCapture(innerNode);
            templates["attribute"] = (innerNode, data) => BuildDynamicCall("Attribute", data);
            templates["parent"] = (innerNode, data) => BuildDynamicCall("get_Parent");
            templates["element_lookup"] = (innerNode, data) => CombineCalls(((OneToOnePath) (a => a.Root)), BuildDynamicCall("NodeById", data));
            return templates;
        }

        private static LambdaExpression BuildElementCapture(XContainer innerNode)
        {
            return BuildDynamicCall("Nodes", new[]
                                                 {
                                                     GetElementValue(innerNode, "namespace"),
                                                     GetElementValue(innerNode, "element_name")
                                                 }
                                                 .Where(name => name != null)
                                                 .ToArray());
        }

        private static LambdaExpression ProcessNode(XContainer element)
        {
            var innerNode2 = element.Elements().First();
            var elementName = innerNode2.Name.LocalName;
            if (_templates.ContainsKey(elementName))
                return _templates[elementName](element.Elements().First(), GetElementValue(element, innerNode2.Name.LocalName));

            throw new Exception("Could not interpret " + innerNode2.Name.LocalName);
        }

        private static LambdaExpression BuildDynamicCall(string methodName, params object[] @params)
        {
            var methods = typeof (IParentNode).GetMethods().Concat(typeof (INode).GetMethods());
            var methodInfo = methods.Single(
                method => method.Name == methodName && method.GetParameters().Count() == @params.Length);
            return methodInfo.BuildLambda(@params);
        }
    }
}