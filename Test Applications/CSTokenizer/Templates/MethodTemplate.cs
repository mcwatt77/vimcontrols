using System.Linq;
using CSTokenizer.Handlers;
using Model;

namespace CSTokenizer.Templates
{
    public class MethodTemplate : Template<Class>
    {
        public MethodTemplate() : base(tokens => Matches(tokens)) { }

        public static bool Matches(Token[] tokens)
        {
            return tokens
                       .Count(token => token.CharacterType == CodeCharacterType.ControlCharacters
                                       && token.Characters == "()") > 0
                   && tokens
                          .Count(token => token.CharacterType == CodeCharacterType.OperatorStrings
                                          && token.Characters == "=") == 0;
        }

        public override object Process(Token[] tokens, Class @object)
        {
            var comments = tokens
                .TakeWhile(token => token.CharacterType == SingleLineCommentCharacterType.StateChangeStrings);
            var identifiers = tokens
                .Skip(comments.Count())
                .TakeWhile(token => token.CharacterType == CodeCharacterType.IdentifierCharacters);

            var name = identifiers.Last().Characters;
            var method = new Method {Name = name};
            @object.Methods.Add(method);
            return method;
        }
    }
}