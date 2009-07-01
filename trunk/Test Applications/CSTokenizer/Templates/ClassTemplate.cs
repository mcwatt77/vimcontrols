using System.Linq;
using CSTokenizer.Handlers;
using Model;

namespace CSTokenizer.Templates
{
    public class ClassTemplate : Template<Namespace>
    {
        public ClassTemplate() : base(tokens => tokens.Count(token => token.Characters == "class") > 0) { }

        public override object Process(Token[] tokens, Namespace @object)
        {
            var comments = tokens
                .TakeWhile(token => token.CharacterType == SingleLineCommentCharacterType.StateChangeStrings);
            var identifiers = tokens
                .Skip(comments.Count())
                .TakeWhile(token => token.CharacterType == CodeCharacterType.IdentifierCharacters);

            var name = identifiers.Last().Characters;
            var @class = new Class();
            @object.CodeListing.Assembly.Classes.Add(@class);
            @class.Namespace = @object.Name;
            @class.Name = name;
            return @class;
        }
    }
}