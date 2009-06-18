using System;
using System.Collections.Generic;
using System.IO;

namespace Parser
{
    public class StatementParserNode : ParseTemplate
    {
        public StatementParserNode(Func<ParseNode, IEnumerable<ParseNode>> apply)
            : base(node => node.GetType() == typeof(StatementNode), apply)
        {}

        public override IEnumerable<ParseNode> Parse(ParseNode node)
        {
            var c = node.ReadChar();
            while (c != '\0')
            {
                if (c == '[')
                {
                    while (c != ']')
                        c = node.ReadChar();
                    var statement = new StatementNode
                                        {
                                            Name = "customAttribute",
                                            Data = new StringReader(node.Buffer.ToString())
                                        };
                    yield return statement;
                    c = node.ReadChar();
                    while (c == ' ' || c == '\r' || c == '\n')
                    {
                        node.ResetBuffer();
                        c = node.ReadChar();
                    }

                }
                while (c != ' ' && c != '.' && c != '\0')
                    c = node.ReadChar();

                var value = node.Buffer.ToString(0, node.Buffer.Length - 1);
                if (c == ' ')
                {
                    if (value == "using")
                    {
                        node.ResetBuffer();
                        var statement = new StatementNode
                                            {
                                                Name = "using",
                                                Data = new StringReader(node.GetStringData())
                                            };
                        yield return statement;
                        continue;
                    }

                    if (value == "class")
                    {
                        node.ResetBuffer();
                        c = node.ReadChar();
                        while (c == ' ')
                        {
                            node.ResetBuffer();
                            c = node.ReadChar();
                        }

                        while (c != ' ' && c != '\r' && c != '\n' && c != '\0')
                            c = node.ReadChar();
                        
                        var statement = new StatementNode
                                            {
                                                Name = "class",
                                                Data = new StringReader(node.Buffer.ToString())
                                            };
                        yield return statement;

                        continue;
                    }

                    if (value == "public")
                    {
                        node.ResetBuffer();
                        var statement = new StatementNode
                                            {
                                                Name = "accessModifier",
                                                Data = new StringReader("public")
                                            };
                        yield return statement;
                        c = node.ReadChar();
                        while (c == ' ')
                            c = node.ReadChar();
                        continue;
                    }

                    if (value == "namespace")
                    {
                        node.ResetBuffer();
                        var statement = new StatementNode()
                                            {
                                                Name = "namespace",
                                                Data = new StringReader(node.GetStringData())
                                            };
                        yield return statement;
                        continue;
                    }
                }
                yield break;
            }
            yield break;
        }
    }
}