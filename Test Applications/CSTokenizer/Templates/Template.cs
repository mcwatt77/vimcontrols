using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Utility.Core;

namespace CSTokenizer.Templates
{
    public abstract class Template<TObject> : TemplateCollection<Token[]>.Template<TObject>
    {
        protected Template(Expression<Predicate<Token[]>> match, int rank) : base(match, rank) { }
        protected Template(Expression<Predicate<Token[]>> match) : base(match) { }

        //TODO: I should be able to handle this be creating more handlers... more conditional states...
        //For instance hitting a semi-colon or something, or coming off identifiers... or something...
        //Creates a different state where it knows the difference between "less than" and "generic grouping"
        protected IEnumerable<Token> ProcessIdentifiers(Token[] tokens)
        {
            IEnumerator eTokens = tokens.GetEnumerator();
            while (eTokens.MoveNext())
            {
                var token = (Token)eTokens.Current;
                if (token.CharacterType != CodeCharacterType.IdentifierCharacters)
                {
                    yield return token;
                    continue;
                }

                if (!eTokens.MoveNext())
                {
                    yield return token;
                    yield break;
                }

                var nextToken = (Token) eTokens.Current;

                while (nextToken.CharacterType == CodeCharacterType.IdentifierCharacters)
                {
                    yield return token;
                    token = nextToken;
                    if (!eTokens.MoveNext())
                    {
                        yield return token;
                        yield break;
                    }
                    nextToken = (Token) eTokens.Current;
                }

                if (nextToken.CharacterType != CodeCharacterType.OperatorStrings || nextToken.Characters != "<")
                {
                    yield return token;
                    yield return nextToken;
                    continue;
                }

                if (token.Children.Count == 0)
                {
                    var tokenList = new List<Token>();
                    while (eTokens.MoveNext())
                    {
                        nextToken = (Token) eTokens.Current;
                        if (nextToken.CharacterType == CodeCharacterType.OperatorStrings && nextToken.Characters == ">")
                            break;
                        tokenList.Add(nextToken);
                    }

                    token.Children = tokenList;
                }
                yield return token;
            }
        }
    }
}