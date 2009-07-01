using System;
using System.Collections.Generic;

namespace CSTokenizer.Handlers
{
    public static class StringEscapeCharacterType
    {
        public static readonly CharacterType StateChangeStrings = new CharacterType("StringEscape.StateChange");
    }

    public class StringEscapeHandler : NewHandler
    {
        public override CharacterType GetDefaultCharacterType()
        {
            return StringLiteralCharacterType.StateChangeStrings;
        }

        public StringEscapeHandler(CommonHandlerData data) : base(data) { }

        public override Dictionary<CharacterType, CharDescriptor> GetCharacterMap()
        {
            return new Dictionary<CharacterType, CharDescriptor>();
        }

        protected override Dictionary<string, Type> GetStateChangeStrings()
        {
            return new Dictionary<string, Type>();
        }

        public override Dictionary<CharacterType, Action> GetActionMap()
        {
            return new Dictionary<CharacterType, Action>
                       {
                           {StringEscapeCharacterType.StateChangeStrings, ChangeState}
                       };
        }

        public override Action GetDefaultAction()
        {
            return ChangeState;
        }

        public override Type GetDefaultStateChange()
        {
            return typeof (StringLiteralHandler);
        }
    }
}