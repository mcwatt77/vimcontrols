using System;

namespace Parser2
{
    public class Parse2
    {
        public enum ParseState
        {
            RootLevel
        }

        private bool IsWhitespace(char c)
        {
            return c == ' ' || c == '\r' || c == '\n';
        }

        private void Ignore() {}
        private void LookFor(string s) {}

        public void BuildParser(char c, ParseState state)
        {
            if (IsWhitespace(c) && state == ParseState.RootLevel) Ignore();
            if (state == ParseState.RootLevel) LookFor("using");
            if (state == ParseState.RootLevel) LookFor("namespace");
        }
    }

    public class ParseTemplateContext
    {}
    public class Class
    {}
    public class ParseTemplate<TType>
    {
        protected ParseTemplate(ParseTemplateContext context, Predicate<TType> match)
        {}
    }

    public class MemberDeclaration : ParseTemplate<Class>
    {
        public MemberDeclaration(ParseTemplateContext context)
            : base(context, c => true)
        {}
    }
}