using ActionDictionary.MapAttributes;

namespace ActionDictionary.Interfaces
{
    public interface IControlKeyProcessor
    {
        [KeyMap(InputMode.Normal, "<cr>")]
        void Enter();
        [KeyMap(InputMode.Normal, "<tab>")]
        void WindowScroll();
        [KeyMap(InputMode.Normal, "<space>")]
        void LocalScroll();
    }
}
