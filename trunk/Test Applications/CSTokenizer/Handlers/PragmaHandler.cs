using System;
using System.Collections.Generic;

namespace CSTokenizer.Handlers
{
    public static class PragmaCharacterType
    {
        public static readonly CharacterType StateChangeStrings = new CharacterType("Pragma.StateChange");
    }

    public class PragmaHandler : NewHandler
    {
        public override CharacterType GetDefaultCharacterType()
        {
            return PragmaCharacterType.StateChangeStrings;
        }

        public PragmaHandler(CommonHandlerData data) : base(data) { }

        public override Dictionary<CharacterType, CharDescriptor> GetCharacterMap()
        {
            return new Dictionary<CharacterType, CharDescriptor>
                       {
                           {PragmaCharacterType.StateChangeStrings, CharDescriptor.FromStrings("\r", "\n")}
                       };
        }

        protected override Dictionary<string, Type> GetStateChangeStrings()
        {
            return new Dictionary<string, Type>
                       {
                           {"\r", typeof (CodeHandler)}, {"\n", typeof(CodeHandler)}
                       };
        }

        public override Dictionary<CharacterType, Action> GetActionMap()
        {
            return new Dictionary<CharacterType, Action>
                       {
                           {PragmaCharacterType.StateChangeStrings, ChangeState}
                       };
        }

        public override Action GetDefaultAction()
        {
            return AddBufferToToken;
        }
    }
}