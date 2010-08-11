using System.Text;
using System.Xml.Linq;

namespace Navigator.Path.Jobs
{
    public class JsonToXmlConvert : XmlConverter
    {
        public new static XDocument ConvertToXml(string source)
        {
            return new JsonToXmlConvert().Convert(source);
        }

        protected override ParseState StartToken()
        {
            return new JsonStart();
        }
    }

    public class JsonStart : XmlConverter.ParseState
    {
        public override XmlConverter.ParseResult Process(char c)
        {
            if (IsWhitespace(c)) return null;
            if (c == '"')
                return new XmlConverter.ParseResult
                           {
                               NextParseState = new IdentifierStart()
                           };
            if (c == '{') return new XmlConverter.ParseResult {NextParseState = this, TokenResult = new GroupToken()};
            if (c == ']' || c == '}')
                return new XmlConverter.ParseResult
                           {
                               NextParseState = this,
                               TokenResult = new XmlConverter.Token(XmlConverter.TokenType.EndElement)
                           };
            return null;
        }
    }

    public class GroupToken : XmlConverter.Token
    {
        public GroupToken() : base(XmlConverter.TokenType.Element)
        {
            TextData = new StringBuilder("group");
        }
    }

    public class IdentifierStart : XmlConverter.ParseState
    {
        private readonly XmlConverter.Token _token = new XmlConverter.Token(XmlConverter.TokenType.Element);

        public override XmlConverter.ParseResult Process(char c)
        {
            if (c == '"') return new XmlConverter.ParseResult {NextParseState = new ValueOrChild(), TokenResult = _token};
            _token.Append(c);
            return null;
        }
    }

    public class ValueOrChild : XmlConverter.ParseState
    {
        public override XmlConverter.ParseResult Process(char c)
        {
            if (c == '"') return new XmlConverter.ParseResult {NextParseState = new ValueStart()};
            if (c == 'n') return new XmlConverter.ParseResult {NextParseState = new NullState()};
            if (c == '{') return new XmlConverter.ParseResult {NextParseState = new JsonStart(), TokenResult = new GroupToken()};
            if (c == '[') return new XmlConverter.ParseResult {NextParseState = new JsonStart()};

            if (c == ']' || c == '}')
                return new XmlConverter.ParseResult
                           {
                               NextParseState = this,
                               TokenResult = new XmlConverter.Token(XmlConverter.TokenType.EndElement)
                           };
            return null;
        }
    }

    public class NullState : XmlConverter.ParseState
    {
        public override XmlConverter.ParseResult Process(char c)
        {
            if (c == 'u' || c == 'l') return null;
            return new XmlConverter.ParseResult {NextParseState = new ValueEnd()};
        }
    }

    public class SkipSeparators : XmlConverter.ParseState
    {
        private readonly XmlConverter.ParseState _nextParseState;

        public SkipSeparators(XmlConverter.ParseState nextParseState)
        {
            _nextParseState = nextParseState;
        }

        public override XmlConverter.ParseResult Process(char c)
        {
            if (c == '}' || c == ']')
                return new XmlConverter.ParseResult
                           {
                               NextParseState = this,
                               TokenResult = new XmlConverter.Token(XmlConverter.TokenType.EndElement)
                           };
            if (IsWhitespace(c) || c == ',') return null;
            return _nextParseState.Process(c);
        }
    }

    public class ValueStart : XmlConverter.ParseState
    {
        private readonly XmlConverter.Token _token = new XmlConverter.Token(XmlConverter.TokenType.Text);

        public override XmlConverter.ParseResult Process(char c)
        {
            if (c == '"') return new XmlConverter.ParseResult {NextParseState = new ValueEnd(), TokenResult = _token};
            _token.Append(c);
            return null;
        }
    }

    public class ValueEnd : XmlConverter.ParseState
    {
        public override XmlConverter.ParseResult Process(char c)
        {
            if (c == '}')
                return new XmlConverter.ParseResult
                           {
                               NextParseState = this,
                               TokenResult = new XmlConverter.Token(XmlConverter.TokenType.EndElement)
                           };
            return new XmlConverter.ParseResult
                       {
                           NextParseState = new JsonStart(),
                           TokenResult = new XmlConverter.Token(XmlConverter.TokenType.EndElement)
                       };
        }
    }
}