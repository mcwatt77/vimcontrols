using System;
using System.Collections.Generic;

namespace CSTokenizer
{
    public class MultiLineCommentHandler : NewHandler
    {
        private static class MultiLineCommentCharacterType
        {
            public static readonly CharacterType StateChangeStrings = new CharacterType("MultiLineComment.StateChange");
        }

        public MultiLineCommentHandler(CommonHandlerData commonData) : base(commonData) { }

        public override Dictionary<CharacterType, CharDescriptor> GetCharacterMap()
        {
            return new Dictionary<CharacterType, CharDescriptor>
                       {
                           {MultiLineCommentCharacterType.StateChangeStrings, CharDescriptor.FromStrings("*/")}
                       };
        }

        protected override Dictionary<string, Type> GetStateChangeStrings()
        {
            return new Dictionary<string, Type>
                       {
                           {"*/", typeof (CodeHandler)}
                       };
        }

        public override Dictionary<CharacterType, Action> GetActionMap()
        {
            return new Dictionary<CharacterType, Action>
                       {
                           {MultiLineCommentCharacterType.StateChangeStrings, ChangeState}
                       };
        }

        public override CharacterType GetDefaultCharacterType()
        {
            return MultiLineCommentCharacterType.StateChangeStrings;
        }

        public override Action GetDefaultAction()
        {
            return AddBufferToToken;
        }
    }
}