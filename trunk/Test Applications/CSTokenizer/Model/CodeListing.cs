using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Model
{
    public interface IHasXml
    {
        XElement GetXml();
    }

    public class CommentStatement : Statement
    {
        public override XElement GetXml() { return new XElement("comment", Text); }
    }

    public class Statement : IHasXml
    {
        public string Text { get; set; }
        public virtual XElement GetXml() { return new XElement("statement", Text); }
    }

    public class Variable : IHasXml
    {
        public string Name { get; set; }
        public XElement GetXml() { return new XElement("variable", new XAttribute("name", Name)); }
    }

    public class NestedStatement : Statement
    {
        public List<Statement> Statements = new List<Statement>();
        public List<Variable> LocalVariables = new List<Variable>();
        public override XElement GetXml()
        {
            if (Statements.Count == 0) return base.GetXml();

            return new XElement("nestedStatement",
                                LocalVariables.Select(local => local.GetXml()),
                                new XElement("statements", Statements.Select(statement => statement.GetXml())));
        }
    }

    public class VariableAssignment : NestedStatement
    {
        public Variable Variable { get; set; }
        public override XElement GetXml()
        {
            return new XElement("variableAssignment",
                                new XAttribute("name", Variable.Name),
                                new XElement("statements", Statements.Select(statement => statement.GetXml())));
        }
    }

    public class Assembly : IHasXml
    {
        public List<Class> Classes = new List<Class>();
        public XElement GetXml() { return new XElement("assembly", Classes.Select(@class => @class.GetXml()));}
    }
    public class CodeListing : IHasXml
    {
        public Assembly Assembly { get; set; }
        public List<Using> Usings = new List<Using>();
        public XElement GetXml()
        {
            return new XElement("codeListing",
                                Usings.Select(@using => @using.GetXml()),
                                Assembly.GetXml());
        }
    }
    public class Namespace
    {
        public CodeListing CodeListing { get; set; }
        public string Name { get; set; }
    }
    public class Using : IHasXml
    {
        public string Namespace { get; set; }
        public XElement GetXml() { return new XElement("using", new XAttribute("namespace", Namespace)); }
    }
    public class Class : IHasXml
    {
        public string Namespace { get; set; }
        public string Name { get; set; }
        public List<Method> Methods = new List<Method>();
        public XElement GetXml()
        {
            return new XElement("class",
                                new XAttribute("namespace", Namespace),
                                new XAttribute("name", Name),
                                new XElement("methods",
                                             Methods.Select(method => method.GetXml())));
        }
    }
    public class Method : NestedStatement
    {
        public string Name { get; set; }
        public override XElement GetXml() { return new XElement("method", new XAttribute("name", Name), base.GetXml()); }
    }
}