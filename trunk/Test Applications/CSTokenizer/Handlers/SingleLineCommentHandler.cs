using System;
using System.Collections.Generic;

namespace CSTokenizer
{
    public static class SingleLineCommentCharacterType
    {
        public static readonly CharacterType StateChangeStrings = new CharacterType("Comment.StateChange");
    }

    public class SingleLineCommentHandler : NewHandler
    {
        public override CharacterType GetDefaultCharacterType()
        {
            return SingleLineCommentCharacterType.StateChangeStrings;
        }

        public SingleLineCommentHandler(CommonHandlerData data) : base(data) { }

        public override Dictionary<CharacterType, CharDescriptor> GetCharacterMap()
        {
            return new Dictionary<CharacterType, CharDescriptor>
                       {
                           {SingleLineCommentCharacterType.StateChangeStrings, CharDescriptor.FromStrings("\r", "\n")}
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
                           {SingleLineCommentCharacterType.StateChangeStrings, ChangeState}
                       };
        }

        public override Action GetDefaultAction()
        {
            return AddBufferToToken;
        }
    }
}