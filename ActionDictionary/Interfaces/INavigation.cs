using ActionDictionary.MapAttributes;

namespace ActionDictionary.Interfaces
{
    public interface INavigation
    {
        [KeyMap(InputMode.Normal, "k")]
        void MoveUp();
        [KeyMap(InputMode.Normal, "j")]
        void MoveDown();
        [KeyMap(InputMode.Normal, "l")]
        void MoveRight();
        [KeyMap(InputMode.Normal, "h")]
        void MoveLeft();
        [KeyMap(InputMode.Normal, "gg")]
        void Beginning();
        [KeyMap(InputMode.Normal, "G")]
        void End();
    }
}