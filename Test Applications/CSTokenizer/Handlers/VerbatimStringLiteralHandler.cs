using System;
using System.Collections.Generic;

namespace CSTokenizer
{
    public class VerbatimStringLiteralHandler : NewHandler
    {
        private static class VerbatimLiteralCharacterType
        {
            public static readonly CharacterType StateChangeStrings = new CharacterType("Verbatim.StateChange");
        }

        public override CharacterType GetDefaultCharacterType()
        {
            return VerbatimLiteralCharacterType.StateChangeStrings;
        }

        public VerbatimStringLiteralHandler(CommonHandlerData data) : base(data) { }

        public override Dictionary<CharacterType, CharDescriptor> GetCharacterMap()
        {
            return new Dictionary<CharacterType, CharDescriptor>
                       {
                           {VerbatimLiteralCharacterType.StateChangeStrings, CharDescriptor.FromStrings("\"")}
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
                           {VerbatimLiteralCharacterType.StateChangeStrings, ChangeState}
                       };
        }

        public override Action GetDefaultAction()
        {
            return AddBufferToToken;
        }
    }
}