using System;
using System.Collections.Generic;

namespace CSTokenizer
{
    public class StringLiteralHandler : NewHandler
    {
        private static class StringLiteralCharacterType
        {
            public static readonly CharacterType StateChangeStrings = new CharacterType("Literal.StateChange");
        }

        public override CharacterType GetDefaultCharacterType()
        {
            return StringLiteralCharacterType.StateChangeStrings;
        }

        public StringLiteralHandler(CommonHandlerData data) : base(data) { }

        public override Dictionary<CharacterType, CharDescriptor> GetCharacterMap()
        {
            return new Dictionary<CharacterType, CharDescriptor>
                       {
                           {StringLiteralCharacterType.StateChangeStrings, CharDescriptor.FromStrings("\"")}
                       };
        }

        protected override Dictionary<string, Type> GetStateChangeStrings()
        {
            return new Dictionary<string, Type>
                       {
                           {"\"", typeof (CodeHandler)}
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