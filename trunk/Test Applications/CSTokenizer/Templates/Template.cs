using System.Linq;

using Model;

namespace CSTokenizer
{
    public abstract class Template
    {
        public abstract bool Matches(Token[] tokens, object @object);
        public abstract object Process(Token[] tokens, object @object);
    }
    public abstract class Template<TObject> : Template
    {
        public abstract bool Matches(Token[] tokens, TObject @object);
        public abstract object Process(Token[] tokens, TObject @object);

        public override bool Matches(Token[] tokens, object @object)
        {
            if (!typeof(TObject).IsAssignableFrom(@object.GetType())) return false;
            return Matches(tokens, (TObject) @object);
        }

        public override object Process(Token[] tokens, object @object)
        {
            return Process(tokens, (TObject) @object);
        }
    }

    public class UsingTemplate : Template<Model.CodeListing>
    {
        public override bool Matches(Token[] tokens, CodeListing @object)
        {
            if (tokens.Length == 0) return false;
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

    public class NamespaceTemplate : Template<Model.CodeListing>
    {
        public override bool Matches(Token[] tokens, CodeListing @object)
        {
            if (tokens.Length == 0) return false;
            return tokens.First().Characters == "namespace";
        }

        public override object Process(Token[] tokens, CodeListing @object)
        {
            var name = tokens.Skip(1).Aggregate("", (s, t) => s + t.Characters);
            var result = new Model.Namespace();
            result.CodeListing = @object;
            result.Name = name;
            return result;
        }
    }

    public class MethodTemplate : Template<Model.Class>
    {
        public override bool Matches(Token[] tokens, Class @object)
        {
            return tokens
                       .Count(token => token.CharacterType == CodeCharacterType.ControlCharacters
                                       && token.Characters == "(") > 0
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

    public class ClassTemplate : Template<Model.Namespace>
    {
        public override bool Matches(Token[] tokens, Namespace @object)
        {
            return tokens.Count(token => token.Characters == "class") > 0;
        }

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
