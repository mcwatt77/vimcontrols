/*
 * 
 * I need to use a tokenizer.
 * The tokenizer will have these states (at a minimum):
 *      Code, StringLiteral, CharacterLiteral, SingleLineComment, MultiLineComment
 *   (might also have Identifier, for allowing numbers in the identifier... will need to)
 * Each character, within it's tokenizer state will either:
 *  AddToToken(), CreateNewToken(), SetTokenState().
 *  
 * Nesting operators (), {}, [], etc, will have unique tokens...
 * Inside comments or literals, these characters won't generate tokens.
 * 
 * Trying to build from that Token stream instead of from the character stream will be far easier.
 * 
 * I could read token patterns, or have some sort of token aggregator, or something.
 * 
 * But that's a crucial step, and should be relatively easy.  It's also a common way to solve the problem.
 * 
 */


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
            return new ParseNode[] {};
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
}