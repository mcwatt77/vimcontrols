using System.Collections.Generic;
using Utility.Core;

namespace CSTokenizer
{
    public class TreeProcessor
    {
        private readonly IEnumerable<Token> _tokens;
        private readonly object _codeListing;
        private readonly TemplateCollection<Token[]> _collection;

        public TreeProcessor(IEnumerable<Token> tokens, object codeListing)
        {
            _collection = new TemplateCollection<Token[]>(GetType().Assembly, type => true);

            _tokens = tokens;
            _codeListing = codeListing;
        }

        public IEnumerable<Token> Tokens
        {
            get { return _tokens; }
        }

        public object CodeListing
        {
            get { return _codeListing; }
        }

        public void ProcessObjectTree()
        {
            var parents = new Stack<object>();
            parents.Push(CodeListing);

            var currentTokenList = new List<Token>();
            foreach(var token in Tokens)
            {
                if (token.Characters == ";")
                {
                    _collection.Transform(currentTokenList.ToArray(), parents.Peek());
                    currentTokenList.Clear();
                    continue;
                }
                if (token.Characters == "{")
                {
                    object peek = parents.Peek();

                    parents.Push(_collection.Transform(currentTokenList.ToArray(), peek));

                    currentTokenList.Clear();
                    continue;
                }
                if (token.Characters == "}")
                {
                    parents.Pop();
                    currentTokenList.Clear();
                    continue;
                }

                currentTokenList.Add(token);
            }
        }
    }
}