using System.Windows.Input;
using ActionDictionary.MapAttributes;

namespace ActionDictionary.Interfaces
{
    public interface ITextInput
    {
        [CharacterMap(InputMode.Insert, "<a-z><space>1234567890<cr>", "abcdefghijklmnopqrstuvwxyz 1234567890\n")]
        [CharacterMapShift(InputMode.Insert, "ABCDEFGHIJKLMNOPQRSTUVWXYZ!@#$%^&*()~_+{}:|", "ABCDEFGHIJKLMNOPQRSTUVWXYZ!@#$%^&*()~_+{}:|")]
        [CharacterMapKeys(InputMode.Insert, " ", 2, Key.LeftShift, Key.Space)]
        void InputCharacter(char c);
        [KeyMap(InputMode.Normal, "a")]
        void InsertAfterCursor();
        [KeyMap(InputMode.Normal, "i")]
        void InsertBeforeCursor();
        [KeyMap(InputMode.Insert, "<bksp>")]
        void DeleteBeforeCursor();
    }
}