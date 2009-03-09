using ActionDictionary.MapAttributes;

namespace ActionDictionary.Interfaces
{
    public interface INavigation
    {
        [KeyMap(InputMode.Normal, "l")]
        void MoveRight();
        [KeyMap(InputMode.Normal, "h")]
        void MoveLeft();
    }
}