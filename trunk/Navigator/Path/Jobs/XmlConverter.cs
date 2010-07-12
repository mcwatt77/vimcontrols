using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Navigator.Path.Jobs
{
    public class XmlConverter
    {
        public static XDocument ConvertToXml(string source)
        {
            var tokens = GetTokens(source).ToArray();

            var currentElement = new XElement("root");
            var rootElement = currentElement;
            XAttribute currentAttribute = null;

            foreach (var token in tokens)
            {
// ReSharper disable PossibleNullReferenceException
                try
                {
                    if (currentAttribute != null)
                    {
                        currentAttribute.Value = token.Type == TokenType.AttributeValue
                                                     ? token.Text
                                                     : currentAttribute.Name.LocalName;
                        currentAttribute = null;
                    }
                    if (token.Type == TokenType.Text)
                        currentElement.Add(new XText(token.Text));
                    if (token.Type == TokenType.Element)
                    {
                        var newElement = new XElement(token.Text);
                        currentElement.Add(newElement);
                        currentElement = newElement;
                    }
                    if (token.Type == TokenType.EndElement)
                    {
                        if (currentElement.Parent != null)
                            currentElement = currentElement.Parent;
                    }
                    if (token.Type == TokenType.Attribute)
                    {
                        if (token.Text == "xmlns") continue;

                        currentAttribute = new XAttribute(token.Text, "");
                        currentElement.Add(currentAttribute);
                    }
                }
                catch
                {
                    currentAttribute = null;
                }
// ReSharper restore PossibleNullReferenceException
            }

            var document = new XDocument();
            document.Add(rootElement);
            return document;
        }

        private static IEnumerable<Token> GetTokens(IEnumerable<char> source)
        {
            ParseState parseState = new ElementStart();

            foreach (var c in source)
            {
                var parseResult = parseState.Process(c);
                if (parseResult == null)
                    continue;
                if (parseResult.TokenResult != null)
                    yield return parseResult.TokenResult;
                if (parseResult.NextParseState != null)
                    parseState = parseResult.NextParseState;
            }
            var token = parseState.Complete();
            if (token != null)
                yield return token;
        }

        private class ParseResult
        {
            public ParseState NextParseState { get; set; }
            public Token TokenResult { get; set; }
        }

        private class Start : ParseState
        {
            private readonly Token _token = new Token(TokenType.Text);

            public override ParseResult Process(char c)
            {
                if (c == '<')
                    return new ParseResult
                               {
                                   NextParseState = new ElementStart(),
                                   TokenResult = !_token.IsPureWhitespace() ? _token : null
                               };
                _token.Append(c);
                return null;
            }

            public override Token Complete()
            {
                return _token;
            }
        }

        private class ElementStart : ParseState
        {
            public override ParseResult Process(char c)
            {
                if (c == '<') return null;
                if (IsWhitespace(c)) return null;
                if (c == '!' || c == '?') return new ParseResult {NextParseState = new SpecialInstruction()};
                if (c == '/') return new ParseResult {NextParseState = new EndElement()};
                return new ParseResult {NextParseState = new Element(c)};
            }
        }

        private class EndElement : ParseState
        {
            private readonly Token _token = new Token(TokenType.EndElement);

            public override ParseResult Process(char c)
            {
                if (IsWhitespace(c)) return new ParseResult {NextParseState = new Attribute(), TokenResult = _token};
                if (c == '>') return new ParseResult {NextParseState = new Start(), TokenResult = _token};
                _token.Append(c);
                return null;
            }
        }

        private class Element : ParseState
        {
            private readonly Token _token = new Token(TokenType.Element);

            public Element(char startChar)
            {
                _token.Append(startChar);
            }

            public override ParseResult Process(char c)
            {
                if (IsWhitespace(c)) return new ParseResult {NextParseState = new Attribute(), TokenResult = _token};
                if (c == '>') return new ParseResult {NextParseState = new Start(), TokenResult = _token};
                if (c == '/') return new ParseResult {NextParseState = new EndingElement(), TokenResult = _token};
                _token.Append(c);
                return null;
            }
        }

        private class EndingElement : ParseState
        {
            public override ParseResult Process(char c)
            {
                if (IsWhitespace(c)) return null;
                if (c == '>')
                    return new ParseResult
                               {
                                   NextParseState = new Start(),
                                   TokenResult = new Token(TokenType.EndElement)
                               };
                return new ParseResult {NextParseState = new Attribute(c)};
            }
        }

        private class Attribute : ParseState
        {
            private readonly Token _token = new Token(TokenType.Attribute);

            public Attribute()
            {}

            public Attribute (char startChar)
            {
                _token.Append(startChar);
            }

            public override ParseResult Process(char c)
            {
                if (IsWhitespace(c) && !_token.IsPureWhitespace())
                    return new ParseResult {NextParseState = new AttributeValueStart(), TokenResult = _token};
                if (IsWhitespace(c)) return null;
                if (c == '=') return new ParseResult {NextParseState = new AttributeValueStart(), TokenResult = _token};
                if (c == '>')
                    return new ParseResult
                               {
                                   NextParseState = new Start(),
                                   TokenResult = !_token.IsPureWhitespace() ? _token : null
                               };
                if (c == '/')
                    return new ParseResult
                               {
                                   NextParseState = new EndingElement(),
                                   TokenResult = !_token.IsPureWhitespace() ? _token : null
                               };
                _token.Append(c);
                return null;
            }
        }

        private class AttributeValueStart : ParseState
        {
            public override ParseResult Process(char c)
            {
                if (c == '>') return new ParseResult {NextParseState = new Start()};
                if (c == '/')
                    return new ParseResult {NextParseState = new EndingElement()};
                if (IsWhitespace(c)) return null;
                if (c == '=') return null;
                if (c == '"') return new ParseResult {NextParseState = new AttributeValue()};
                return new ParseResult {NextParseState = new Attribute(c)};
            }
        }

        private class AttributeValue : ParseState
        {
            private readonly Token _token = new Token(TokenType.AttributeValue);

            public override ParseResult Process(char c)
            {
                if (c == '"') return new ParseResult {NextParseState = new Attribute(), TokenResult = _token};
                _token.Append(c);
                return null;
            }
        }

        private class SpecialInstruction : ParseState
        {
            public override ParseResult Process(char c)
            {
                if (c == '>') return new ParseResult {NextParseState = new Start()};
                return null;
            }
        }

        private abstract class ParseState
        {
            public abstract ParseResult Process(char c);

            public virtual Token Complete()
            {
                return null;
            }

            public static bool IsWhitespace(char c)
            {
                return c == '\r' || c == '\n' || c == ' ';
            }
        }

        private enum TokenType
        {
            EndElement, Element, Attribute, AttributeValue, Text
        }

        private class Token
        {
            private int _length;
            private StringBuilder TextData { get; set; }
            private bool _pureWhitespace = true;

            public Token(TokenType type)
            {
                TextData = new StringBuilder();
                Type = type;
            }

            public void Append(char c)
            {
                TextData.Append(c);
                _length++;

                if (_pureWhitespace) return;
                _pureWhitespace = ParseState.IsWhitespace(c);
            }

            public bool IsPureWhitespace()
            {
                return _pureWhitespace;
            }

            public string Text
            {
                get
                {
                    return TextData.ToString();
                }
            }

            public TokenType Type { get; private set; }
        }
    }
}