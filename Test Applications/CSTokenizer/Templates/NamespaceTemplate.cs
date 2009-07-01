using System.Collections.Generic;
using System.Linq;
using Model;

namespace CSTokenizer.Templates
{
    public class NamespaceTemplate : Template<CodeListing>
    {
        public NamespaceTemplate() : base(tokens => Matches(tokens)) { }

        private static bool Matches(ICollection<Token> tokens)
        {
            if (tokens.Count == 0) return false;
            return tokens.First().Characters == "namespace";
        }

        public override object Process(Token[] tokens, CodeListing @object)
        {
            var name = tokens.Skip(1).Aggregate("", (s, t) => s + t.Characters);
            var result = new Namespace {CodeListing = @object, Name = name};
            return result;
        }
    }
}