using System.Collections.Generic;
using System.Linq;
using CSTokenizer.Handlers;
using Model;
using Utility.Core;

namespace CSTokenizer.Templates
{
    public static class CommentTemplateProcessor
    {
        public static Token[] Process(Token[] tokens, NestedStatement @object)
        {
            var comments = GetComments(tokens);
            if (comments.Count() > 0)
            {
                var cStatement = new CommentStatement {Text = comments.Select(token => token.Characters).SeparateBy("\r\n")};
                @object.Statements.Add(cStatement);
            }

            return tokens.Skip(comments.Count()).ToArray();
        }

        public static Token[] Process(Token[] tokens, object @object)
        {
            var comments = GetComments(tokens);

            return tokens.Skip(comments.Count()).ToArray();
        }

        private static Token[] GetComments(IEnumerable<Token> tokens)
        {
            return tokens
                .TakeWhile(token => token.CharacterType == SingleLineCommentCharacterType.StateChangeStrings)
                .ToArray();
        }

        public static bool Matches(IEnumerable<Token> tokens)
        {
            return tokens
                       .Select(token => token.CharacterType)
                       .FirstOrDefault() == SingleLineCommentCharacterType.StateChangeStrings;
        }
    }

    public class CommentTemplate : Template<NestedStatement>
    {
        public CommentTemplate() : base(tokens => Matches(tokens), 5) {}

        private static bool Matches(IEnumerable<Token> tokens)
        {
            return CommentTemplateProcessor.Matches(tokens);
        }

        public override object Process(Token[] tokens, NestedStatement @object)
        {
            return ApplyTemplates(new[] {CommentTemplateProcessor.Process(tokens, @object)}, @object)
                       .FirstOrDefault() ?? @object;
        }
    }

    public class RootCommentTemplate : Template<object>
    {
        public RootCommentTemplate() : base(tokens => Matches(tokens), 5) {}

        private static bool Matches(IEnumerable<Token> tokens)
        {
            return CommentTemplateProcessor.Matches(tokens);
        }

        public override object Process(Token[] tokens, object @object)
        {
            return ApplyTemplates(new[] {CommentTemplateProcessor.Process(tokens, @object)}, @object)
                       .FirstOrDefault() ?? @object;
        }
    }
}