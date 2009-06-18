using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Parser
{
    public class NestedStatementParserNode : ParseTemplate
    {
        public NestedStatementParserNode(Func<ParseNode, IEnumerable<ParseNode>> apply)
            : base(node => node.Name == "root" || node.GetType() == typeof(NestedStatementNode), apply)
        {}

        //There are a couple of ways to accomplish this...
        //When I hit {, I could go into recursive mode
        //Alternatively, when I hit { I could seek ahead for the end node
        //This causes hard to follow looping logic if done correctly though
        public override IEnumerable<ParseNode> Parse(ParseNode node)
        {
            var c = node.ReadChar();
            while (c != '\0')
            {
                while (c != ';' && c != '{' && c != '\0' && c != '/' && c != '\'' && c != '"')
                    c = node.ReadChar();

                if (c == '"')
                {
                    c = ProcessStringLiteral(node);
                    continue;
                }
                if (c == '/')
                {
                    c = node.ReadChar();
                    if (c == '/')
                    {
                        while (c != '\r')
                            c = node.ReadChar();
                    }
                }
                if (c == '\'')
                {
                    c = ProcessCharacterLiteral(node);
                    continue;
                }
                if (c == ';')
                {
                    var statement = new StatementNode
                                        {
                                            Name = "statement",
                                            Data = new StringReader(node.Buffer.ToString().TrimStart(' ', '\r', '\n', '\t'))
                                        };
                    node.ResetBuffer();
                    c = node.ReadChar();
                    statement.Children = ApplyTemplates(statement).ToArray();

                    yield return statement;
                    continue;
                }
                if (c == '{')
                {
                    var nestCount = 1;
                    while (nestCount > 0)
                    {
                        c = node.ReadChar();
                        //Would be much better to create CharacterLiteral, StringLiteral, Comment classes, then parse them
                        if (c == '/')
                        {
                            c = node.ReadChar();
                            if (c == '/')
                            {
                                while (c != '\r')
                                    c = node.ReadChar();
                            }
                        }
                        if (c == '\'')
                        {
                            ProcessCharacterLiteral(node);
                            continue;
                        }
                        if (c == '{') nestCount++;
                        if (c == '}') nestCount--;
                    }
                    var stringData = node.Buffer.ToString().TrimStart(' ', '\r', '\n', '\t');
                    var firstBracket = stringData.IndexOf('{');
                    var innerData = stringData.Substring(firstBracket + 1, stringData.Length - firstBracket - 2).Trim();

                    var children = new List<ParseNode>();
                    var statementNode = new StatementNode
                                            {
                                                Name = "command",
                                                Data =
                                                    new StringReader(stringData.Substring(0, firstBracket - 1).Trim())
                                            };
                    statementNode.Children = ApplyTemplates(statementNode).ToArray();
                    children.Add(statementNode);

                    var nestedNode = new NestedStatementNode
                                         {
                                             CommandData = stringData.Substring(0, firstBracket - 1).Trim(),
                                             Name = "nestedStatement",
                                             Data = new StringReader(innerData)
                                         };
                    children.AddRange(ApplyTemplates(nestedNode));

                    nestedNode.Children = children;
                    yield return nestedNode;
                    node.ResetBuffer();
                    c = node.ReadChar();
                    continue;
                }
                yield break;
            }
        }

        private char ProcessCharacterLiteral(ParseNode node)
        {
            char c;
            c = node.ReadChar();
            if (c == '\\')
                node.ReadChar();
            c = node.ReadChar();
            return c;
        }

        private char ProcessStringLiteral(ParseNode node)
        {
            char c;
            c = node.ReadChar();
            while (c == '\\')
            {
                node.ReadChar();
                c = node.ReadChar();
            }
            while (c != '"')
            {
                c = node.ReadChar();
                {
                    while (c == '\\')
                    {
                        node.ReadChar();
                        c = node.ReadChar();
                    }
                }
            }
            c = node.ReadChar();
            return c;
        }
    }
}