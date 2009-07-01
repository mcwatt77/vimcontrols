using System;
using System.Collections.Generic;

namespace CSTokenizer.Handlers
{
    public static class CharacterLiteralCharacterType
    {
        public static readonly CharacterType StateChangeStrings = new CharacterType("Character.StateChange");
    }

    public class CharacterLiteralHandler : NewHandler
    {
        public override CharacterType GetDefaultCharacterType()
        {
            return CharacterLiteralCharacterType.StateChangeStrings;
        }

        public CharacterLiteralHandler(CommonHandlerData data) : base(data) { }

        public override Dictionary<CharacterType, CharDescriptor> GetCharacterMap()
        {
            return new Dictionary<CharacterType, CharDescriptor>
                       {
                           {CharacterLiteralCharacterType.StateChangeStrings, CharDescriptor.FromStrings("'")}
                       };
        }

        protected override Dictionary<string, Type> GetStateChangeStrings()
        {
            return new Dictionary<string, Type>
                       {
                           {"'", typeof (CodeHandler)}
                       };
        }

        public override Dictionary<CharacterType, Action> GetActionMap()
        {
            return new Dictionary<CharacterType, Action>
                       {
                           {CharacterLiteralCharacterType.StateChangeStrings, ChangeState}
                       };
        }

        public override Action GetDefaultAction()
        {
            return AddBufferToToken;
        }
    }
}