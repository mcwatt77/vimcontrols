using System.Collections.Generic;
using System.Linq;
using Model;

namespace CSTokenizer.Templates
{
    public class UsingTemplate : Template<CodeListing>
    {
        public UsingTemplate() : base(tokens => Matches(tokens)) { }

        private static bool Matches(ICollection<Token> tokens)
        {
            if (tokens.Count == 0) return false;
            return tokens.First().Characters == "using";
        }

        public override object Process(Token[] tokens, CodeListing @object)
        {
            var result = tokens.Skip(1).Aggregate("", (s, t) => s + t.Characters);
            var @using = new Using{Namespace = result};
            @object.Usings.Add(@using);
            return @using;
        }
    }
}