using System.Linq;
using Model;
using Utility.Core;

namespace CSTokenizer.Templates
{
    public class StatementPreprocessTemplate : Template<NestedStatement>
    {
        public StatementPreprocessTemplate() : base(tokens => true, 3)
        {}

        public override object Process(Token[] input, NestedStatement parent)
        {
            return ApplyTemplates(new[] {ProcessIdentifiers(input).ToArray()},
                                  parent,
                                  template => template.GetType() != typeof (StatementPreprocessTemplate))
                       .FirstOrDefault()
                   ?? parent;
        }
    }

    public class AssignmentTemplate : Template<NestedStatement>
    {
        public AssignmentTemplate() : base(tokens => Matches(tokens)) { }

        public static bool Matches(Token[] tokens)
        {
            if (tokens.Count() < 2) return false;
            if (tokens.Count() == 2)
            {
                return tokens[0].CharacterType == CodeCharacterType.IdentifierCharacters
                       && tokens[1].CharacterType == CodeCharacterType.IdentifierCharacters;
            }
            if (tokens[0].CharacterType == CodeCharacterType.IdentifierCharacters
                   && tokens[1].CharacterType == CodeCharacterType.OperatorStrings
                   && tokens[1].Characters == "=") return true;
            return (tokens[0].CharacterType == CodeCharacterType.IdentifierCharacters
                    && tokens[1].CharacterType == CodeCharacterType.IdentifierCharacters
                    && tokens[2].CharacterType == CodeCharacterType.OperatorStrings
                    && tokens[2].Characters == "=");
        }

        public override object Process(Token[] input, NestedStatement parent)
        {
            //there is a local variable declaration if there are two tokens before the equal sign
            var typeToken = input[0];
            var variableToken = input[1];
            if (variableToken.CharacterType == CodeCharacterType.OperatorStrings)
            {
                variableToken = typeToken;
                typeToken = null;
            }
            Variable variable;
            if (typeToken != null)
            {
                variable = new Variable { Name = variableToken.Characters };
                parent.LocalVariables.Add(variable);
            }
            else
                variable = parent.LocalVariables.SingleOrDefault(@var => @var.Name == variableToken.Characters);

            var variableAssignment = new VariableAssignment {Variable = variable};
            parent.Statements.Add(variableAssignment);

            ApplyTemplates(new []{input.Skip(typeToken == null ? 2 : 3).ToArray()}, variableAssignment);
            return variable;
        }
    }

    public class StatementTemplate : Template<NestedStatement>
    {
        public StatementTemplate() : base(tokens => Matches(tokens)) { }

        public static bool Matches(Token[] tokens)
        {
            return true;
        }

        public override object Process(Token[] tokens, NestedStatement @object)
        {
            //if it fits the pattern:  declaration? identifier Equals <stuff>, it's an assignment
            //If it fits the pattern: identifier < identifier (, identifier)* > it's a type identifier
            //The declaration might be a generic definition
            //Should be able to call apply templates to part of the tokens...
            var statement = new NestedStatement
                                {
                                    Text = tokens
                                        .Select(token => token.Characters)
                                        .SeparateBy(" ")
                                };
            @object.Statements.Add(statement);

            return statement;
        }
    }
}