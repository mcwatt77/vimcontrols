using System.Collections.Generic;
using System.Linq;

namespace CSTokenizer
{
    public static class TokenNester
    {
        private static IEnumerable<Token> Process(IEnumerator<Token> eTokens, char? expectedTerminator)
        {
            var openNesting = new[] {"[", "("};
            while (eTokens.MoveNext())
            {
                var token = eTokens.Current;
                if (expectedTerminator != null && token.Characters.Length == 1)
                {
                    if (token.Characters[0] == expectedTerminator)
                        yield break;
                }

                if (token.CharacterType != CodeCharacterType.ControlCharacters)
                {
                    yield return token;
                    continue;
                }

                if (openNesting.Contains(token.Characters))
                {
                    var terminator = token.Characters == "[" ? ']' : ')';
                    token.Characters += terminator;
                    token.Children = Process(eTokens, terminator).ToList();
                    yield return token;
                }
                else yield return token;
            }
        }

        public static IEnumerable<Token> Process(IEnumerable<Token> tokens)
        {
            return Process(tokens.GetEnumerator(), null);
        }
    }
}