using ActionDictionary.MapAttributes;

namespace ActionDictionary.Interfaces
{
    public interface IPaging
    {
        [KeyMap(InputMode.Normal, "k")]
        void MoveUp();
        [KeyMap(InputMode.Normal, "j")]
        void MoveDown();
        [KeyMap(InputMode.Normal, "gg")]
        void Beginning();
        [KeyMap(InputMode.Normal, "G")]
        void End();
        [KeyMap(InputMode.Normal, "<c-u>")]
        void PageUp();
        [KeyMap(InputMode.Normal, "<c-d>")]
        void PageDown();
    }
}
