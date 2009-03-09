using ActionDictionary.MapAttributes;

namespace ActionDictionary.Interfaces
{
    public interface IPaging
    {
        [KeyMap(InputMode.Normal, "<c-u>")]
        void PageUp();
        [KeyMap(InputMode.Normal, "<c-d>")]
        void PageDown();
    }
}
