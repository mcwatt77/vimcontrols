using System;
using System.Collections.Generic;

namespace CSTokenizer.Handlers
{
    public static class StringLiteralCharacterType
    {
        public static readonly CharacterType StateChangeStrings = new CharacterType("Literal.StateChange");
    }

    public class StringLiteralHandler : NewHandler
    {
        public override CharacterType GetDefaultCharacterType()
        {
            return StringLiteralCharacterType.StateChangeStrings;
        }

        public StringLiteralHandler(CommonHandlerData data) : base(data) { }

        public override Dictionary<CharacterType, CharDescriptor> GetCharacterMap()
        {
            return new Dictionary<CharacterType, CharDescriptor>
                       {
                           {StringLiteralCharacterType.StateChangeStrings, CharDescriptor.FromStrings("\"", "\\")}
                       };
        }

        protected override Dictionary<string, Type> GetStateChangeStrings()
        {
            return new Dictionary<string, Type>
                       {
                           {"\"", typeof (CodeHandler)}, {"\\", typeof(StringEscapeHandler)}
                       };
        }

        public override Dictionary<CharacterType, Action> GetActionMap()
        {
            return new Dictionary<CharacterType, Action>
                       {
                           {StringLiteralCharacterType.StateChangeStrings, ChangeState}
                       };
        }

        public override Action GetDefaultAction()
        {
            return AddBufferToToken;
        }
    }
}