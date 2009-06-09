using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using NUnit.Framework;

namespace Parser
{
    [TestFixture]
    public class ParseTest
    {
        [Test]
        public void Test()
        {
            const string fileName = @"..\..\ParseNode.cs";
            var parser = new CsParser();
            var node = parser.Parse(new FileInfo(fileName));

            var element = node.GetXml();

            Assert.Greater(node.Children.Count(), 0);
        }
    }

    public class ParseNode
    {
        private StringBuilder _buffer;
        public StringBuilder Buffer
        {
            get
            {
                if (_buffer == null)
                    _buffer = new StringBuilder();
                return _buffer;
            }
        }

        public string Name { get; set; }
        public TextReader Data { get; set; }

        private IEnumerable<ParseNode> _children;
        public IEnumerable<ParseNode> Children
        {
            get
            {
                return _children ?? new ParseNode[] {};
            }
            set
            {
                _children = value;
            }
        }

        public virtual XElement GetXml()
        {
            var element = new XElement("node",
                                new XAttribute("name", Name),
                                Children.Select(child => child.GetXml()));
            if (Children.Count() == 0)
                element.Add(new XText(GetStringData()));
            return element;
        }

        //these things should be inside some sort of stream wrapper
        public string GetStringData()
        {
            Buffer.Append(Data.ReadToEnd());
            return Buffer.ToString();
        }

        public char ReadChar()
        {
            var i = Data.Read();
            if (i == -1) return '\0';
            var c = (char) i;
            Buffer.Append(c);
            return c;
        }

        public char ReadCharFromEnd()
        {
            return '\0';
        }

        public void ResetBuffer()
        {
            Buffer.Length = 0;
        }
    }

    public abstract class Parser
    {
        private readonly List<ParseTemplate> _parseTemplates;
        private readonly NullTemplate _nullTemplate;

        protected Parser()
        {
            _nullTemplate = new NullTemplate(Parse);
            _parseTemplates = InitializeTemplates();
        }

        public ParseNode Parse(FileInfo fileInfo)
        {
            var stream = fileInfo.OpenText();
            var node = new ParseNode {Name = "root", Data = stream};
            node.Children = Parse(node).ToArray();
            return node;
        }

        protected virtual bool MatchTemplate(Type templateType)
        {
            return GetType().Namespace == templateType.Namespace;
        }

        private bool FullMatchTemplate(Type templateType)
        {
            if (templateType == typeof(NullTemplate)
                || !typeof(ParseTemplate).IsAssignableFrom(templateType)
                || templateType.IsAbstract) return false;
            var b = MatchTemplate(templateType);
            if (!b) return false;
            if (templateType.GetConstructor(new[] { typeof(Func<ParseNode, IEnumerable<ParseNode>>) }) == null)
                throw new Exception("Matching types must have public parameterless constructors");
            return true;
        }

        private List<ParseTemplate> InitializeTemplates()
        {
            Func<ParseNode, IEnumerable<ParseNode>> apply = Parse;
            var list = GetType()
                .Assembly
                .GetTypes()
                .Where(FullMatchTemplate)
                .Select(type => (ParseTemplate) Activator.CreateInstance(type, apply))
                .ToList();
            return list;
        }

        protected virtual ParseTemplate FindTemplate(ParseNode node)
        {
            return _parseTemplates
                .FirstOrDefault(template => template.Matches(node))
                ?? _nullTemplate;
        }

        private IEnumerable<ParseNode> Parse(ParseNode node)
        {
            if (node.Children.Count() > 0)
                throw new Exception("Already parsed");

            var template = FindTemplate(node);
            return template.Parse(node);
        }
    }

    public class NullTemplate : ParseTemplate
    {
        public NullTemplate(Func<ParseNode, IEnumerable<ParseNode>> apply) : base(node => true, apply)
        {}

        public override IEnumerable<ParseNode> Parse(ParseNode node)
        {
            return null;
        }
    }

    public class CsParser : Parser
    {
    }

    public abstract class ParseTemplate
    {
        private readonly Predicate<ParseNode> _match;
        private readonly Func<ParseNode, IEnumerable<ParseNode>> _apply;

        protected ParseTemplate(Predicate<ParseNode> match, Func<ParseNode, IEnumerable<ParseNode>> apply)
        {
            _match = match;
            _apply = apply;
        }

        public bool Matches(ParseNode node)
        {
            return _match(node);
        }

        public abstract IEnumerable<ParseNode> Parse(ParseNode node);

        protected IEnumerable<ParseNode> ApplyTemplates(ParseNode node)
        {
            return _apply(node);
        }
    }

    public class StatementNode : ParseNode
    {}

    public class NestedStatementNode : ParseNode
    {
        public string CommandData { get; set; }

        public override XElement GetXml()
        {
            var element = base.GetXml();
            element.Add(new XAttribute("command", CommandData));
            return element;
        }
    }

    public class StatementParserNode : ParseTemplate
    {
        public StatementParserNode(Func<ParseNode, IEnumerable<ParseNode>> apply) : base(node => true, apply)
        {}

        //There are a couple of ways to accomplish this...
        //When I hit {, I could go into recursive mode
        //Alternatively, when I hit { I could seek ahead for the end node
        //This causes hard to follow looping logic if done correctly though
        public override IEnumerable<ParseNode> Parse(ParseNode node)
        {
            var c = node.ReadChar();
            while (c != '\0')
            {
//                while (c != ';' && c != '{' && c != '\0' && c != '/' && c != '\'' && c != '"')
                while (c != ';' && c != '{' && c != '\0' && c != '/' && c != '\'' && c != '"')
                    c = node.ReadChar();

                if (c == '"')
                {
                    c = node.ReadChar();
                    while (c == '\\')
                    {
                        node.ReadChar();
                        c = node.ReadChar();
                    }
                    while (c != '"')
                    {
                        c = node.ReadChar();
                        {
                            while (c == '\\')
                            {
                                node.ReadChar();
                                c = node.ReadChar();
                            }
                        }
                    }
                    c = node.ReadChar();
                }
                if (c == '/')
                {
                    c = node.ReadChar();
                    if (c == '/')
                    {
                        while (c != '\r')
                            c = node.ReadChar();
                    }
                }
                if (c == '\'')
                {
                    c = node.ReadChar();
                    if (c == '\\')
                        node.ReadChar();
                    c = node.ReadChar();
                    continue;
                }
                if (c == ';')
                {
                    yield return new StatementNode
                                     {
                                         Name = "statement",
                                         Data = new StringReader(node.Buffer.ToString().TrimStart(' ', '\r', '\n', '\t'))
                                     };
                    node.ResetBuffer();
                    c = node.ReadChar();
                    continue;
                }
                if (c == '{')
                {
                    var nestCount = 1;
                    while (nestCount > 0)
                    {
                        c = node.ReadChar();
                        //Would be much better to create CharacterLiteral, StringLiteral, Comment classes, then parse them
                        if (c == '/')
                        {
                            c = node.ReadChar();
                            if (c == '/')
                            {
                                while (c != '\r')
                                    c = node.ReadChar();
                            }
                        }
                        if (c == '\'')
                        {
                            c = node.ReadChar();
                            if (c == '\\')
                                node.ReadChar();
                            c = node.ReadChar();
                            continue;
                        }
                        if (c == '{') nestCount++;
                        if (c == '}') nestCount--;
                    }
                    var stringData = node.Buffer.ToString().TrimStart(' ', '\r', '\n', '\t');
                    var firstBracket = stringData.IndexOf('{');
                    var innerData = stringData.Substring(firstBracket + 1, stringData.Length - firstBracket - 2).Trim();
                    var nestedNode = new NestedStatementNode
                                         {
                                             CommandData = stringData.Substring(0, firstBracket - 1).Trim(),
                                             Name = "nestedStatement",
                                             Data = new StringReader(innerData)
                                         };

                    if (nestedNode.CommandData == "public virtual XElement GetXml()")
                    {
                        int debug = 0;
                    }

                    nestedNode.Children = ApplyTemplates(nestedNode).ToArray();
                    yield return nestedNode;
                    node.ResetBuffer();
                    c = node.ReadChar();
                    continue;
                }
                yield break;
            }
        }
    }
}