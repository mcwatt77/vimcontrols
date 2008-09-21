using VIMControls.Interfaces.Framework;

namespace VIMControls.Input
{
    public interface ITextInput
    {
        [KeyMapInsert(@"\[a-z]", "$1")]
        void InsertCharacter(char c);
    }
}
