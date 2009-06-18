using System;
using System.Collections.Generic;

namespace CSTokenizer
{
    public static class CodeCharacterType
    {
        public static readonly CharacterType Whitespace = new CharacterType("Code.Whitespace");
        public static readonly CharacterType ControlCharacters = new CharacterType("Code.Control");
        public static readonly CharacterType OperatorStrings = new CharacterType("Code.Operator");
        public static readonly CharacterType IdentifierCharacters = new CharacterType("Code.Identifier");
        public static readonly CharacterType StateChangeStrings = new CharacterType("Code.StateChange");
    }

    public class CodeHandler : NewHandler
    {

        public override CharacterType GetDefaultCharacterType()
        {
            return CodeCharacterType.IdentifierCharacters;
        }

        public CodeHandler(CommonHandlerData data) : base(data) { }

        protected override Dictionary<string, Type> GetStateChangeStrings()
        {
            return new Dictionary<string, Type>
                       {
                           {"\"", typeof (StringLiteralHandler)}, {"'", typeof(Handler)}, {"//", typeof(SingleLineCommentHandler)},
                           {"/*", typeof(MultiLineCommentHandler)}, {"@\"", typeof(VerbatimStringLiteralHandler)}, {"#", typeof(Handler)}
                       };
        }

        public override Dictionary<CharacterType, CharDescriptor> GetCharacterMap()
        {
            return new Dictionary<CharacterType, CharDescriptor>
                       {
                           {CodeCharacterType.Whitespace, CharDescriptor.FromRange(" \r\n\t")},
                           {CodeCharacterType.ControlCharacters, CharDescriptor.FromRange(".;()[]{},:~")},
                           {CodeCharacterType.OperatorStrings, CharDescriptor.FromStrings("=", "<", ">", "+", "-", "/",
                               "*", "%", "&", "|", "^", "!", "--", "&&", "||", "<<", ">>", "==", "!=", "<=", ">=",
                               "+=", "-=", "*=", "/=", "%=", "&=", "|=", "^=", "<<=", ">>=", "=>")},
                           {CodeCharacterType.IdentifierCharacters, CharDescriptor.FromRange("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890_")},
                           {CodeCharacterType.StateChangeStrings, CharDescriptor.FromStrings("\"", "'", "//", "/*", "@\"", "#")}
                       };
        }

        public override Dictionary<CharacterType, Action> GetActionMap()
        {
            return new Dictionary<CharacterType, Action>
                       {
                           {CodeCharacterType.Whitespace, PushTokenAndReset},
                           {CodeCharacterType.ControlCharacters, ProcessControl},
                           {CodeCharacterType.OperatorStrings, ProcessControl},
                           {CodeCharacterType.IdentifierCharacters, AddBufferToToken},
                           {CodeCharacterType.StateChangeStrings, ChangeState}
                       };
        }
    }
}